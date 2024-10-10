using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UpgradeShop : MonoBehaviour
{
    public static UpgradeShop Instance; 
    [FormerlySerializedAs("abilityPanelPrefab")] [SerializeField] private GameObject upgradePanelPrefab;
    private GameObject upgradeGrid;
    private List<GameObject> upgradePanelList;
    private UpgradeSlotPanel currentUpgradePanelSelected;
    private UpgradesSO currentUpgradeSelected;
    [Header("Ability Details")]
    [SerializeField] private GameObject detailsView;
    [FormerlySerializedAs("abilityName")] [SerializeField] private TextMeshProUGUI upgradeName;
    [FormerlySerializedAs("abilityDescription")] [SerializeField] private TextMeshProUGUI upgradeDescription; 
    [SerializeField] private TextMeshProUGUI purpleMineralUpgradeMaterialText;
    [SerializeField] private TextMeshProUGUI airFilterUpgradeMaterialText;
    [SerializeField] private TextMeshProUGUI redMineralUpgradeMaterialText;
    [FormerlySerializedAs("buyAbilityButton")] [SerializeField] private Button buyUpgradeButton;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("[UpgradeShop.cs] : THERE IS ALREADY A UpgradeShop");
            Destroy(this);
        }

        Instance = this;
    }

    private void OnEnable()
    {
        upgradePanelList = new List<GameObject>();
        SetUpAbilitiesInShop();
        detailsView.SetActive(false);
    }

    public void ShowAbilityDetails(UpgradesSO upgrade, bool isUnlocked, UpgradeSlotPanel panel)
    { 
        currentUpgradeSelected = upgrade;
        if (currentUpgradePanelSelected != null)
        {
            currentUpgradePanelSelected.ShowBlackPanel(); 
            currentUpgradePanelSelected.SetIsSelected(false);
            currentUpgradePanelSelected.ResetColor();
        }
        currentUpgradePanelSelected = panel;
        panel.HideBlackPanel();
        detailsView.SetActive(true);
        upgradeName.text = upgrade.name;
        upgradeDescription.text = upgrade.description;
        purpleMineralUpgradeMaterialText.text = upgrade.purpleMineralCost.ToString();
        airFilterUpgradeMaterialText.text = upgrade.airFilterCost.ToString();
        redMineralUpgradeMaterialText.text = upgrade.redMineralCost.ToString();
        buyUpgradeButton.gameObject.SetActive(!isUnlocked);
        if (!isUnlocked)
        {
        buyUpgradeButton.onClick.AddListener(() => BuyUpgradeButton());
        }
    }


    private void BuyUpgradeButton()
    {
        if (TrainManager.Instance.resourceAirFilter >= currentUpgradeSelected.airFilterCost)
        {
            bool haveRedMineral =
                TrainBaseInventory.Instance.GetIfItemIsInInventory(currentUpgradeSelected.redMineralItem,
                    currentUpgradeSelected.redMineralCost);
            bool havePurpleMineral =
                TrainBaseInventory.Instance.GetIfItemIsInInventory(currentUpgradeSelected.purpleMineralItem,
                    currentUpgradeSelected.purpleMineralCost);
            if (haveRedMineral && havePurpleMineral)
            {
                TrainManager.Instance.resourceAirFilter -= currentUpgradeSelected.airFilterCost;
                TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(currentUpgradeSelected.redMineralItem, currentUpgradeSelected.redMineralCost);
                TrainBaseInventory.Instance.FindAndDeleteItemsFromItemSlot(currentUpgradeSelected.purpleMineralItem, currentUpgradeSelected.purpleMineralCost);
                PlayerPrefs.SetInt("UpgradeUnlocked_" + currentUpgradeSelected.upgradeID, 1);
                currentUpgradePanelSelected.SetIsUnlocked(true);
                buyUpgradeButton.gameObject.SetActive(false); 
                currentUpgradePanelSelected.HideBlackPanel();
            }
            else
            {
               buyUpgradeButton.gameObject.GetComponent<Animator>().SetTrigger("Shake"); 
            }
        }
        else
        {
                buyUpgradeButton.gameObject.GetComponent<Animator>().SetTrigger("Shake"); 
        }
    }
    
    private void OnDisable()
    {
        detailsView.SetActive(false);
        foreach (var panel in upgradePanelList)
        {
            Destroy(panel);
        }
        
        upgradePanelList.Clear();
    }

    private void SetUpAbilitiesInShop()
    {
        List<UpgradesSO> upgradesList = UnityEngine.Resources.LoadAll<UpgradesSO>("PassiveUpgrades").ToList();
        
        foreach (var upgrades in upgradesList)
        {
            GameObject upgradePanel = Instantiate(upgradePanelPrefab, Vector2.zero, Quaternion.identity,
                this.gameObject.transform);

            int upgradeStatus = PlayerPrefs.GetInt("UpgradeUnlocked_" + upgrades.upgradeID);
            upgradePanelList.Add(upgradePanel);
            upgradePanel.GetComponent<UpgradeSlotPanel>().SetUpProperties(upgrades, upgradeStatus);
        }
    }
}
