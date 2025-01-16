using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "ScriptableObjects/MapGenerator/RoomData")]
public class RoomData : ScriptableObject, ISerializationCallbackReceiver
{
    public Vector2Int roomSize; // Room size in cells.
    [SerializeField] private BoolMatrix _shape; // Occupation matrix (true = occupied, false = not occupied).
    [SerializeField] private BoolMatrix _entrances; // Entrances matrix (true = entrance, false = not entrance).
    [SerializeField] private List<SerializableRoomEntrances> _serializableRoomEntrances;

    public Dictionary<Vector2Int, DirectionFlag> entrancesDirections; // Stores directions of each entrance.

    public void OnBeforeSerialize()
    {
        // Convert the dictionary back to a list for serialization
        _serializableRoomEntrances = new List<SerializableRoomEntrances>();
        foreach (var kvp in entrancesDirections)
        {
            _serializableRoomEntrances.Add(new SerializableRoomEntrances(kvp.Key, kvp.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        if (entrancesDirections == null)
        {
            entrancesDirections = new Dictionary<Vector2Int, DirectionFlag>();
        }

        // Convert the list to a dictionary
        if (_serializableRoomEntrances != null)
        {
            foreach (var item in _serializableRoomEntrances)
            {
                entrancesDirections[item.position] = item.directionFlags;
            }
        }
    }

    public void OnValidate()
    {
        if (_shape == null || _shape.GetLength(0) != roomSize.x || _shape.GetLength(1) != roomSize.y)
        {
            InitializeShape();
        }

        if (_entrances == null || _entrances.GetLength(0) != roomSize.x || _entrances.GetLength(1) != roomSize.y)
        {
            InitializeEntrances();
        }

        if (entrancesDirections == null)
        {
            entrancesDirections = new Dictionary<Vector2Int, DirectionFlag>();
        }
    }

    private void InitializeShape()
    {
        _shape = new BoolMatrix(roomSize.x, roomSize.y);
    }

    private void InitializeEntrances()
    {
        _entrances = new BoolMatrix(roomSize.x, roomSize.y);
    }

    public BoolMatrix GetShape()
    {
        return AuxiliarFunctions.CopyBoolMatrix(_shape);
    }

    public BoolMatrix GetEntrances()
    {
        return AuxiliarFunctions.CopyBoolMatrix(_entrances);
    }

    public BoolMatrix GetOriginalShape()
    {
        return _shape;
    }

    public BoolMatrix GetOriginalEntrances()
    {
        return _entrances;
    }
}
