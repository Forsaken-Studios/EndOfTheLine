using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShortcutDetails : MonoBehaviour
{

    [SerializeField] private Image shortCutIcon;
    [SerializeField] private TextMeshProUGUI shortCutText;
    
    public void SetUpProperties(ShortcutSO info)
    {
        this.shortCutIcon.sprite = info.shortCut.shortCutImage;
        this.shortCutText.text = info.shortCut.shortCutText;
    }
}
