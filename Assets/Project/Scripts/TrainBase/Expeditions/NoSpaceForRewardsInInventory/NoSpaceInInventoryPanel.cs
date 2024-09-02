using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoSpaceInInventoryPanel : MonoBehaviour
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
        Destroy(this.transform.parent.gameObject);
    }
}
