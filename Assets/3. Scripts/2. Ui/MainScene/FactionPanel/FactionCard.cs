using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactionCard : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text factionName;
    public TMP_Text NodataMessage;
    private bool NoData;
    [SerializeField] internal Button startButton;

    [Header("Data")]
    public string factionId;
    [SerializeField] private FactionName CardOf;
    public DecSelectionData SelectedFactionData;

    [Header("Parents")]
    [SerializeField] private Transform Offense_Parent;
    [SerializeField] private Transform Defense_Parent;
    [SerializeField] private Transform Resource_Parent;

    [Header("Prefab")]
    [SerializeField] private FactionEntityDetails BlockPrefab;

    [SerializeField] private Vector3 normalScale;
    [SerializeField] private Vector3 selectedScale;

    private bool blocksGenerated;


    void OnEnable()
    {
        GenerateBlocks();
        if(NoData)
        {
            NodataMessage.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
        else
        {
            NodataMessage.gameObject.SetActive(false);
        }
    }

    public void SetSelected(bool selected)
    {
        transform.localScale = selected ? selectedScale : normalScale;


        if ((selected && !NoData) || (!selected && !NoData))
        {
            startButton.gameObject.SetActive(selected);
            NodataMessage.gameObject.SetActive(false);
            blocksGenerated = true;
        }
        else if(NoData)
        {
            NodataMessage.gameObject.SetActive(true);
            startButton.gameObject.SetActive(false);
        }
    }

    private void GenerateBlocks()
    {
        if (SelectedFactionData == null) return;

        ClearParents();
        foreach (DeckData faction in SelectedFactionData.AllFactionDecData)
        {
            if (faction.FactionType != CardOf)
                continue;

            bool noData =
                (faction.SelectedUnitDeck?.Count ?? 0) == 0 &&
                (faction.SelectedDefenseDec?.Count ?? 0) == 0 &&
                (faction.SelectedResourceDeck?.Count ?? 0) == 0;

            NoData = noData;
            NodataMessage.text = noData ? "Not Selected this Faction.....!" : "";

            if (noData)
                break;

            foreach (var unit in faction.SelectedUnitDeck)
                Instantiate(BlockPrefab, Offense_Parent).SetData(unit);

            foreach (var def in faction.SelectedDefenseDec)
                Instantiate(BlockPrefab, Defense_Parent).SetData(def);

            foreach (var res in faction.SelectedResourceDeck)
                Instantiate(BlockPrefab, Resource_Parent).SetData(res);

            break; // 👈 IMPORTANT: only one faction should be processed
        }
    }

    private void ClearParents()
    {
        Clear(Offense_Parent);
        Clear(Defense_Parent);
        Clear(Resource_Parent);
    }

    private void Clear(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
            Destroy(t.GetChild(i).gameObject);
    }
}
