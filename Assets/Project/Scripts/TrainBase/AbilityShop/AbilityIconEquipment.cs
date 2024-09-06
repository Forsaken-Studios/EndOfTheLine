using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityIconEquipment : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
        
    [SerializeField] private Image abilityIcon;
    [SerializeField] private GameObject blackPanel;
    private bool unlocked;
    private Button button;

    private Ability ability;
    
    public void SetUpProperties(Ability ability, int abilityStatus)
    {
        button = GetComponentInChildren<Button>();
        this.ability = ability;
        unlocked = abilityStatus == 1;
        this.abilityIcon.sprite = ability.abilityIcon;
        blackPanel.SetActive(abilityStatus == 0);

    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!unlocked)
        {
            blackPanel.SetActive(false);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    { 
        if (!unlocked)
        {
            blackPanel.SetActive(true);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        this.blackPanel.SetActive(false);
    }

    public void HideBlackPanel()
    {
        blackPanel.SetActive(false);
    }
}
