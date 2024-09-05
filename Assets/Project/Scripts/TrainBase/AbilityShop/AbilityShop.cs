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
    [Header("Ability Details")]
    [SerializeField] private GameObject detailsView;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private TextMeshProUGUI abilityCooldown;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI abilityGoldCost;
    [SerializeField] private TextMeshProUGUI abilityMaterialCost;

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

    public void ShowAbilityDetails(Ability ability)
    {
       detailsView.SetActive(true);
       abilityName.text = ability.name;
       abilityDescription.text = ability.description;
       abilityIcon.sprite = ability.abilityIcon;
       abilityCooldown.text = ability.cooldownTime.ToString();
       abilityGoldCost.text = ability.goldNeededToUnlock.ToString();
       abilityMaterialCost.text = ability.materialNeededToUnlock.ToString();
       

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
