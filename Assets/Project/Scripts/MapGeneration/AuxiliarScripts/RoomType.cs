using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class RoomType
{
    public string Name { private set; get; }
    private Dictionary<int, List<int>> variation_rotation = new Dictionary<int, List<int>>();

    public RoomType(string name, string variation, string rotation)
    {
        Name = name;
        AddRoom(variation, rotation);
    }

    public void AddRoom(string variation, string rotation)
    {
        int variationInt = int.Parse(variation);
        int rotationInt = int.Parse(rotation);

        if(variation_rotation.ContainsKey(variationInt))
        {
            variation_rotation[variationInt].Add(rotationInt);
        }
        else
        {
            List<int> rotations = new List<int>();
            rotations.Add(rotationInt);
            variation_rotation.Add(variationInt, rotations);
        }
    }

    public GameObject GetRandomVariationRotation()
    {
        System.Random rnd = new System.Random();

        int variation = rnd.Next(0, variation_rotation.Keys.Count) + 1;
        int rotation_index = rnd.Next(0, variation_rotation[variation].Count);
        int rotation = variation_rotation[variation][rotation_index];

        string path = $"Rooms/AllRooms/{Name}_{variation}_{rotation}";
        GameObject prefab = UnityEngine.Resources.Load<GameObject>(path);

        return prefab;
    }
}
