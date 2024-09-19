using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils.CustomLogs;

public class AbilityHolder : MonoBehaviour
{
    [SerializeField] private int abilityHolderID;
    public Ability ability;
    protected float cooldownTime;
    protected float activeTime;
    private Vector2 positionToThrowAbility = Vector2.zero;
    private Vector2 positionToThrowAbility2 = Vector2.zero;
    [Header("UI")]
    [SerializeField] private AbilityUI abilityUI;

    private bool isPreparingAbility = false;
    private GameObject currentGameObjectCreated;
    private GameObject currentCanvasCreated;
    private bool needToReactivate;

    private bool canThrowAbility = true;
    enum AbilityState
    {
        ready, preparing, activating, active, cooldown
    }

    private AbilityState state = AbilityState.ready;

    public KeyCode key;

    private void Start()
    {
        LoadAbilityEquipped();
    }

    public void TryToCancelAbility()
    {
        Debug.Log("ID: " + abilityHolderID);
        if (state == AbilityState.preparing)
        {
            Destroy(currentCanvasCreated);
            this.state = AbilityState.ready;
        }
    }

    private void LoadAbilityEquipped()
    {
        int abilityEquipped1 = PlayerPrefs.GetInt("AbilityIDEquipped_" + abilityHolderID.ToString());
        int abilityEquippedInSlot1 = abilityEquipped1 / 10;
        int ability1Slot = abilityEquipped1 % 10;

        Debug.Log("COLOCANDO HABILIDAD DE SLOT" + ability1Slot);
        int abilityIDtoEquip = 0;
        
        if (ability1Slot == 1)
        {
            //Ability 1 is in slot 1
            abilityIDtoEquip = abilityEquippedInSlot1;
        }
        else
        {
            //Ability 2 is in slot 2
            int abilityEquipped2 = PlayerPrefs.GetInt("AbilityIDEquipped_2");
            abilityIDtoEquip = abilityEquipped1 / 10;
        }
        
        //Get ability from ID
        Ability ability = FindAbilityID(abilityIDtoEquip);
        this.ability = ability;
        //Load image to ability icon
        Debug.Log(ability);
        abilityUI.SetUpProperties(ability);
    }

    private Ability FindAbilityID(int id)
    {
        List<Ability> abilityList = UnityEngine.Resources.LoadAll<Ability>("Abilities").ToList();

        foreach (var ability in abilityList)
        {
            if (ability.abilityID == id)
            {
                return ability;
            }
        }

        return null;
    }

    public void UpdatePositionToThrowAbility(Vector2 position, Vector2 position2)
    {
        this.positionToThrowAbility = position;
        this.positionToThrowAbility2 = position2;
    }

    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key) && OverheatManager.Instance.CheckIfWeCanThrowAbility(ability.overheatCost))
                {
                    ability.PrepareAbility(gameObject, this, out currentCanvasCreated);
                    OverheatManager.Instance.SetHolderToPrepareAbility(abilityHolderID);
                    isPreparingAbility = true;
                    GameManager.Instance.SetHolder(abilityHolderID, true);
                    state = AbilityState.preparing;
                }
                break;  
            case AbilityState.preparing:
                if (canThrowAbility)
                {
                    //LogManager.Log("PREPARING ABILITY [" + ability.name + "]", FeatureType.Player);
                    if (ability.needToBeReactivated)
                    {
                        // needToReactivate == true -> Ya la hemos colocado y está a la espera
                        if (needToReactivate)
                        {
                            LogManager.Log("WAITING [" + ability.name + "]", FeatureType.Player);
                            ActivateAbility();
                        }
                        else
                        {
                            if (Input.GetKeyDown(key))
                            {
                                LogManager.Log("PLACING [" + ability.name + "]", FeatureType.Player);
                                needToReactivate = true;
                                GameManager.Instance.SetHolder(abilityHolderID, false);
                                ActivatingAbility();
                                //Colocar objeto en el sitio
                            }else if (Input.GetKeyDown(KeyCode.Escape))
                            {
                                GameManager.Instance.SetHolder(abilityHolderID, false);
                                state = AbilityState.ready;
                                Destroy(currentCanvasCreated);
                            }
                        }
                    }
                    else
                    {
                        ActivatingAbility();
                    }
                }
                break;
            case AbilityState.activating:
                //To activate ability, we will need to call ActivateAbility();
                if(!ability.needToBeReactivated)
                    ActivateAbility();
                else
                {
                    if (Input.GetKeyDown(key))
                    {
                        ActivateAbility();
                    }
                }
                break;
            case AbilityState.active:
                LogManager.Log("ABILITY ACTIVATED [" + ability.name + "]", FeatureType.Player);
                if (activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    Destroy(currentGameObjectCreated);
                    abilityUI.StartCooldown();
                    ability.BeginCooldown(gameObject);
                    state = AbilityState.cooldown;
                    cooldownTime = ability.cooldownTime;
                }
                break; 
            case AbilityState.cooldown:
                LogManager.Log("ABILITY ON COOLDOWN [" + ability.name + "]", FeatureType.Player);
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.ready;
                }
                break; 
        }
    }

    private void CancelOtherAbility()
    {
        if (abilityHolderID == 1)
        {
            Debug.Log("CANCELING ID: 2");
            OverheatManager.Instance.GetHolder2().TryToCancelAbility();
        }
        else
        {
            Debug.Log("CANCELING ID: 1");
            OverheatManager.Instance.GetHolder1().TryToCancelAbility();
        }
    }

    
    public void ActivateAbility()
    {
        ability.Activate(gameObject, positionToThrowAbility, positionToThrowAbility2);
        state = AbilityState.active;
        activeTime = ability.activeTime;
    }
    
    private void ActivatingAbility()
    {
        if (Input.GetKeyDown(key))
        {
            //Activate
            OverheatManager.Instance.IncreaseEnergy(ability.overheatCost);
            ability.Activating(gameObject, positionToThrowAbility, positionToThrowAbility2, out currentGameObjectCreated);
            GameManager.Instance.SetHolder(abilityHolderID, false);
            state = AbilityState.activating;
        }else if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Destroy canvas
            GameManager.Instance.SetHolder(abilityHolderID, false);
            state = AbilityState.ready;
            Destroy(currentCanvasCreated);
        }
    }

    public void SetIfCanThrowAbility(bool aux)
    {
        this.canThrowAbility = aux;
    }
    

  
}
