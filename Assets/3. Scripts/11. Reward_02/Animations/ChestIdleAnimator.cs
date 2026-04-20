using UnityEngine;
using System.Collections;
using TMPro;

public class ChestIdleAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform chestRoot;

    [Header("Float Settings")]
    public float floatHeight = 15f;      // UI units (pixels)
    public float floatDuration = 1.2f;

    [Header("Shake Settings")]
    public float shakeDuration = 0.4f;
    public float shakeStrength = 8f;

    [Header("Timing")]
    public float waitBetween = 0.5f;

    private Vector3 startPos;
    private bool isRunning = true;
    private Coroutine loopRoutine;

    private void OnEnable()
    {
        startPos = chestRoot.localPosition;
        loopRoutine = StartCoroutine(IdleLoop());
        
    }

    public void StopIdle()
    {
        isRunning = false;

        if (loopRoutine != null)
            StopCoroutine(loopRoutine);

        // Reset transform cleanly
        chestRoot.localPosition = startPos;
        chestRoot.localRotation = Quaternion.identity;
    }

    private IEnumerator IdleLoop()
    {
        while (isRunning)
        {
            // 1. FLOAT UP
            yield return MoveY(startPos, startPos + Vector3.up * floatHeight, floatDuration / 2f);

            // 2. FLOAT DOWN
            yield return MoveY(startPos + Vector3.up * floatHeight, startPos, floatDuration / 2f);

            yield return new WaitForSeconds(waitBetween);

            // 3. SHAKE
            yield return Shake();

            yield return new WaitForSeconds(waitBetween);
        }
    }

    IEnumerator MoveY(Vector3 from, Vector3 to, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.SmoothStep(0, 1, time / duration);
            chestRoot.localPosition = Vector3.Lerp(from, to, t);

            yield return null;
        }
    }

    IEnumerator Shake()
    {
        float time = 0;

        while (time < shakeDuration)
        {
            time += Time.deltaTime;

            float angle = Mathf.Sin(time * 50f) * shakeStrength;
            chestRoot.localRotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        chestRoot.localRotation = Quaternion.identity;
    }


}