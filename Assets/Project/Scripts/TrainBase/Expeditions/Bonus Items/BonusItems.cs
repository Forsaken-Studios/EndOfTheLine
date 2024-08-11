using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BonusItems : MonoBehaviour
{

    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemBonusButton;
    [SerializeField] private GameObject blackPanel;

    private MissionBonusItemsSO missionBonusItem;
    
    public static event EventHandler<EventBonusItemInfo> onButtonClicked ;
    
    private bool isActivated = false;
    public void SetUpProperties(MissionBonusItemsSO missionBonus)
    {
        this.itemImage.sprite = missionBonus.item.itemIcon;
        missionBonusItem = missionBonus;
    }

    private void OnEnable()
    {
        itemBonusButton.onClick.AddListener(() => HandleBonusItem());
    }

    private void HandleBonusItem()
    {
        if (TrainBaseInventory.Instance.GetIfItemIsInInventory(missionBonusItem.item, 1)
            || PlayerInventory.Instance.GetIfItemIsInPlayerInventory(missionBonusItem.item, 1))
        {
            blackPanel.SetActive(isActivated);
            isActivated = !isActivated;
            EventBonusItemInfo eventInfo = new EventBonusItemInfo(isActivated, missionBonusItem);
            onButtonClicked?.Invoke(this, eventInfo);
        }
    }

    public MissionBonusItemsSO GetMissionBonusItem()
    {
        return missionBonusItem;
    }

    private void OnDisable()
    {
        itemBonusButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        itemBonusButton.onClick.RemoveAllListeners();
    }
}


public class EventBonusItemInfo
{
    public bool isActivated;
    public MissionBonusItemsSO missionBonus;

    public EventBonusItemInfo(bool isActivated, MissionBonusItemsSO mission)
    {
        this.isActivated = isActivated;
        this.missionBonus = mission;
    }
}
