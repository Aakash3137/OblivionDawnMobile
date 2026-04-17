using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    public float range1 = 5f;
    public float range2 = 10f;
    public float range3 = 15f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range1);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range2);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range3);
    }
}
