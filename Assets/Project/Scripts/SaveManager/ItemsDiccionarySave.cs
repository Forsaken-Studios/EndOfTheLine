
using System.Collections.Generic;


namespace SaveManagerNamespace
{
    public class ItemsDiccionarySave
    {
        public Dictionary<int , int> itemsSaved;

        public ItemsDiccionarySave()
        {
            itemsSaved = new Dictionary<int, int>();
        }
    
        public ItemsDiccionarySave(Dictionary<int, int> items)
        {
            itemsSaved = items;
        }

        public Dictionary<int, int> GetInventory()
        {
            return itemsSaved;
        }
    }
}

public class ItemsBoolDiccionarySave
{
    public Dictionary<int , bool> itemsSaved;

    public ItemsBoolDiccionarySave()
    {
        itemsSaved = new Dictionary<int, bool>();
    }
    
    public ItemsBoolDiccionarySave(Dictionary<int, bool> items)
    {
        itemsSaved = items;
    }

    public Dictionary<int, bool> GetInventory()
    {
        return itemsSaved;
    }
}
