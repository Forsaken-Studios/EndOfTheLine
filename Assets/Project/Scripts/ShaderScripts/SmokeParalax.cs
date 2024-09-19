using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeParalax : MonoBehaviour
{
    
    private Material mat;
    private Vector2 startOffset;
    public GameObject cam;
    public float paralaxFactor=1;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;

        startOffset = mat.GetTextureOffset("_SmokeTex");
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 position = cam.transform.position;
        Vector2 dist = new Vector2(position.x, position.y);
        dist *= paralaxFactor;
        mat.SetTextureOffset("_SmokeTex",startOffset+dist);
        
    }
}
