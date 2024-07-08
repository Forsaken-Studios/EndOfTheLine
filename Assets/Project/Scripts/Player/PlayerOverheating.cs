using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOverheating : IPlayer_Bar
{
            
    public static PlayerOverheating Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[PlayerStamina.cs] : There is already a PlayerStamina Instance");
            Destroy(this);
        }

        Instance = this;
    }

    public override void SetStamina(float newStamina)
    {
        this.stamina = Mathf.Clamp(newStamina, 0, MAX_STAMINA);
    }
}
