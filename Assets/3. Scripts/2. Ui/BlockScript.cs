using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockScript : MonoBehaviour
{
    [SerializeField] internal Image IconImage;
    [SerializeField] internal TMP_Text TitleText;
    [SerializeField] internal TMP_Text CurrentValueText;
    [SerializeField] internal TMP_Text IncreaseByText;
    [SerializeField] internal bool Increasable;
    [SerializeField] internal BlockData blockData;
}

[System.Serializable]
public class BlockData
{
    public float CurrentValue;
    public float IncreaseByValue;
}
