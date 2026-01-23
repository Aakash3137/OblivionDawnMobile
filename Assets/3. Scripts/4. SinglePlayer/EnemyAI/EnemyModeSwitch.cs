using UnityEngine;
using UnityEngine.UI;

public enum CurrentEnemyMode
{
    EnemyAIMode,   
    EnemySelf     
}

public class EnemyModeSwitch : MonoBehaviour
{
    [Header("UI Slider to control Enemy Mode")]
    public Slider modeSlider;     
    
    // Play Enemy vs player (For Development Testing)
    [Header("Current Enemy Mode")]
    public CurrentEnemyMode EnemyMode;

    [SerializeField] internal EnemyTileClickManager _enemyTileClickManager;
    void Start()
    {
        // Set default mode even if slider is not assigned
        EnemyMode = CurrentEnemyMode.EnemyAIMode;
        Debug.Log($"[EnemyModeSwitch] Default mode set to: {EnemyMode}");
        
        if (modeSlider != null)
        {
            modeSlider.minValue = 0;
            modeSlider.maxValue = 1;
            modeSlider.wholeNumbers = true;  

            // Set default value (AI mode)
            modeSlider.value = 1;

            // Listen to slider changes
            modeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            Debug.LogWarning("[EnemyModeSwitch] Mode Slider not assigned - using default AI mode");
        }
    }

    private void OnSliderValueChanged(float value)
    {
        EnemyMode = value == 0 
            ? CurrentEnemyMode.EnemySelf 
            : CurrentEnemyMode.EnemyAIMode;

        Debug.Log($"[EnemyModeSwitch] Enemy Mode changed to: {EnemyMode}");
        
        if(EnemyMode == CurrentEnemyMode.EnemyAIMode)
        {
            if (_enemyTileClickManager != null)
            {
                _enemyTileClickManager.enabled = false;
                _enemyTileClickManager.CloseBuildPanel();
            }
        }
        else
        {
            if (_enemyTileClickManager != null)
                _enemyTileClickManager.enabled = true;
        }
    }

    public void SetEnemyMode(CurrentEnemyMode mode)
    {
        EnemyMode = mode;
        if (modeSlider != null)
        {
            modeSlider.value = mode == CurrentEnemyMode.EnemyAIMode ? 1 : 0;
        }
    }
}