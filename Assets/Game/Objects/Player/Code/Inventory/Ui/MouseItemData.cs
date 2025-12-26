using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI itemCount;

    void Awake()
    {
        ItemSprite.color = Color.clear;
        itemCount.text = null;
    }
}
