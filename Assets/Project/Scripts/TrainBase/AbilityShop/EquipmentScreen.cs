using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentScreen : MonoBehaviour
{
    public static EquipmentScreen Instance;
    
    [SerializeField] private GameObject abilityPanelPrefab;
    private GameObject abilityGrid;
    private List<GameObject> abilitiesPanelList;
    private List<Ability> abilitiesEquipped;

    [SerializeField] private EquipmentSlot slot1;
    [SerializeField] private EquipmentSlot slot2;

    private Ability abilityInSlot1;
    private Ability abilityInSlot2;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }

    private void OnEnable()
    {
        //PlayerPrefs.SetInt("AbilityIDEquipped_1", 0);
        abilitiesPanelList = new List<GameObject>();
        abilitiesEquipped = new List<Ability>(2);
        SetUpAbilitiesUnlocked(); 
        SetUpAbilitiesEquipped();
    }
    
        
    private void OnDisable()
    {
        foreach (var panel in abilitiesPanelList)
        {
            Destroy(panel);
        }
        
        abilitiesPanelList.Clear();
    }

    private void SetUpAbilitiesEquipped()
    {
        if(abilityInSlot1!= null)
            slot1.UpdateSlot(abilityInSlot1);
        else
            slot1.ClearSlot();
        //Slot 2 
        if(abilityInSlot2 != null)
            slot2.UpdateSlot(abilityInSlot2);
        else
            slot2.ClearSlot();
    }

    private void SetUpAbilitiesUnlocked()
    {
        List<Ability> abilityList = UnityEngine.Resources.LoadAll<Ability>("Abilities").ToList();
        
        int abilityEquipped1 = PlayerPrefs.GetInt("AbilityIDEquipped_1");
        int abilityEquippedInSlot1 = abilityEquipped1 / 10;
        int ability1Slot = abilityEquipped1 % 10;
        int abilityEquipped2 = PlayerPrefs.GetInt("AbilityIDEquipped_2");
        int abilityEquippedInSlot2 = abilityEquipped2 / 10;
        int ability2Slot = abilityEquipped2 % 10;
        
        
        foreach (var ability in abilityList)
        {
            int abilityStatus = PlayerPrefs.GetInt("AbilityUnlocked_" + ability.abilityID);

            HandleAbilityEquipped(ability, abilityEquippedInSlot1, abilityEquippedInSlot2, ability1Slot, ability2Slot);
            if (abilityStatus == 1)
            {
                GameObject abilityPanel = Instantiate(abilityPanelPrefab, Vector2.zero, Quaternion.identity,
                    this.gameObject.transform);
                abilitiesPanelList.Add(abilityPanel);
                abilityPanel.GetComponent<AbilityIconEquipment>().SetUpProperties(ability, 1);
            }
        }
    }

    private void HandleAbilityEquipped(Ability ability, int abilityEquippedInSlot1, int abilityEquippedInSlot2, 
        int ability1Slot, int ability2Slot)
    {
        if (ability.abilityID == abilityEquippedInSlot1)
        {
            if (ability1Slot == 1)
                abilityInSlot1 = ability;
            else
                abilityInSlot2 = ability;
        }
        if(ability.abilityID == abilityEquippedInSlot2)
        {
            if (ability2Slot == 1)
                abilityInSlot1 = ability;
            else
                abilityInSlot2 = ability;
        }
    }


    public bool CheckIfAbilityIsAlreadyEquipped(int abilityID, int slotUsed)
    {
        if (slotUsed == 1)
        {
            if (abilityInSlot2 != null)
            {
                return abilityInSlot2.abilityID == abilityID;
            }
        }
        else
        {
            if (abilityInSlot1 != null)
            {
                return abilityInSlot1.abilityID == abilityID;
            }
        }

        return false;
    }
}
