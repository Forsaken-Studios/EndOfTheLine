using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TryingToRaidWithResources : MonoBehaviour
{

    [SerializeField] private Button cancelButton;
    [FormerlySerializedAs("letAllItemsInBaseButton")] [SerializeField] private Button stashAllItemsInBaseButton;
    // Start is called before the first frame update
    void Start()
    {
       cancelButton.onClick.AddListener(() => CancelRaid()); 
       stashAllItemsInBaseButton.onClick.AddListener(() => LetAllItemsInBase()); 
    }
    

    private void LetAllItemsInBase()
    {
        PlayerInventory.Instance.StashAllItemsInBase();
        SaveManager.Instance.SaveGame();
        //Aqui ya tendríamos que hacer la generación o lo que sea, por ahora iniciamos test level
        SceneManager.LoadSceneAsync("1");
    }

    private void CancelRaid()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onMissionSelector;
        GameObject parent = this.gameObject.transform.parent.gameObject;
        TrainManager.Instance.GetMissionSelector().MakePlayButtonInteractable();
        Destroy(this.gameObject);
        Destroy(parent);
    }
    
    
    
    private void OnDestroy()
    {
        cancelButton.onClick.RemoveAllListeners();
        stashAllItemsInBaseButton.onClick.RemoveAllListeners();
    }
}
