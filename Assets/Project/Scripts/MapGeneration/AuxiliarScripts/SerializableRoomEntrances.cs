using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableRoomEntrances
{
    public Vector2Int position;
    public DirectionFlag directionFlags;

    public SerializableRoomEntrances(Vector2Int pos, DirectionFlag dirFlags)
    {
        position = pos;
        directionFlags = dirFlags;
    }
}

