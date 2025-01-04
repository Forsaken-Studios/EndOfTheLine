using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomsDatabase", menuName = "ScriptableObjects/MapGenerator/RoomsDB")]
public class RoomsDataBase : ScriptableObject
{
    [SerializeField] private List<DirectionRoomsPair> serializedRoomsPrefabs;
    private Dictionary<DirectionFlag, List<RoomWithConfiguration>> roomsPrefabs;

    [SerializeField] private bool _isRefreshed = false;

    private void OnEnable()
    {
        if (roomsPrefabs == null)
        {
            roomsPrefabs = new Dictionary<DirectionFlag, List<RoomWithConfiguration>>();
            foreach (DirectionRoomsPair pair in serializedRoomsPrefabs)
            {
                roomsPrefabs[pair.direction] = pair.rooms;
            }
        }

        if (!_isRefreshed)
        {
            LoadRooms();
        }
    }

    public void LoadRooms()
    {
        roomsPrefabs = new Dictionary<DirectionFlag, List<RoomWithConfiguration>>();
        string path = $"Rooms/AllRooms";

        GameObject[] loadedRooms = UnityEngine.Resources.LoadAll<GameObject>(path);
        SortRooms(loadedRooms);

        serializedRoomsPrefabs = new List<DirectionRoomsPair>();
        foreach (var entry in roomsPrefabs)
        {
            serializedRoomsPrefabs.Add(new DirectionRoomsPair
            {
                direction = entry.Key,
                rooms = entry.Value
            });
        }

        _isRefreshed = true;
    }

    private void SortRooms(GameObject[] loadedRooms)
    {
        foreach (GameObject roomPrefab in loadedRooms)
        {
            GameObject roomObject = Instantiate(roomPrefab, Vector3.zero, roomPrefab.transform.rotation);
            Room room = roomObject.GetComponent<Room>();

            int amountRoomData = room.GetCountRoomData();
            for (int i = 0; i < amountRoomData; i++)
            {
                room.SetRoomData(i);
                Dictionary<Vector2Int, DirectionFlag> entrancesDirections = room.GetEntrancesDirections();

                List<DirectionFlag> openDirections = new List<DirectionFlag>();
                foreach (KeyValuePair<Vector2Int, DirectionFlag> directionEntry in entrancesDirections)
                {
                    if (!openDirections.Contains(directionEntry.Value))
                    {
                        openDirections.Add(directionEntry.Value);
                    }
                }

                foreach (KeyValuePair<Vector2Int, DirectionFlag> entry in entrancesDirections)
                {
                    DirectionFlag direction = entry.Value;

                    if (!roomsPrefabs.ContainsKey(direction))
                    {
                        roomsPrefabs[direction] = new List<RoomWithConfiguration>();
                    }
                    roomsPrefabs[direction].Add(new RoomWithConfiguration
                    {
                        roomPrefab = roomPrefab,
                        configurationIndex = i,
                        openDirections = openDirections
                    });
                }
            }

            DestroyImmediate(roomObject);
        }
    }

    public Dictionary<DirectionFlag, List<RoomWithConfiguration>>  GetRoomsPrefabs()
    {
        return roomsPrefabs;
    }
}

