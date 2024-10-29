using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.CustomLogs;

public enum ShortcutType
{
    undefined,
    openInventory,
    lootCrate,
    placeAbility,
    useAbilityE,
    useAbilityQ,
    cancelAbility,
    
}

public class ShortcutsUIManager : MonoBehaviour
{
    public static ShortcutsUIManager Instance;

    [SerializeField] private GameObject shortCutPrefab;
    private Dictionary<ShortcutType, ShortcutSO> shortcutsDictionary;
    private List<ShortcutDetails> shortcutDetailsList;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("[ShortcutsUIManager.cs] : There is already a shortcutsManager");
            Destroy(this);
        }

        Instance = this;
    }

    private void Start()
    {
        shortcutsDictionary = new Dictionary<ShortcutType, ShortcutSO>();
        shortcutDetailsList = new List<ShortcutDetails>();
        Loadshortcuts();
    }

    private void Loadshortcuts()
    {
        List<ShortcutSO> allSpecificItems = UnityEngine.Resources.LoadAll<ShortcutSO>("Shortcuts").ToList();
        foreach (var shortCut in allSpecificItems)
        {
            ShortcutType shortcutType;
            if (Enum.IsDefined(typeof(ShortcutType), shortCut.shortCut.shortCutEnum))
            {
                shortcutType = (ShortcutType)(Enum.Parse(typeof(ShortcutType), shortCut.shortCut.shortCutEnum));  
            }
            else
            {
                shortcutType = ShortcutType.undefined; 
            }
            shortcutsDictionary.Add(shortcutType, shortCut);
        }
    }


    public GameObject AddShortcuts(ShortcutType shortcutType)
    {
        if (shortcutDetailsList.Find(t => t.GetIfIsLoot()) && shortcutType == ShortcutType.lootCrate)
        {
            return null;
        }

        GameObject newShortcut = Instantiate(shortCutPrefab, new Vector3(0, 0, 0), Quaternion.identity,
            this.gameObject.transform);
        ShortcutDetails details = newShortcut.GetComponent<ShortcutDetails>();
        details.SetUpProperties(shortcutsDictionary[shortcutType]);
        shortcutDetailsList.Add(details);
        return newShortcut;
    }

    public void RemoveShortcut(GameObject prefab)
    {
        shortcutDetailsList.Remove(prefab.GetComponent<ShortcutDetails>());
        Destroy(prefab);
    }
    
}
