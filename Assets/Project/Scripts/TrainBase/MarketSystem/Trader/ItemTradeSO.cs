using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Market System/Item Trade", order = 1)]
public class ItemTradeSO : ScriptableObject
{

    public List<Requirements> requirements;
    public TradeItem itemReceived;
    public int id;
    public int daysToComplete;
}
[Serializable]
public class TradeItem
{
    public ItemReceivedByTrade item;
    public int amountReceived;
}

