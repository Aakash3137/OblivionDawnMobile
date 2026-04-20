using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    private static PersistentObject instance;
    private void Awake()
    {
        // If another instance already exists, destroy this one
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign and persist
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}