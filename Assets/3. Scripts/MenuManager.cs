using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public bool isAttackPersonality;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        DontDestroyOnLoad(gameObject);
    }
    
    
    
}
