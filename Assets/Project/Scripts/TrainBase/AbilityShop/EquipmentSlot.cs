using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IDropHandler
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

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableAbilityIcon draggableAbilityIcon = dropped.GetComponent<DraggableAbilityIcon>();
        Ability ability = draggableAbilityIcon.GetAbility();
        Debug.Log("INTENTO " + ability.name + " [ID: + " + ability.abilityID + "] en slot " + slotID);
        if (!EquipmentScreen.Instance.CheckIfAbilityIsAlreadyEquipped(ability, this.slotID))
        {
            // First number - Ability ID
            // Second number - Slot saved
            int playerPrefSave = int.Parse(ability.abilityID.ToString() + slotID.ToString());
            PlayerPrefs.SetInt("AbilityIDEquipped_" + slotID, playerPrefSave);
            this.UpdateSlot(ability);
        }
    }
}
