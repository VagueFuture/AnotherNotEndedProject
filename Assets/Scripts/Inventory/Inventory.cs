using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    public enum PlaceType { Weapon, Shield }
    public Transform cellsParent;
    public List<InventoryCell> DopCells = new List<InventoryCell>();
    public DraggableItemCell draggableItemCell;
    List<InventoryCell> cells = new List<InventoryCell>();
    List<ItemInInventory> itemsInInventory = new List<ItemInInventory>();
    bool prepared;

    public void Initiate()
    {
        if (!prepared)
            FillCells();
    }

    public void UpdateInventory()
    {
        ClearInventory();
        foreach (var item in itemsInInventory)
        {
            if (item.itemCell == null)
                foreach (var cell in cells)
                {
                    if (cell.itemInInveotory == null)
                    {
                        cell.PutInCell(item);
                        break;
                    }
                }
            else
            {
                item.itemCell.PutInCell(item);
            }
        }
    }

    private void FillCells()
    {
        for (int i = 0; i < cellsParent.childCount; i++)
        {
            InventoryCell newCell = cellsParent.GetChild(i).GetComponent<InventoryCell>();
            if (newCell != null)
            {
                newCell.InitCell(cellsParent.GetChild(i), cellsParent.GetChild(i).GetComponent<Image>(), this);
                cells.Add(newCell);
            }
        }
        foreach (var c in DopCells)
        {
            c.InitCell(c.transform, c.GetComponent<Image>(), this);
        }
        prepared = true;
    }

    public void AddItemToInventory(Item newItem)
    {
        Initiate();

        if (cells.Count > itemsInInventory.Count)
        {
            ItemInInventory itemInInventory = new ItemInInventory(newItem);
            itemsInInventory.Add(itemInInventory);
            UpdateInventory();
            GameManager.Inst.OnItemInInventory?.Invoke(newItem);
        }
        else
        {
            Debug.Log("Not enough place");
        }
    }

    public void AddItemToInventory(Item newItem, InventoryCell cell)
    {
        Initiate();

        ItemInInventory itemInInventory = new ItemInInventory(newItem,cell);
        itemsInInventory.Add(itemInInventory);
        UpdateInventory();
        GameManager.Inst.OnItemInInventory?.Invoke(newItem);
    }

    public bool CheckItemStillInInventory(ItemInInventory item)
    {
        return item.ownerInventory == this;
    }

    public void RemoveItemFromInventory(ItemInInventory item)
    {
        Initiate();
        if (item != null)
        {
            itemsInInventory.Remove(item);
            GameManager.Inst.OnItemRemoveFromInventory?.Invoke(item.item);
            UpdateInventory();
        }
        else
        {
            Debug.Log("Item already not Have in Invenory");
        }
    }

    public void RemoveItemFromInventory(Item item)
    {
        ItemInInventory itemInInventory = FindInventoryItemWithItem(item);
        RemoveItemFromInventory(itemInInventory);
    }

    public void ClearInventory()
    {
        foreach (var cell in cells)
            cell.DropFromCell();
        foreach (var cell in DopCells)
            cell.DropFromCell();
    }

    public void ResetInventory()
    {
        itemsInInventory.Clear();
        ClearInventory();
    }

    public ItemInInventory FindInventoryItemWithItem(Item item)
    {
        return itemsInInventory.Find((x) => x.item == item);
    }

    public void PutItemInCell(Item item, InventoryCell cell)
    {
        ItemInInventory itemsInInventory = new ItemInInventory(item);
        cell.PutInCell(itemsInInventory);
    }

    public DraggableItemCell SpawnDraggableItemCell(Transform cellTr)
    {
        return Instantiate(draggableItemCell, cellTr.position, transform.rotation, transform.parent.parent);
    }
    public void DestroyDraggableItemCell(DraggableItemCell dragItem)
    {
        Destroy(dragItem.gameObject);
    }

    public List<Item> GetItemsFromInventory()
    {
        List<Item> result = new List<Item>();
        foreach (var item in itemsInInventory)
            result.Add(item.item);
        return result;
    }
    public List<InventoryCell> GetCellsFromInventory => cells;
}

public class ItemInInventory
{
    public Inventory ownerInventory;
    public Item item;
    public InventoryCell itemCell;

    public ItemInInventory(Item item)
    {
        this.item = item;
    }
    public ItemInInventory(Item item, InventoryCell cell)
    {
        this.item = item;
        this.itemCell = cell;
    }
}

