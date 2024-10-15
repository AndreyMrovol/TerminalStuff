using ShipInventory.Helpers;
using ShipInventory.Objects;
using System.Collections.Generic;

namespace TerminalStuff.Compatibility
{
    internal class ShipInventoryCompat
    {
        //Only call this method after checking bool is true
        internal static int GetInventoryValue()
        {
            return ItemManager.GetTotalValue();
        }


        internal static void GetInventoryItems(out List<Item> itemsInventory)
        {
            itemsInventory = [];
            if (!Plugin.instance.ShipInventory)
                return;

            List<ItemData> allItems = [.. ItemManager.GetItems()];

            foreach (ItemData item in allItems)
            {
                Item thisItem = item.GetItem();
                itemsInventory.Add(thisItem);
            }


        }
    }
}
