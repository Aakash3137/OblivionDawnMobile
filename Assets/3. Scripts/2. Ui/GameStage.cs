using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameStage : MonoBehaviour
{
    public GameSelectionMode GameTypes;
    public FactionButton factionButton;

    public TMP_Text ErrorMessage;

    public Mode MyType;

    private void OnEnable() 
    {
        GameTypes.CurrentType = Mode.None;
    }

    public void OnSelectGame()
    {
        GameTypes.CurrentType = MyType;
    }

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
    }

    private IEnumerator DisableErrortext()
    {
        yield return new WaitForSeconds(5f);
        ErrorMessage.gameObject.SetActive(false);
    } 

}
