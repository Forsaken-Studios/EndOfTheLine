using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionFailedPanel : MonoBehaviour
{
    private Button continueButton;

    private void OnEnable()
    {
        continueButton = GetComponentInChildren<Button>();
        continueButton.onClick.AddListener(() => ContinueButtonBehavior());
    }
    
    private void ContinueButtonBehavior()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onExpeditionRoom;
        PlayerPrefs.SetInt("ExpeditionInProgress", 0);
        PlayerPrefs.SetInt("ExpeditionEndDay", 0);
        NewExpeditionManager.Instance.UpdateMissionPanelStatusToDisable();
        NewExpeditionManager.Instance.ResetDetailsView();
        Destroy(this.transform.parent.gameObject);
    }
}
