using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityShop : MonoBehaviour
{

    public static AbilityShop Instance; 
    [SerializeField] private GameObject abilityPanelPrefab;
    private GameObject abilityGrid;
    private List<GameObject> abilitiesPanelList;
    private AbilityPanel currentabilityPanelSelected;
    private Ability currentAbilitySelected;
    [Header("Ability Details")]
    [SerializeField] private GameObject detailsView;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private TextMeshProUGUI abilityCooldown;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI abilityGoldCost;
    [SerializeField] private TextMeshProUGUI abilityMaterial2Cost;
    [SerializeField] private TextMeshProUGUI abilityMaterial3Cost;
    [SerializeField] private Button buyAbilityButton;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("[AbilityShop.cs] : THERE IS ALREADY A ABILITY SHOP");
            Destroy(this);
        }

        Instance = this;
    }

    private void OnEnable()
    {
        abilitiesPanelList = new List<GameObject>();
        SetUpAbilitiesInShop();
        detailsView.SetActive(false);
    }

    public void ShowAbilityDetails(Ability ability, bool isUnlocked, AbilityPanel abilityPanel)
    { 
        currentAbilitySelected = ability;
        if (currentabilityPanelSelected != null)
        {
            currentabilityPanelSelected.HideBlackPanel();
            currentabilityPanelSelected.SetIsSelected(false);
            currentabilityPanelSelected.ResetColor();
        }
            
        currentabilityPanelSelected = abilityPanel;
        abilityPanel.HideBlackPanel();
       detailsView.SetActive(true);
       abilityName.text = ability.name;
       abilityDescription.text = ability.description;
       abilityIcon.sprite = ability.abilityIcon;
       abilityCooldown.text = ability.cooldownTime.ToString();
       abilityGoldCost.text = ability.airFilterNeededToUnlock.ToString();
       abilityMaterial2Cost.text = ability.material1NeededToUnlock.ToString();
       abilityMaterial3Cost.text = ability.material2NeededToUnlock.ToString();
       buyAbilityButton.gameObject.SetActive(!isUnlocked);
       if (!isUnlocked)
       {
           buyAbilityButton.onClick.AddListener(() => BuyAbilityButton());
       }
    }


    private void BuyAbilityButton()
    {
        
        if (TrainManager.Instance.resourceAirFilter >= currentAbilitySelected.airFilterNeededToUnlock)
        {
            bool haveRedMineral =
                TrainBaseInventory.Instance.GetIfItemIsInInventory(currentAbilitySelected.material1Item,
                    currentAbilitySelected.material1NeededToUnlock);
            bool havePurpleMineral =
                TrainBaseInventory.Instance.GetIfItemIsInInventory(currentAbilitySelected.material2Item,
                    currentAbilitySelected.material2NeededToUnlock);
            if (haveRedMineral && havePurpleMineral)
            {
                TrainManager.Instance.resourceAirFilter -= currentAbilitySelected.airFilterNeededToUnlock;
                TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(currentAbilitySelected.material1Item, currentAbilitySelected.material1NeededToUnlock);
                TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(currentAbilitySelected.material2Item, currentAbilitySelected.material2NeededToUnlock);
                PlayerPrefs.SetInt("AbilityUnlocked_" + currentAbilitySelected.abilityID, 1);
                buyAbilityButton.gameObject.SetActive(false);
            }
            else
            {
                buyAbilityButton.gameObject.GetComponent<Animator>().SetTrigger("Shake"); 
            }
        }
        else
        {
            buyAbilityButton.gameObject.GetComponent<Animator>().SetTrigger("Shake"); 
        }
    }
    
    private void OnDisable()
    {
        detailsView.SetActive(false);
        foreach (var panel in abilitiesPanelList)
        {
            Destroy(panel);
        }
        
        abilitiesPanelList.Clear();
    }

    private void SetUpAbilitiesInShop()
    {
        List<Ability> abilityList = UnityEngine.Resources.LoadAll<Ability>("Abilities").ToList();
        
        foreach (var ability in abilityList)
        {
            GameObject abilityPanel = Instantiate(abilityPanelPrefab, Vector2.zero, Quaternion.identity,
                this.gameObject.transform);

            int abilityStatus = PlayerPrefs.GetInt("AbilityUnlocked_" + ability.abilityID);
            abilitiesPanelList.Add(abilityPanel);
            abilityPanel.GetComponent<AbilityPanel>().SetUpProperties(ability, abilityStatus);
        }
    }
}
