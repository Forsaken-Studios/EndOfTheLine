using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySpecificSound : MonoBehaviour
{
    public void PlaySound(string sound)
    {
        SoundManager.Instance.ActivateSoundByName((SoundAction)System.Enum.Parse(typeof(SoundAction), sound), null, true);
    }
}
