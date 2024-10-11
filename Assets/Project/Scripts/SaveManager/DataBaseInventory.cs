
using System.Collections.Generic;
namespace SaveManagerNamespace
{
    public class DataBaseInventory
    {
        /// <summary>
        /// Int - Slot index
        /// ItemInBaseDataSave - Item in given slot
        /// </summary>
        public Dictionary<int, ItemInBaseDataSave> baseInventory;

        public DataBaseInventory(Dictionary<int, ItemInBaseDataSave> baseInventory)
        {
            this.baseInventory = baseInventory; 
        }
    
        public DataBaseInventory()
        {
            baseInventory = new Dictionary<int, ItemInBaseDataSave>();
        }

        public Dictionary<int, ItemInBaseDataSave> GetInventory()
        {
            return baseInventory;
        }
    }
}


public class ItemInBaseDataSave
{
    public int itemID;
    public int itemSlotAmount;


    public ItemInBaseDataSave(int itemID, int itemSlotAmount)
    {
        this.itemID = itemID;
        this.itemSlotAmount = itemSlotAmount;
    }
}
