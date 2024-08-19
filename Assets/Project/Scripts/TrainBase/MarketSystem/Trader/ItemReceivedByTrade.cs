using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Market System/Item Received", order = 1)]
public class ItemReceivedByTrade : ScriptableObject
{
    public Item itemReceived;
}