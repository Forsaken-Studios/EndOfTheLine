using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player.PlayerController;

public class FollowCharacter : MonoBehaviour
{

    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerControllerInstance != null)
        {
            playerTransform = PlayerControllerInstance.gameObject.transform;
        }
        else
        {
            Debug.LogWarning("[NoiseCircleShader.cs] : There is no PlayerController Instance in the Scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position =
            new Vector3(playerTransform.transform.position.x, playerTransform.transform.position.y, -2);
            

    }
}
