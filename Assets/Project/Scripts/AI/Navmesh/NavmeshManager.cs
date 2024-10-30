using NavMeshPlus.Components;
using System.Collections;
using UnityEngine;

public class NavmeshManager : MonoBehaviour
{
    public static NavmeshManager Instance;

    [SerializeField] private NavMeshSurface _NMS;

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
        
        if (_NMS == null)
        {
            Debug.LogError("NavMeshSurface reference is missing!");
            yield break;
        }
        _NMS.RemoveData();
        _NMS.BuildNavMesh();
    }
}
