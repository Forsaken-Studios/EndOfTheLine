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
        GameObject newShortcut = Instantiate(shortCutPrefab, new Vector3(0, 0, 0), Quaternion.identity,
            this.gameObject.transform);

        newShortcut.GetComponent<ShortcutDetails>().SetUpProperties(shortcutsDictionary[shortcutType]);
        return newShortcut;
    }

    public void RemoveShortcut(GameObject prefab)
    {
        Destroy(prefab);
    }
    
}
