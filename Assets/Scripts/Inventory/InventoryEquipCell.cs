using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEquipCell : InventoryCell
{
    public ItemWeapon itemWeapon;
    public Inventory.PlaceType placeType;
    public override void PutInCell(ItemInInventory item)
    {
        base.PutInCell(item);

        itemWeapon = (ItemWeapon)item.item;
        CallBackItemEquiped();

    }

    public override void DropFromCell()
    {
        base.DropFromCell();
        itemWeapon = null;
        CallBackItemEquiped();
    }

    public void CallBackItemEquiped()
    {
        GameManager.Inst.OnItemEquiped?.Invoke(itemWeapon, placeType);
    }
}
