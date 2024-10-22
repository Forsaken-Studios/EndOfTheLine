using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoomLoader
{
    private static GameObject[] _roomPrefabs;

    private static List<RoomType> _roomTypes = new List<RoomType>();

    public static void Load()
    {
        string path = $"Rooms/AllRooms";
        _roomPrefabs = UnityEngine.Resources.LoadAll<GameObject>(path);

        foreach (GameObject roomPrefab in _roomPrefabs)
        {
            string[] pathParts = roomPrefab.name.Split('_');

            string roomTypeName = pathParts[0];
            string roomVariation = pathParts[1];
            string roomRotation = pathParts[2];

            // Se añade la habitación diferenciando entre si la habitación ya estaba dentro o si no.
            bool isInsideList = false;
            foreach (RoomType roomType in _roomTypes)
            {
                if(roomType.Name == roomTypeName)
                {
                    roomType.AddRoom(roomVariation, roomRotation);
                    isInsideList = true;
                }
            }
            if(!isInsideList)
            {
                RoomType roomType = new RoomType($"{roomTypeName}", roomVariation, roomRotation);
                _roomTypes.Add(roomType);
            }
        }
    }

    public static GameObject GetRandomRoom()
    {
        if(_roomTypes.Count <= 0)
        {
            Debug.LogError("Error cargando las habitaciones.");
            return null;
        }

        System.Random rnd = new System.Random();
        int index = rnd.Next(0, _roomTypes.Count);

        return _roomTypes[index].GetRandomVariationRotation();
    }
}
