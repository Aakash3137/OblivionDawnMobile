using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer foodSR;
    [SerializeField] private SpriteRenderer goldSR;
    [SerializeField] private SpriteRenderer metalSR;
    [SerializeField] private SpriteRenderer powerSR;

    [SerializeField] private List<Image> foodIcons;
    [SerializeField] private List<Image> goldIcons;
    [SerializeField] private List<Image> metalIcons;
    [SerializeField] private List<Image> powerIcons;
}
