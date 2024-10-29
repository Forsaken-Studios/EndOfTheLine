using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutDetails : MonoBehaviour
{

    private ShortcutSO shortcutSO;
    [SerializeField] private Image shortCutIcon;
    [SerializeField] private TextMeshProUGUI shortCutText;
    
    public void SetUpProperties(ShortcutSO info)
    {
        shortcutSO = info;
        this.shortCutIcon.sprite = info.shortCut.shortCutImage;
        this.shortCutText.text = info.shortCut.shortCutText;
    }

    public bool GetIfIsLoot()
    {
        return shortcutSO.shortCut.shortCutText.Contains("Loot");
    }
}
