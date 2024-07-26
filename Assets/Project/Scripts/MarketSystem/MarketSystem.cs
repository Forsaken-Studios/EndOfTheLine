using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MarketSystem : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI testingDay;
    
    private void Start()
    {
        TrainManager.Instance.OnDayChanged += UpdateStore;
    }

    private void UpdateStore(object sender, EventArgs e)
    {
        Debug.Log("UPDATING STORE");
        testingDay.text = PlayerPrefs.GetInt("CurrentDay").ToString();
    }



    
    
    
    
}
