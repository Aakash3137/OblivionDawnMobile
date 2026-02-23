using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioGem : MonoBehaviour
{
    public Userdata userdata;
    public float transformAnimationYOffset;
    public float despawnTime;

    private Button button;

    private RewardCanvas rewardCanvas;
    private MotionHandle scaleHandle;
    // private MotionHandle rotateHandle;
    // private MotionHandle moveHandle;


    private void Start()
    {
        button = GetComponent<Button>();

        // Destroy(transform.parent.gameObject, despawnTime);
        AnimateObject();
        rewardCanvas = RewardCanvas.Instance;
        button.onClick.AddListener(OnGemClaim);
    }

    private void ClaimAllGems()
    {
        rewardCanvas.ClearRewards();
    }

    public void OnGemClaim()
    {
        rewardCanvas.RemoveReward(this);

        StopAnimation();

        if (rewardCanvas.rewardUpdateUI != null)
        {
            rewardCanvas.rewardUpdateUI.SpawnGemAndAnimate(transform.position);
        }
        else
        {
            userdata.Diamonds++;
            Debug.Log("<color=red>rewardUpdateUI is null </color>");
        }
    }

    private void AnimateObject()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.1f;

        scaleHandle = LMotion.Create(startScale, endScale, 0.3f)
            .WithEase(Ease.InOutSine)
            .WithLoops(-1, LoopType.Yoyo)
            .BindToLocalScale(transform)
            .AddTo(gameObject);

        // float startRotation = transform.localRotation.eulerAngles.z;
        // float endRotation = startRotation + 45f;

        // rotateHandle = LMotion.Create(startRotation, endRotation, 1.5f)
        // .WithEase(Ease.Linear)
        // .WithLoops(-1, LoopType.Yoyo)
        // .BindToLocalEulerAnglesZ(transform)
        // .AddTo(this);
    }

    private void StopAnimation()
    {
        scaleHandle.TryCancel();
        // moveHandle.TryCancel();
        // rotateHandle.TryCancel();
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnGemClaim);
    }
}
