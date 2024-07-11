using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertColorHUD : MonoBehaviour
{

    public Image alertMeter;
    public Gradient colorGradient;
    public float detection, maxDetection;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Updates the fill ammount and the color of the alert meter
    public void UpdateHealth(float quantity)
    {
        detection = Mathf.Clamp(quantity, 0, maxDetection);
        alertMeter.fillAmount = quantity;

        //Debug.Log(colorGradient.Evaluate(alertMeter.fillAmount));
        alertMeter.color = colorGradient.Evaluate(alertMeter.fillAmount);
    }
}
