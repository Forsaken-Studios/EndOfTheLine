using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CorridorsDatabase", menuName = "ScriptableObjects/MapGenerator/CorridorDB")]
public class CorridorsDataBase : ScriptableObject
{
    public List<GameObject> corridorPrefabs;
    [SerializeField] private bool _isRefreshed = false;

    public void LoadCorridors()
    {
        if (_isRefreshed == false)
        {
            corridorPrefabs = new List<GameObject>();

            for (int i = 1; i <= 15; i++)
            {
                string path = $"Corridors/{i}";

                GameObject[] loadedCorridors = UnityEngine.Resources.LoadAll<GameObject>(path);
                corridorPrefabs.AddRange(loadedCorridors);
            }

            _isRefreshed = true;
        }
    }
}
