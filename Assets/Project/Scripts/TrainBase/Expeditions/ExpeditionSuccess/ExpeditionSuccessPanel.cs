using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpeditionSuccessPanel : MonoBehaviour
{
    private Button continueButton;

    [SerializeField] private TextMeshProUGUI itemList;
    private void OnEnable()
    {
        continueButton = GetComponentInChildren<Button>();
        continueButton.onClick.AddListener(() => ContinueButtonBehavior());
    }

    public void SetUpItemList(Dictionary<Item, int> items)
    {
        foreach (var item in items)
        {
            itemList.text += "x" + item.Value + " " + item.Key.itemName + "\n";
        }
    }
    
    private void ContinueButtonBehavior()
    {
        TrainManager.Instance.TrainStatus = TrainStatus.onExpeditionRoom;
        PlayerPrefs.SetInt("ExpeditionInProgress", 0);
        PlayerPrefs.SetInt("ExpeditionEndDay", 0);
        ExpeditionManager.Instance.GetMissionChooser().ResetStartExpeditionButton();
        Destroy(this.transform.parent.gameObject);
    }
}
