using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Sirenix.OdinInspector;

public class RepairButtonHandler : MonoBehaviour
{
    [Header("Target Positions")]
    [SerializeField] private Vector2 showPosition = new Vector2(0, 0.75f);
    [SerializeField] private Vector2 hidePosition = new Vector2(0, -1f);

    [Header("Scale")]
    public Vector3 showScale = Vector3.one;
    public Vector3 hideScale = Vector3.zero;

    [Header("Animation")]
    public float duration = 0.3f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float CoolDownTimer = 15f;
    [Header ("UI")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private Button Repairbtn;

    [Header ("Data")]
    [SerializeField] internal Stats StatsData;
    [SerializeField] internal bool IsReady;
    void OnEnable()
    {
        PlayHide();
        IsReady = true;
    }

    void Start()
    {
        CheckIFEnemy();
        Repairbtn.onClick.RemoveAllListeners();
        Repairbtn.onClick.AddListener(OnClickRepairBtn);
    }

    private void OnMouseDown() 
    {
        if(!IsReady)
           return;

        Debug.Log("On Click Repair Button");
        StatsData.HealthRepair();
        Repairbtn.gameObject.SetActive(false);
        StartCoroutine(CoolDownTimerStart());
    }

    public void PlayShow()
    {
        if(!CheckIFEnemy() || !IsReady)
            return;
        
        Debug.Log("Play Show");
        Repairbtn.gameObject.SetActive(true);
       StartCoroutine(Animate(showPosition, showScale));
    }

    public void PlayHide()
    {
        StartCoroutine(HideRoutine());
    }

    bool CheckIFEnemy()
    {
        if(StatsData.side == Side.Enemy)
        {
            gameObject.SetActive(false);
            return false;
        }
        return true;
    }

    IEnumerator HideRoutine()
    {
        Repairbtn.gameObject.SetActive(false);
        yield return Animate(hidePosition, hideScale);
    }

    IEnumerator Animate(Vector2 targetPos, Vector3 targetScale)
    {
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = curve.Evaluate(time / duration);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rect.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        
        rect.anchoredPosition = targetPos;
        rect.localScale = targetScale;
    }

    [Button]
    public void OnClickRepairBtn()
    {
        if(!IsReady)
           return;

        Debug.Log("On Click Repair Button");
        StatsData.HealthRepair();
        Repairbtn.gameObject.SetActive(false);
        StartCoroutine(CoolDownTimerStart());
    }

    IEnumerator CoolDownTimerStart()
    {
        IsReady = false;
        yield return new WaitForSeconds(CoolDownTimer);
        IsReady = true;
    }
}


public static class InputHelper
{
    public static bool IsPointerOverUI()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
        return EventSystem.current.IsPointerOverGameObject();
    }
}