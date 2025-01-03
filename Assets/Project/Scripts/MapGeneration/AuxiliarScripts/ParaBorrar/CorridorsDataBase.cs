using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CorridorsDatabase", menuName = "ScriptableObjects/MapGenerator/CorridorDB")]
public class CorridorsDataBase : ScriptableObject
{
    public List<GameObject> corridorPrefabs;
    [SerializeField] private bool _isRefreshed = false;

    private void OnEnable()
    {
        if (!_isRefreshed)
        {
            LoadCorridors();
        }
    }

    public void LoadCorridors()
    {
        corridorPrefabs = new List<GameObject>();

        for (int i = 1; i <= 16; i++)
        {
            string path = $"Corridors/{i}";

            GameObject[] loadedCorridors = UnityEngine.Resources.LoadAll<GameObject>(path);
            corridorPrefabs.AddRange(loadedCorridors);
        }

        _isRefreshed = true;
    }
}

