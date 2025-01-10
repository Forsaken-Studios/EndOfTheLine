using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RoomDictionaryEntry
{
    public int key;
    public List<RoomWithConfiguration> rooms;
}

[CreateAssetMenu(fileName = "RoomsDatabase", menuName = "ScriptableObjects/MapGenerator/RoomsDB")]
public class RoomsDataBase : ScriptableObject
{
    [SerializeField] private List<RoomDictionaryEntry> serializedRoomsDB = new List<RoomDictionaryEntry>();
    private Dictionary<int, List<RoomWithConfiguration>> roomsDB = new Dictionary<int, List<RoomWithConfiguration>>();

    [SerializeField] private bool _isRefreshed = false;

    private void OnEnable()
    {
        if (roomsDB == null || roomsDB.Count == 0)
        {
            DeserializeDictionary();
        }

        if (!_isRefreshed)
        {
            LoadRooms();
        }
    }

    public void LoadRooms()
    {
        roomsDB = new Dictionary<int, List<RoomWithConfiguration>>();
        string path = $"Rooms/AllRooms";

        GameObject[] loadedRooms = UnityEngine.Resources.LoadAll<GameObject>(path);
        SortRooms(loadedRooms);

        _isRefreshed = true;
        SerializeDictionary();
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

                // Se agrega una nueva configuración de habitación a la lista general
                RoomWithConfiguration newRoom = new RoomWithConfiguration
                {
                    roomPrefab = roomPrefab,
                    configurationIndex = i,
                    openDirections = openDirections
                };

                if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Down }))
                {
                    if (!roomsDB.ContainsKey(1))
                    {
                        roomsDB[1] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[1].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Left }))
                {
                    if (!roomsDB.ContainsKey(2))
                    {
                        roomsDB[2] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[2].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Up }))
                {
                    if (!roomsDB.ContainsKey(3))
                    {
                        roomsDB[3] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[3].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Right }))
                {
                    if (!roomsDB.ContainsKey(4))
                    {
                        roomsDB[4] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[4].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Up, DirectionFlag.Down }))
                {
                    if (!roomsDB.ContainsKey(5))
                    {
                        roomsDB[5] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[5].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Right }))
                {
                    if (!roomsDB.ContainsKey(6))
                    {
                        roomsDB[6] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[6].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Down }))
                {
                    if (!roomsDB.ContainsKey(7))
                    {
                        roomsDB[7] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[7].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Up }))
                {
                    if (!roomsDB.ContainsKey(8))
                    {
                        roomsDB[8] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[8].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Up, DirectionFlag.Right }))
                {
                    if (!roomsDB.ContainsKey(9))
                    {
                        roomsDB[9] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[9].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Right, DirectionFlag.Down }))
                {
                    if (!roomsDB.ContainsKey(10))
                    {
                        roomsDB[10] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[10].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Down, DirectionFlag.Left, DirectionFlag.Up }))
                {
                    if (!roomsDB.ContainsKey(11))
                    {
                        roomsDB[11] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[11].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Left, DirectionFlag.Up, DirectionFlag.Right }))
                {
                    if (!roomsDB.ContainsKey(12))
                    {
                        roomsDB[12] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[12].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Up, DirectionFlag.Right, DirectionFlag.Down }))
                {
                    if (!roomsDB.ContainsKey(13))
                    {
                        roomsDB[13] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[13].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Right, DirectionFlag.Down, DirectionFlag.Left }))
                {
                    if (!roomsDB.ContainsKey(14))
                    {
                        roomsDB[14] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[14].Add(newRoom);
                }
                else if (IsListEqual(openDirections, new List<DirectionFlag> { DirectionFlag.Right, DirectionFlag.Down, DirectionFlag.Left, DirectionFlag.Up }))
                {
                    if (!roomsDB.ContainsKey(15))
                    {
                        roomsDB[15] = new List<RoomWithConfiguration>();
                    }

                    roomsDB[15].Add(newRoom);
                }
            }

            DestroyImmediate(roomObject);
        }
    }

    bool IsListEqual(List<DirectionFlag> list1, List<DirectionFlag> list2)
    {
        // Se filtran las listas para ignorar los elementos None
        var filteredList1 = list1.Where(flag => flag != DirectionFlag.None).ToList();
        var filteredList2 = list2.Where(flag => flag != DirectionFlag.None).ToList();

        // Se comparan las listas para asegurarse de que coincidan exactamente
        bool result = filteredList1.Count == filteredList2.Count &&
                      filteredList1.All(filteredList2.Contains) &&
                      filteredList2.All(filteredList1.Contains);

        return result;
    }

    private void SerializeDictionary()
    {
        serializedRoomsDB.Clear();
        foreach (var kvp in roomsDB)
        {
            serializedRoomsDB.Add(new RoomDictionaryEntry
            {
                key = kvp.Key,
                rooms = kvp.Value
            });
        }
    }

    private void DeserializeDictionary()
    {
        roomsDB.Clear();
        foreach (var entry in serializedRoomsDB)
        {
            roomsDB[entry.key] = entry.rooms;
        }
    }

    public Dictionary<int, List<RoomWithConfiguration>> GetRoomDB()
    {
        return roomsDB;
    }
}
