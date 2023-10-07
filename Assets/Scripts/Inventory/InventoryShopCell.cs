using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryShopCell : InventoryCell
{
    public Action<InventoryCell, Item> itemAdded;

    public override void PutInCell(ItemInInventory item)
    {
        base.PutInCell(item);
        itemAdded?.Invoke(this,item.item);
    }

    public override void DropFromCell()
    {
        base.DropFromCell();
        itemAdded?.Invoke(this, null);
    }
}
