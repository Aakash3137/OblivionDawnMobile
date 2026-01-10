using UnityEngine;

public enum AirState
{
    Ascending,
    Airborne,
    Attacking,
    Evading
}

public class AirUnit : MonoBehaviour
{
    [Header("Flight Settings")]
    public float flyHeight = 5f;
    public float climbAngle = 45f;
    public float moveSpeed = 5f;
    public float turnSpeed = 2f;
    public float bankAngle = 15f;

    [Header("Combat Settings")]
    public int burstCount = 6;
    public float burstCooldown = 0.1f;
    public float evadeRadius = 50f;
    public float attackAngleTolerance = 90f;

    [Header("Takeoff Protection")]
    public bool invulnerableDuringTakeoff = true;

    private AirState airState = AirState.Ascending;
    private BattleUnit battleUnit;
    private int shotsFired = 0;
    private float burstTimer = 0f;
    private Vector3 evadeCenter;
    private float evadeAngle = 0f;
    private int evadeDirection = 1;

    void Start()
    {
        battleUnit = GetComponent<BattleUnit>();
    }

    void Update()
    {
        if (airState == AirState.Ascending)
            PerformTakeoff();
        else if (airState == AirState.Evading)
            PerformEvade();
    }

    /* ===================== TAKEOFF ===================== */

    void PerformTakeoff()
    {
        float currentHeight = transform.position.y;
        float progress = Mathf.Clamp01(currentHeight / flyHeight);

        float pitchAngle = Mathf.Lerp(climbAngle, 0f, progress);

        Quaternion pitchRotation =
            Quaternion.AngleAxis(-pitchAngle, Vector3.right);

        Quaternion yawRotation =
            Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            yawRotation * pitchRotation,
            3f * Time.deltaTime
        );

        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if (currentHeight >= flyHeight - 0.1f)
        {
            Vector3 pos = transform.position;
            pos.y = flyHeight;
            transform.position = pos;
            airState = AirState.Airborne;
        }
    }

    /* ===================== NORMAL FLIGHT ===================== */

    public void FlyTowards(Vector3 targetPosition)
    {
        if (airState == AirState.Ascending || airState == AirState.Evading)
            return;

        targetPosition.y = flyHeight;

        Vector3 dir = (targetPosition - transform.position).normalized;
        if (dir == Vector3.zero) return;

        // --- FLATTEN VECTORS (CRITICAL FIX) ---
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;

        if (flatDir == Vector3.zero) return;

        // --- SIGNED TURN ANGLE ---
        float signedAngle = Vector3.SignedAngle(flatForward, flatDir, Vector3.up);

        // --- BANK ---
        float bank = Mathf.Clamp(
            signedAngle * 0.5f,
            -bankAngle,
            bankAngle
        );

        Quaternion yawRotation = Quaternion.LookRotation(flatDir);
        Quaternion bankRotation = Quaternion.AngleAxis(-bank, Vector3.forward);
        Quaternion finalRotation = yawRotation * bankRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRotation,
            turnSpeed * Time.deltaTime
        );

        Vector3 pos = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        pos.y = flyHeight;
        transform.position = pos;
    }

    /* ===================== SEPARATION ===================== */

    public void ApplySeparation(Vector3 separationForce)
    {
        if (airState != AirState.Airborne && airState != AirState.Attacking)
            return;

        Vector3 pos = transform.position + separationForce * Time.deltaTime;
        pos.y = flyHeight;
        transform.position = pos;
    }

    /* ===================== EVADE ===================== */

    void PerformEvade()
    {
        // Rotate angle in chosen direction
        evadeAngle += 60f * evadeDirection * Time.deltaTime;

        float rad = evadeAngle * Mathf.Deg2Rad;

        float x = Mathf.Cos(rad) * evadeRadius;
        float z = Mathf.Sin(rad) * evadeRadius;

        Vector3 targetPos = evadeCenter + new Vector3(x, 0, z);
        targetPos.y = flyHeight;

        Vector3 dir = (targetPos - transform.position).normalized;
        if (dir == Vector3.zero) return;

        // --- FLATTEN VECTORS ---
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatDir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;

        if (flatDir == Vector3.zero) return;

        // --- SIGNED TURN ANGLE (AUTO LEFT / RIGHT) ---
        float signedAngle = Vector3.SignedAngle(flatForward, flatDir, Vector3.up);

        // --- BANK INTO TURN ---
        float bank = Mathf.Clamp(
            signedAngle * 0.8f,
            -30f,
            30f
        );

        Quaternion yawRotation = Quaternion.LookRotation(flatDir);
        Quaternion bankRotation = Quaternion.AngleAxis(-bank, Vector3.forward);
        Quaternion finalRotation = yawRotation * bankRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            finalRotation,
            turnSpeed * Time.deltaTime
        );

        Vector3 pos = transform.position + transform.forward * moveSpeed * Time.deltaTime;
        pos.y = flyHeight;
        transform.position = pos;

        // Completed one full circle
        if (Mathf.Abs(evadeAngle) >= 360f)
        {
            airState = AirState.Airborne;
            shotsFired = 0;

            // Pick next evade direction randomly
            evadeDirection = Random.value > 0.5f ? 1 : -1;
            evadeAngle = 0f;
        }
    }

    /* ===================== COMBAT ===================== */

    public bool CanAttack()
    {
        return airState == AirState.Airborne || airState == AirState.Attacking;
    }

    public bool IsFacingTarget(GameObject target)
    {
        if (!target) return false;

        Vector3 targetPos = target.transform.position;
        targetPos.y = transform.position.y;

        Vector3 dirToTarget = (targetPos - transform.position).normalized;
        return Vector3.Angle(transform.forward, dirToTarget) <= attackAngleTolerance;
    }

    public bool ShouldFireBurst(GameObject target)
    {
        if (airState == AirState.Evading) return false;
        if (!IsFacingTarget(target)) return false;
        if (shotsFired >= burstCount) return false;

        burstTimer += Time.deltaTime;

        if (burstTimer >= burstCooldown)
        {
            burstTimer = 0f;
            shotsFired++;

            if (shotsFired >= burstCount)
            {
                airState = AirState.Evading;
                evadeCenter = transform.position;
                evadeAngle = 0f;
            }
            else
            {
                airState = AirState.Attacking;
            }
            return true;
        }
        return false;
    }

    /* ===================== QUERIES ===================== */

    public bool IsAirborne()
    {
        return airState != AirState.Ascending;
    }

    public bool IsEvading()
    {
        return airState == AirState.Evading;
    }

    public bool CanBeTargeted()
    {
        return !invulnerableDuringTakeoff || airState == AirState.Airborne;
    }
}
