using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{

    [SerializeField] private int slotID;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI abilityText;
    [SerializeField] private Sprite emptySprite;

    public void UpdateSlot(Ability ability)
    {
        this.abilityIcon.sprite = ability.abilityIcon;
        this.abilityText.text = ability.name.ToString();
    }

    public void ClearSlot()
    {
        this.abilityIcon.sprite = emptySprite;
        this.abilityText.text = "";
    }
}
