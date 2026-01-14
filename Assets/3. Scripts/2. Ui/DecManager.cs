using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DecManager : MonoBehaviour
{
    [SerializeField] private List<DecSelector> Declist = new List<DecSelector>();
    [SerializeField] private Transform CustomDeckParent;
    [SerializeField] private Transform AllDeckParent;

    private void OnEnable()
    {
        if (Declist.Count > 0)
            OnClickButton(Declist[0].InActiveObj);
    }

    public void OnClickButton(GameObject CurrentActive)
    {
        foreach (DecSelector dec in Declist)
        {
            dec.InActiveObj.SetActive(true);
            dec._Checked.SetActive(false);
        }

        DecSelector selected = Declist.Find(dec => dec.InActiveObj == CurrentActive);

        if (selected != null)
        {
            selected.InActiveObj.SetActive(false);
            selected._Checked.SetActive(true);

            if (selected.Cards.Count > 0)
            {
                StartCoroutine(RebuildCustomDeck(selected));
            }
        }
    }

    private IEnumerator RebuildCustomDeck(DecSelector selected)
    {
        for (int i = CustomDeckParent.childCount - 1; i >= 0; i--)
        {
            Destroy(CustomDeckParent.GetChild(i).gameObject);
        }

        yield return new WaitForEndOfFrame();

        while (CustomDeckParent.childCount > 0)
            yield return null;

        foreach (GameObject card in selected.Cards)
        {
            Instantiate(card, CustomDeckParent);
        }
    }

    private void ShowAllCards()
    {
        StartCoroutine(RebuildAllCards());
    }

    private IEnumerator RebuildAllCards()
    {
        for (int i = AllDeckParent.childCount - 1; i >= 0; i--)
        {
            Destroy(AllDeckParent.GetChild(i).gameObject);
        }

        yield return new WaitForEndOfFrame();

        while (AllDeckParent.childCount > 0)
            yield return null;

        foreach (DecSelector dec in Declist)
        {
            foreach (GameObject card in dec.Cards)
            {
                Instantiate(card, AllDeckParent);
            }
        }
    }
}

[System.Serializable] 
public class DecSelector
{
    public GameObject _Checked;
    public GameObject InActiveObj;
    public List<GameObject> Cards = new List<GameObject>();
}
