using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallWidth : MonoBehaviour
{
    private BoxCollider2D collider2D;
    // Start is called before the first frame update
    void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        collider2D.size = new Vector2(AbilityManager.Instance.GetWallWidth(), collider2D.size.y); 
    }
}
