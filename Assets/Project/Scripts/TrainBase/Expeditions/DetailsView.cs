using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsView : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI stationName; 
    [SerializeField] private TextMeshProUGUI descriptionName;
    [SerializeField] private Image stationImage;



    public void SetUpDetailsView(ExpeditionSO expedition)
    {
        this.stationImage.sprite = expedition.expeditionImage;
        this.stationName.text = expedition.stationName;
        this.descriptionName.text = expedition.stationDescription;
    }
}
