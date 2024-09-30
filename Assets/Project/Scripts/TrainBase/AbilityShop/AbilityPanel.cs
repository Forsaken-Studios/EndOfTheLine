using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    
    [SerializeField] private Image abilityIcon;
    [SerializeField] private GameObject blackPanel;
    private bool unlocked;
    private Button button;
    private bool isSelected = false;
    
    private Ability ability;
    
    public void SetUpProperties(Ability ability, int abilityStatus)
    {
        button = GetComponentInChildren<Button>();
        this.ability = ability;
        unlocked = abilityStatus == 1;
        this.abilityIcon.sprite = ability.abilityIcon;
        blackPanel.SetActive(abilityStatus == 0);
        
        button.onClick.AddListener(() => ShowAbilityDetails());
    }


    private void ShowAbilityDetails()
    {     
        isSelected = true;
        button.GetComponent<Image>().color = Color.blue;
        AbilityShop.Instance.ShowAbilityDetails(this.ability, unlocked, this);   
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
        if (!unlocked && !isSelected)
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
    
    public void ShowBlackPanel()
    {
        if (!unlocked)
        {
            blackPanel.SetActive(true);
        }
    }

    public void ResetColor()
    {
        button.GetComponent<Image>().color = Color.white;
    }
    
    public void SetIsSelected(bool aux)
    {
        this.isSelected = aux;
    }
}
