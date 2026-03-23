using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanelManager : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text percentText;
    [SerializeField] private TMP_Text loadingText;


    [Space(10)]
    [SerializeField] private float fillSpeed = 1f;
    [SerializeField] private float afterLoadingDelay = 1f;

    private float targetProgress;
    private static readonly string[] dots = { "", ".", "..", "..." };


    private void Update()
    {
        fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetProgress, fillSpeed * Time.deltaTime);

        int amount = (int)(fillImage.fillAmount * 100f);

        percentText.SetText($"{amount}%");
    }

    public void Initialize(Mode mode)
    {
        string sceneName = mode switch
        {
            Mode.Death_Solo => "SinglePlayerScene",
            Mode.MultiPlayer_Type => "SinglePlayerScene",
            Mode.PVP_Type => "SinglePlayerScene",
            Mode.Scenario_Type => "SinglePlayerScene",
            _ => "MainScene"
        };

        _ = LoadScene(sceneName);
        _ = AnimateLoadingText();
    }

    private async Awaitable LoadScene(string sceneName)
    {
        fillImage.fillAmount = 0f;
        targetProgress = 0f;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        // Yield each frame while loading Progress caps at 0.9f intentionally here <- Unity thing
        while (scene.progress < 0.9f && !destroyCancellationToken.IsCancellationRequested)
        {
            targetProgress = scene.progress;
            await Awaitable.NextFrameAsync(destroyCancellationToken);
        }

        // force full load bar 
        targetProgress = 1f;

        // Wait for the visual fill
        while (fillImage.fillAmount < 0.99f && !destroyCancellationToken.IsCancellationRequested)
            await Awaitable.NextFrameAsync(destroyCancellationToken);

        scene.allowSceneActivation = true;

        await Awaitable.WaitForSecondsAsync(afterLoadingDelay, destroyCancellationToken);

        if (sceneName == "MainScene")
            GameStateManager.Instance.ChangeState(GameStateEnum.MAIN_MENU);
        else
            GameStateManager.Instance.ChangeState(GameStateEnum.PLAYING);
    }

    private async Awaitable AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (!destroyCancellationToken.IsCancellationRequested)
        {
            loadingText.text = baseText + dots[dotCount];
            dotCount = (dotCount + 1) % 4;
            await Awaitable.WaitForSecondsAsync(0.4f, destroyCancellationToken);
        }
    }

}
