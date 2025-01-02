using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAuxController : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnFinishAimingAnimation()
    {
        Debug.Log($"END AIM ANIMATION");
        PlayerController.Instance.SetIsAimingValue(false);
    }
}
