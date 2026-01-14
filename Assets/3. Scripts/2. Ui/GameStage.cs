using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameStage : MonoBehaviour
{
    public GameSelectionMode GameTypes;
    public FactionButton factionButton;

    public TMP_Text ErrorMessage;

    public Mode MyType;
    public Toggle _Toggle;

    private void OnEnable() 
    {
        GameTypes.CurrentType = MyType;
        Debug.Log($"My Type:{MyType} Chnaged Data: {GameTypes.CurrentType}"); 
    }

    // public void OnSelectGame()
    // {
    //     GameTypes.CurrentType = MyType;
    //     Debug.Log($"My Type:{MyType}"); 
    //      Debug.Log($"Toggle Name: {_Toggle.isOn}");
    // }

    public void OnClickLoadGame()
    {
        if(GameTypes.CurrentType == Mode.None)
        {
            ErrorMessage.text = "Select Game Type";
            ErrorMessage.color = Color.red;
            ErrorMessage.gameObject.SetActive(true);
            StartCoroutine(DisableErrortext());
            return;
        }
        GameTypes.loadGameScene();
        gameObject.SetActive(false);
    }

    private IEnumerator DisableErrortext()
    {
        yield return new WaitForSeconds(5f);
        ErrorMessage.gameObject.SetActive(false);
    } 

}
