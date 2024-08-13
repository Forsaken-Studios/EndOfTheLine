using System;
using System.Collections;
using System.Collections.Generic;
using Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionTypeChooser : MonoBehaviour
{
    [Header("Mission Type")]
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI missionDaysToCompleteText;
    [Header("Expedition Properties Text")]
    [SerializeField] private TextMeshProUGUI chanceOfSuccessText;
    private float rewardsMultiplier = 1;
    [Header("Requirements Needed")] 
    [SerializeField] private GameObject requirementPrefab;
    [SerializeField] private GameObject requirementGrid; 
    private List<GameObject> listOfRequirements;
    private List<RequirementSO> listOfRequirementsSO;
    [Header("Rewards Needed")] 
    [SerializeField] private GameObject rewardsPrefab;
    [SerializeField] private GameObject rewardsGrid;
    private List<GameObject> listOfRewards;
    private List<ExpeditionRewardSO> listOfRewardsSO;
    [Header("Bonuses Items Needed")] 
    [SerializeField] private GameObject bonusesItemsPrefab;
    [SerializeField] private GameObject bonusesItemsGrid;
    private List<GameObject> listOfBonusesItems;
    private float currentChanceOfSuccess;
    private int currentMissionIndex = 0;
    private List<MissionStatSO> missions;
    private MissionStatSO currentMissionSelected;
    [Header("Start Expedition Bonus")]
    [SerializeField] private Button startExpeditionButton;
    private bool canStartExpedition = true;
    private bool expeditionInProgress = false;


    private void OnEnable()
    {
        //PlayerPrefs.SetInt("ExpeditionInProgress", 0);
        InitializeList();
        InitializeButtons();
        SetUpProperties(0);
        this.leftArrowButton.interactable = false;
        UpdateChanceOfSuccess(missions[currentMissionIndex].basicChanceOfSuccess);
        //Event
        BonusItems.onButtonClicked += OnBonusItemClicked;
        ItemsToHelpExpedition.onToolsIncreaseChanged += OnToolsIncreaseChanged;
        ItemsToHelpExpedition.onToolsDecreaseChanged += OnToolsDecreaseChanged;
        
        this.expeditionInProgress = PlayerPrefs.GetInt("ExpeditionInProgress") == 1;
        
        if (expeditionInProgress)
        {
            startExpeditionButton.interactable = false;
            Debug.Log("EE");
        }
        else
        {
            startExpeditionButton.interactable = true; 
        }
    }

    private void OnToolsDecreaseChanged(object sender, int e)
    {
       RemoveChanceOfSuccess(e);
    }

    private void OnToolsIncreaseChanged(object sender, int e)
    {
        AddChanceOfSuccess(e);
    }


    private void InitializeList()
    {
        listOfRequirements = new List<GameObject>();
        listOfRewardsSO = new List<ExpeditionRewardSO>();
        listOfRewards = new List<GameObject>();
        listOfBonusesItems = new List<GameObject>();
        listOfRequirementsSO = new List<RequirementSO>();
        missions = new List<MissionStatSO>();
        missions = ExpeditionManager.Instance.GetExpeditionClicked().GetMissions();
    }

    private void InitializeButtons()
    {
        startExpeditionButton.onClick.AddListener(() => StartExpedition());
        leftArrowButton.onClick.AddListener(() => SwapMissionToLeft());
        rightArrowButton.onClick.AddListener(() => SwapMissionToRight());
    }


    private void StartExpedition()
    {
        float randomValue = UnityEngine.Random.Range(0, 101);

        if (randomValue < currentChanceOfSuccess)
        {
            Debug.Log("SUCCESS");
            PlayerPrefs.SetInt("ExpeditionResult", 1);
            PlayerPrefs.SetInt("ExpeditionInProgress", 1);
            startExpeditionButton.interactable = false;
            //Success expedition
            SaveRewardsInJson();
        }
        else
        {
            //Failure expedition
            PlayerPrefs.SetInt("ExpeditionResult", -1);
            PlayerPrefs.SetInt("ExpeditionInProgress", 1);
            startExpeditionButton.interactable = false;
            Debug.Log("FAILURE");
            // -1 => Failure
        }

        int endingDay = PlayerPrefs.GetInt("CurrentDay") + currentMissionSelected.daysToComplete;
        PlayerPrefs.SetInt("ExpeditionEndDay", endingDay);
    }

    private void SaveRewardsInJson()
    {
        Dictionary<int, int> rewardsDictionary = new Dictionary<int, int>();
            
        foreach (var reward in listOfRewardsSO)
        {
            //We use this script as we use the same info
            int rewardAmount = UnityEngine.Random.Range(reward.minAmount, reward.maxAmount + 1);
            rewardsDictionary.Add(reward.item.itemID, rewardAmount);
        }
        DataPlayerInventory rewardsSave = new DataPlayerInventory(rewardsDictionary);
        SaveManager.Instance.SaveExpeditionRewardJson(rewardsSave);
    }
    
    private void OnDisable()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        startExpeditionButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
        BonusItems.onButtonClicked -= OnBonusItemClicked;
        ItemsToHelpExpedition.onToolsIncreaseChanged -= OnToolsIncreaseChanged;
        ItemsToHelpExpedition.onToolsDecreaseChanged -= OnToolsDecreaseChanged;
    }
    
    private void SetUpProperties(int index)
    {
        UpdateChanceOfSuccess(missions[index].basicChanceOfSuccess);
       // Debug.Log(missions.Count);
        this.missionText.text = missions[index].missionName;
        this.missionDaysToCompleteText.text = missions[index].daysToComplete.ToString() + " days";
        currentMissionSelected = missions[index];
        //Set up requirements if needed
        SetUpRequirements();
        //Check if we meet the requirements
        if(!expeditionInProgress)
            CheckIfWeMeetUpRequirements();
        //Change rewards
        ChangeRewardsGrid();
        //Set Up bonuses items if needed
        SetUpBonusesItems();
    }

    private void OnBonusItemClicked(object sender, EventBonusItemInfo e)
    {
        if (e.isActivated)
        {
            //Increase reward
            rewardsMultiplier += e.missionBonus.itemsRewardsMultiplier;
        }
        else
        {
            rewardsMultiplier -= e.missionBonus.itemsRewardsMultiplier;
        }
        //Change rewards text
        UpdateRewardsText(rewardsMultiplier);
    }

    private void UpdateRewardsText(float multiplier)
    {
        foreach (var reward  in listOfRewards)
        {
            reward.GetComponent<RewardIcon>().WriteNewRewards(multiplier);
        }
    }

    private void SetUpBonusesItems()
    {
        ClearList(listOfBonusesItems);   
       /* string resourcesPath = "Expedition/MissionEXP" + ExpeditionManager.Instance.GetStationSelected().stationID + "/EXP" + 
                               ExpeditionManager.Instance.GetStationSelected().stationID + "_MIS"  + (currentMissionIndex + 1) + "_BonusItems";
        MissionBonusItemsSO[] missionBonusItems =
            UnityEngine.Resources.LoadAll<MissionBonusItemsSO>(resourcesPath);*/
       
       
       List<MissionBonusItems> missionBonusItems = currentMissionSelected.bonusItems;
       
       
        if (missionBonusItems.Count > 0)
        {
            foreach (var bonusItem in missionBonusItems)
            {
                GameObject bonusItemsGameObject = Instantiate(bonusesItemsPrefab, Vector2.zero, Quaternion.identity, bonusesItemsGrid.transform);
                listOfBonusesItems.Add(bonusItemsGameObject);
                bonusItemsGameObject.GetComponent<BonusItems>().SetUpProperties(bonusItem.bonusItems); 
            }
        }
    }

    private void ChangeRewardsGrid()
    {
        ClearList(listOfRewards);   
        listOfRewardsSO.Clear();
        /*string resourcesPath = "Expedition/MissionEXP" + ExpeditionManager.Instance.GetStationSelected().stationID + "/EXP" + 
                               ExpeditionManager.Instance.GetStationSelected().stationID + "_MIS"  + (currentMissionIndex + 1) + "_Rewards";
        ExpeditionRewardSO[] missionRewards =
            UnityEngine.Resources.LoadAll<ExpeditionRewardSO>(resourcesPath);*/
        
        List<MissionRewards> missionRewards = currentMissionSelected.rewards;

        if (missionRewards.Count > 0)
        {
            foreach (var reward in missionRewards)
            {
                GameObject rewardGameObject = Instantiate(rewardsPrefab, Vector2.zero, Quaternion.identity, rewardsGrid.transform);
                listOfRewards.Add(rewardGameObject);
                listOfRewardsSO.Add(reward.reward);
                rewardGameObject.GetComponent<RewardIcon>().SetUpProperties(reward.reward); 
            }
        }
    }

    private void CheckIfWeMeetUpRequirements()
    {
        if (listOfRequirementsSO.Count > 0)
        {
            foreach (var requirement in listOfRequirementsSO)
            {
                //Check in inventory & playerInventory
                if (TrainBaseInventory.Instance.GetIfItemIsInInventory(requirement.item, requirement.amountNeeded) ||
                    PlayerInventory.Instance.GetIfItemIsInPlayerInventory(requirement.item, requirement.amountNeeded))
                {
                    startExpeditionButton.interactable = true;
                }
                else
                { 
                    startExpeditionButton.interactable = false;
                    break; 
                }
            }   
        }else
            startExpeditionButton.interactable = true;
    }

    private bool GetIfWeHaveItem(Item item, int amount)
    {
        return TrainBaseInventory.Instance.GetIfItemIsInInventory(item, amount) ||
               PlayerInventory.Instance.GetIfItemIsInPlayerInventory(item, amount);
    }

    private void SetUpRequirements()
    {
        ClearList(listOfRequirements);
        listOfRequirementsSO.Clear();
        
        /*
        string resourcesPath = "Expedition/MissionEXP" + ExpeditionManager.Instance.GetStationSelected().stationID + "/EXP" + 
                               ExpeditionManager.Instance.GetStationSelected().stationID + "_MIS"  + (currentMissionIndex + 1) + "_Requirements";
        RequirementSO[] missionRequirements =
            UnityEngine.Resources.LoadAll<RequirementSO>(resourcesPath);*/
        
        List<Requirements> missionRequirements = currentMissionSelected.requirements;
 

        if (missionRequirements.Count > 0)
        {
            foreach (var requirement in missionRequirements)
            {
                GameObject requirementGameObject = Instantiate(requirementPrefab, Vector2.zero, Quaternion.identity, requirementGrid.transform);
                listOfRequirements.Add(requirementGameObject);
                listOfRequirementsSO.Add(requirement.requirement);
                requirementGameObject.GetComponent<Requirement>().SetUpProperties(requirement.requirement, 
                    GetIfWeHaveItem(requirement.requirement.item, requirement.requirement.amountNeeded));
            }
        }
        else
        {
            //MESSAGE OF NO REQUIERMENT NEEDED
        }
    }

    private void ClearList(List<GameObject> list)
    {
        foreach (var aux in list)
        {
            Destroy(aux);
        }
        list.Clear();
 
    }
    
    private void SwapMissionToLeft()
    {
        int newIndex = currentMissionIndex - 1;
        currentMissionIndex = (int) Mathf.Clamp(newIndex, 0, this.missions.Count - 1);
        SetUpProperties(currentMissionIndex);
        BlockButtonIfFirstMission(currentMissionIndex);
    }
    
    private void SwapMissionToRight()
    {
        int newIndex = currentMissionIndex + 1;
        currentMissionIndex = Mathf.Clamp(newIndex, 0, this.missions.Count - 1);
        SetUpProperties(currentMissionIndex);
        BlockButtonIfLastMission(currentMissionIndex);
    }

    private void UpdateChanceOfSuccess(float value )
    {
        currentChanceOfSuccess = value;
        chanceOfSuccessText.text = value.ToString() + " %";
    }
    
    public void AddChanceOfSuccess(float value)
    {
        currentChanceOfSuccess += value;
        chanceOfSuccessText.text = currentChanceOfSuccess.ToString() + " %";
    }
    public void RemoveChanceOfSuccess(float value )
    {
        currentChanceOfSuccess -= value;
        Debug.Log(currentChanceOfSuccess);
        chanceOfSuccessText.text = currentChanceOfSuccess.ToString() + " %";
    }
    private void BlockButtonIfFirstMission(int index)
    {
        if (index == 0)
            this.leftArrowButton.interactable = false;
        
        if (index < missions.Count - 1)
            this.rightArrowButton.interactable = true;
    }
    private void BlockButtonIfLastMission(int index)
    {
        if (index > 0)
            this.leftArrowButton.interactable = true;
        
        if (index == missions.Count - 1)
            this.rightArrowButton.interactable = false;
    }
}
