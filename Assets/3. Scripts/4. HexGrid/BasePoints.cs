using UnityEngine;

public class BasePoints : MonoBehaviour
{
    public Transform playerBase;
    public Transform enemyBase;

    void Start()
    {
        playerBase.tag = "Player";
        enemyBase.tag = "Enemy";
    }
}
