using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHelper : MonoBehaviour
{
    public static SceneTransitionHelper Instance;
    
    public string LastActiveScene { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Track scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LastActiveScene = scene.name;
        Debug.Log($"[SceneTransitionHelper] Scene loaded: {scene.name}");
        
        // Notify all tile selection managers
        var tileManagers = FindObjectsOfType<TileSelectionManager>();
        foreach (var manager in tileManagers)
        {
            manager.SendMessage("OnSceneChanged", scene.name, SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public bool IsInGameScene()
    {
        return LastActiveScene.Equals("GameScene", System.StringComparison.OrdinalIgnoreCase);
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}