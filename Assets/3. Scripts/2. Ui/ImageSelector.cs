using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageSelector : MonoBehaviour
{
    internal Image TargetImage;
    [SerializeField] internal Image CharacterImage; 
    [SerializeField] internal Image tem_Selected;

    [SerializeField] internal ProfileManager _ProfileManager;

    public void SelectProfileImage()
    {
        foreach (ImageSelector data in _ProfileManager.profileEditData.CharacterImages)
        {
            data.tem_Selected.gameObject.SetActive(false);
        }
        tem_Selected.gameObject.SetActive(true);
        TargetImage.sprite = CharacterImage.sprite;
        _ProfileManager.profileEditData.TargetImage = CharacterImage.sprite;
    }
}
