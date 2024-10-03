using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShaderConfig : MonoBehaviour
{
    [Range(0.0f, 10f)]
    [SerializeField] private float electricitySpeed;
    private Material mat;

    [SerializeField]
    private GameObject endSpawn;
    
    private GameObject wallParent;
    private Vector4 spawnLocations;
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        wallParent = transform.parent.gameObject;
        Vector4 spawnLocations = new Vector4();
    }

    // Update is called once per frame
    void Update()
    {
        spawnLocations.x = wallParent.transform.position.x;
        spawnLocations.y = wallParent.transform.position.y;
        spawnLocations.z = endSpawn.transform.position.x;
        spawnLocations.w = endSpawn.transform.position.y;
        
        mat.SetFloat("_ElecSpeed",electricitySpeed);
        mat.SetVector("_SpawnPoints",spawnLocations);
    }
}
