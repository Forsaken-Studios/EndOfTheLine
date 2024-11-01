using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionName;
    [SerializeField] private TextMeshProUGUI missionDuration;
    [SerializeField] private GameObject inProgressText;
    private MissionStatSO mission;

    [SerializeField] private GameObject timeRemainingPanel;
    [SerializeField] private Image imageFiller;
    
    [SerializeField] private GameObject rewardsGrid;
    [SerializeField] private GameObject rewardPrefab;
    private ExpeditionRewardSO expeditionReward;
    public void SetUpProperties(MissionStatSO mission, int expeditionInProgressID)
    {
        this.missionName.text = mission.missionName;
        this.missionDuration.text = mission.daysToComplete.ToString() + " days";
        this.mission = mission;
        List<MissionRewards> missionRewards = mission.rewards;

        if (expeditionInProgressID == this.mission.id)
        {
            inProgressText.SetActive(true);
            timeRemainingPanel.SetActive(true);

            HandleProgressBar();

        }
        else
        {
            inProgressText.SetActive(false);
            timeRemainingPanel.SetActive(false);
        }
        
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => HandleClickInExpeditionPanel());
        if (missionRewards.Count > 0)
        {
            foreach (var reward in missionRewards)
            {
                GameObject rewardGameObject = Instantiate(rewardPrefab, Vector2.zero, Quaternion.identity, rewardsGrid.transform);
                rewardGameObject.GetComponent<RewardIcon>().SetUpProperties(reward.reward, false);
            }
        }
    }

    private void HandleProgressBar()
    {
        float currentDayLocal = PlayerPrefs.GetInt("CurrentDay");
        float endingDay = PlayerPrefs.GetInt("ExpeditionEndDay");
        float remainingDays = (endingDay - currentDayLocal) / mission.daysToComplete;
        if (remainingDays < 0)
        {
            imageFiller.fillAmount = 1;
        }
        else
        {
            imageFiller.fillAmount = 1 - remainingDays;
        }
    }

    public void SetUpExpeditionTextToInProgress()
    {
        inProgressText.SetActive(true);
        inProgressText.GetComponentInChildren<TextMeshProUGUI>().text = "-InProgress-";
        HandleProgressBar();
    }


    public void HideExpeditionProgress()
    {
        inProgressText.SetActive(false);
        timeRemainingPanel.SetActive(false);
    }

    public void SetUpExpeditionTextToCompleted()
    {
        inProgressText.SetActive(true);
        inProgressText.GetComponentInChildren<TextMeshProUGUI>().text = "-Completed-";
    }
    
    private void HandleClickInExpeditionPanel()
    {
        //Update Details View
        NewExpeditionManager.Instance.ActivateDetailsView(this.mission, this);
    }
}
