using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomsDatabase", menuName = "ScriptableObjects/MapGenerator/RoomsDB")]
public class RoomsDataBase : ScriptableObject
{
    [SerializeField] private List<RoomWithConfiguration> roomsList;

    [SerializeField] private bool _isRefreshed = false;

    private void OnEnable()
    {
        if (roomsList == null || roomsList.Count == 0)
        {
            roomsList = new List<RoomWithConfiguration>();
        }

        if (!_isRefreshed)
        {
            LoadRooms();
        }
    }

    public void LoadRooms()
    {
        roomsList = new List<RoomWithConfiguration>();
        string path = $"Rooms/AllRooms";

        GameObject[] loadedRooms = UnityEngine.Resources.LoadAll<GameObject>(path);
        SortRooms(loadedRooms);

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

                // Se agrega una nueva configuración de habitación a la lista general
                roomsList.Add(new RoomWithConfiguration
                {
                    roomPrefab = roomPrefab,
                    configurationIndex = i,
                    openDirections = openDirections
                });
            }

            DestroyImmediate(roomObject);
        }
    }

    public List<RoomWithConfiguration> GetRoomsList()
    {
        return roomsList;
    }
}
