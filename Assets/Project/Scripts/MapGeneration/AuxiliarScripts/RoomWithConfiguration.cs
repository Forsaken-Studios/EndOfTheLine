using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomWithConfiguration
{
    public GameObject roomPrefab = null;
    public int configurationIndex = -1;
    public List<DirectionFlag> openDirections = new List<DirectionFlag>();
}
