using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{       
    private GameObject currentHotkeyGameObject;

    private LoreSO lore;
    private bool isAlreadyLooted = false;
    private bool _isLooteable = false;
    public bool IsLooteable
    {
        get { return _isLooteable; }
        set { _isLooteable = value; }
    }
    
    private void Update()
    {
        //TODO: Habría que ver si habría mas lores disponibles, para no repetir.
        //Por ahora se elige en sucesión
        if (_isLooteable)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //Activate lore pickup 
                PrepareLore();
                GameObject pickupLoreText = Instantiate(LoreManager.Instance.GetPickUpLorePrefab(), 
                    new Vector2(0, 0), Quaternion.identity);
                pickupLoreText.GetComponentInChildren<PickupLoreText>().SetLoreSO(this.lore);
                DesactivateKeyHotkeyImage();
                isAlreadyLooted = true;
                UpdateLoreIndex();
            }
        }
    }

    private void PrepareLore()
    {
        int playerLore = PlayerPrefs.GetInt("CurrentLoreIndex");
        UnityEngine.Object loreObject = UnityEngine.Resources.Load("Lore/Lore " + playerLore);
        LoreSO loreSO = loreObject as LoreSO;
        lore = loreSO;
        Debug.Log("SE HA COLOCADO EL LORE [ " + playerLore + " ]");
    }

    private void UpdateLoreIndex()
    {
        int playerLore = PlayerPrefs.GetInt("CurrentLoreIndex");
        if (playerLore >= LoreManager.Instance.GetCurrentNumberOfLorePages())
            PlayerPrefs.SetInt("CurrentLoreIndex", 1); //Por ahora resetamos, luego ya veremos
        else
            PlayerPrefs.SetInt("CurrentLoreIndex", playerLore + 1);
        
    }

    public void ActivateKeyHotkeyImage()
    {
        currentHotkeyGameObject = Instantiate(LootUIManager.Instance.GetHotkeyPrefab(),
            new Vector2(this.transform.position.x, this.transform.position.y + 1), Quaternion.identity); 
        _isLooteable = true;
    }

    public void DesactivateKeyHotkeyImage()
    {
        Destroy(currentHotkeyGameObject);
        currentHotkeyGameObject = null;
        _isLooteable = false;
    }

    public bool GetIfItIsAlreadyLooted()
    {
        return isAlreadyLooted;
    }
}
