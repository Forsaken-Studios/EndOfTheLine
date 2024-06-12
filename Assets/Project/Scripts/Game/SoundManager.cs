using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.CustomLogs;

[Serializable]
public class Sound
{
    private string audioName;
    private AudioClip audioClip;

    public Sound(string audioName, AudioClip audioClip)
    {
        this.audioClip = audioClip;
        this.audioName = audioName;
    }

    public string GetSoundName()
    {
        return audioName;
    }
}

public enum SoundAction
{
    Undefined,
    Inventory_OpenInventory,
    Inventory_CloseInventory,
    Inventory_MoveItem,
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    private List<Sound> inventoryAudioClips;
    //Se tendría que ver como lo renombramos en resources para tener varios sonidos para lo mismo
    private Dictionary<SoundAction, AudioClip> audioDictionary;
    [SerializeField] private AudioSource musicSource, sfxSource;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one SoundManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        inventoryAudioClips = new List<Sound>();
        audioDictionary = new Dictionary<SoundAction, AudioClip>();
        LoadAllSounds();
    }

    private void LoadAllSounds()
    {
       //LoadSpecificSounds("Sounds/Inventory", inventoryAudioClips);
       //Debug.Log(inventoryAudioClips[0].GetSoundName());
       LoadSpecificSoundsInDictionary("Sounds/Inventory");
    }

    private void LoadSpecificSounds(string path, List<Sound> soundList)
    {
        List<UnityEngine.Object> allSpecificItems = UnityEngine.Resources.LoadAll(path).ToList();
        foreach (var sound in allSpecificItems)
        {
            soundList.Add(new Sound(sound.ToString(), (AudioClip) sound));
        }
    }

    private void LoadSpecificSoundsInDictionary(string path)
    {
        List<UnityEngine.Object> allSpecificItems = UnityEngine.Resources.LoadAll(path).ToList();
        foreach (var sound in allSpecificItems)
        {
           
            SoundAction soundAction;
            if (Enum.IsDefined(typeof(SoundAction), sound.name))
            {
                soundAction = (SoundAction)(Enum.Parse(typeof(SoundAction), sound.name));  
            }
            else
            {
                
                soundAction = SoundAction.Undefined; 
                LogManager.Log("THERE IS NO ENUM TO ASSOCIATE WITH THIS NAME", FeatureType.General);
            }
          
            
            audioDictionary.Add(soundAction, (AudioClip) sound);
        }
        
        //CHECK 
        foreach (var pair in audioDictionary)
        {
            Debug.Log(pair.Key);
        }
     
    }
    
    private AudioClip GetAudioClipFromName(SoundAction audioAction)
    {
        if(audioDictionary.ContainsKey(audioAction))
            return audioDictionary[audioAction];
        LogManager.Log("THERE IS NO AUDIO FOR THAT NAME [" + audioAction.ToString() + "]", FeatureType.General);
        return null; 
    }

    public void ActivateSoundByName(SoundAction audioAction)
    {
        AudioClip audioClip = GetAudioClipFromName(audioAction);
        if (audioClip != null)
        {
            sfxSource.PlayOneShot(audioClip);
        }
    }
    
}
