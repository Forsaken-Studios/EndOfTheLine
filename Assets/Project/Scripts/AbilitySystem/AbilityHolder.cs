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
    
    [Header("Test")]
    [SerializeField] private bool testAbility;
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

    private GameObject placeUseShortcut;
    private GameObject cancelShortcut;

    [SerializeField] private bool canThrowAbility = true;
    enum AbilityState
    {
        ready, preparing, activating, active, cooldown
    }

    private AbilityState state = AbilityState.ready;

    public KeyCode key;

    private void Start()
    {
        if (testAbility)
        {
            abilityUI.SetUpProperties(ability);
        }
        else
        {
            LoadAbilityEquipped();
        }
    }

    public void TryToCancelAbility()
    {
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

        if (abilityEquipped1 == 0)
        {
            //No ability equipped
            Debug.Log("NO ABILITY EQUIPPED");
            return;
        }
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
        try
        {
            if (ability != null)
            {
                this.ability = ability;
                //Load image to ability icon
                abilityUI.SetUpProperties(ability);
            }
        }
        catch (Exception e)
        {
            Debug.Log("ABILITY HOLDER ID: " + abilityHolderID + "NO HA CARGADO HABILIDAD");
        }
      

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
        if (GameManager.Instance.GameState == GameState.OnGame && ability != null)
        {
            switch (state)
            {
                case AbilityState.ready: 
                    //Primera vez que pulsamos, activamos el canvas
                    AbilityHolderReady();
                    break;  
                case AbilityState.preparing:
                    //Comprobamos dos casos, en el caso de que haya que activar una segunda vez, o lo lanzamos al instante, 
                    // es decir, si pasamos a active o activating
                    AbilityHolderPreparing(); 
                    break;
                case AbilityState.activating:
                    //Preparamos para cuando pulsemos por segunda vez y aparezca la habilidad
                    AbilityHolderActivating();
                    break;
                case AbilityState.active:
                    //Lanzamos habilidad
                    AbilityHolderActive();
                    break; 
                case AbilityState.cooldown:
                    //Comenzamos cooldown
                    AbilityHolderOnCooldown();
                    break; 
            }
        }
      
    }
    
    public void ActivateAbility()
    {
        ability.Activate(gameObject, positionToThrowAbility, positionToThrowAbility2);
        DestroyAllShortcuts();
        state = AbilityState.active;
        activeTime = ability.activeTime;
        needToReactivate = false;
    }
    
    private void ActivatingAbility()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Activate
            OverheatManager.Instance.IncreaseEnergy(ability.overheatCost);
            ability.Activating(gameObject, positionToThrowAbility, positionToThrowAbility2, out currentGameObjectCreated);
            ActivateShortcutsWhenActivatingAbility();
            GameManager.Instance.SetHolder(abilityHolderID, false);
            state = AbilityState.activating;
        }else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            //Destroy canvas
            GameManager.Instance.SetHolder(abilityHolderID, false);
            DestroyAllShortcuts();
            state = AbilityState.ready;
            Destroy(currentCanvasCreated);
        }
    }

    private void DestroyAllShortcuts()
    {
        if(placeUseShortcut != null)
            ShortcutsUIManager.Instance.RemoveShortcut(placeUseShortcut);
        if(cancelShortcut != null)
            ShortcutsUIManager.Instance.RemoveShortcut(cancelShortcut);
    }
    
    private void ActivateShortcutsWhenActivatingAbility()
    {
        if(placeUseShortcut != null)
            ShortcutsUIManager.Instance.RemoveShortcut(placeUseShortcut);
        if(cancelShortcut != null)
            ShortcutsUIManager.Instance.RemoveShortcut(cancelShortcut);
        if (abilityHolderID == 1)
            placeUseShortcut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.useAbilityQ);
        else
            placeUseShortcut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.useAbilityE);
    }

    private void ActivateShortcuts()
    {
        if (ability.needToBeReactivated)
            placeUseShortcut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.placeAbility);
        else
        {
            if (abilityHolderID == 1)
                placeUseShortcut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.useAbilityQ);
            else
                placeUseShortcut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.useAbilityE);
        }
        cancelShortcut = ShortcutsUIManager.Instance.AddShortcuts(ShortcutType.cancelAbility);
    }

    public void SetIfCanThrowAbility(bool aux)
    {
        this.canThrowAbility = aux;
    }

    private void AbilityHolderReady()
    {
        if (Input.GetKeyDown(key) && OverheatManager.Instance.CheckIfWeCanThrowAbility(ability.overheatCost))
        {
            ability.PrepareAbility(gameObject, this, out currentCanvasCreated);
            OverheatManager.Instance.SetHolderToPrepareAbility(abilityHolderID);
            ActivateShortcuts();
            isPreparingAbility = true;
            GameManager.Instance.SetHolder(abilityHolderID, true);
            state = AbilityState.preparing;
        }
    }

    private void AbilityHolderPreparing()
    {
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
                    if (Input.GetKeyDown(KeyCode.Mouse0) && canThrowAbility)
                    {
                        LogManager.Log("PLACING [" + ability.name + "]", FeatureType.Player);
                        needToReactivate = true;
                        GameManager.Instance.SetHolder(abilityHolderID, false);
                        ActivatingAbility();
                        //Colocar objeto en el sitio
                    }else if (Input.GetKeyDown(KeyCode.Mouse1))
                    {
                        GameManager.Instance.SetHolder(abilityHolderID, false);
                        state = AbilityState.ready;
                        DestroyAllShortcuts();
                        Destroy(currentCanvasCreated);
                    }
                }
            }
            else
            {
                ActivatingAbility();
            }
        }else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameManager.Instance.SetHolder(abilityHolderID, false);
            state = AbilityState.ready;
            DestroyAllShortcuts();
            Destroy(currentCanvasCreated);
        }
    }

    private void AbilityHolderActivating()
    {
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
    }

    private void AbilityHolderActive()
    {
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
    }

    private void AbilityHolderOnCooldown()
    {
        LogManager.Log("ABILITY ON COOLDOWN [" + ability.name + "]", FeatureType.Player);
        if (cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime;
        }
        else
        {
            state = AbilityState.ready;
        }
    }

  
}
