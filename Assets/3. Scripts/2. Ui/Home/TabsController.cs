using UnityEngine;

public class TabsController : MonoBehaviour
{
    [Header("Assign In Enum Order")]
    [SerializeField] private TabButton[] buttons;   // size 5
    [SerializeField] private GameObject[] panels;   // size 5

    private int currentIndex = -1;

    private void Awake()
    {
        // Safety check (editor only)
#if UNITY_EDITOR
        if (buttons.Length != 5 || panels.Length != 5)
            Debug.LogError("TabsController requires exactly 5 buttons and 5 panels.");
#endif

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Initialize(this, (UITab)i);
        }
    }

    private void Start()
    {
        SelectTab(UITab.Home);
    }

    public void SelectTab(UITab tab)
    {
        int newIndex = (int)tab;

        if (newIndex == currentIndex)
            return;

        // Disable previous
        if (currentIndex >= 0)
        {
            panels[currentIndex].SetActive(false);
            buttons[currentIndex].SetSelected(false);
        }

        // Enable new
        panels[newIndex].SetActive(true);
        buttons[newIndex].SetSelected(true);

        currentIndex = newIndex;
    }
}