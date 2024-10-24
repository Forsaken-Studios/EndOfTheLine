using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavmeshManager : MonoBehaviour
{
    public static NavmeshManager Instance;

    private NavMeshSurface _NMS;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void GenerateNavmesh()
    {
        StartCoroutine(GenerateNavmeshCoroutine());
    }

    private IEnumerator GenerateNavmeshCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _NMS = GetComponent<NavMeshSurface>();
        _NMS.RemoveData();
        _NMS.BuildNavMesh();
    }
}
