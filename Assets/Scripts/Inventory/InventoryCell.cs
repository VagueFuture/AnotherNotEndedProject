using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class InventoryCell : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemImage;
    public Transform transformCell;
    public ItemInInventory itemInInveotory;
    public List<ItemType> forItemsWithType = new List<ItemType>();
    Inventory inventory;
    Sprite deafultSprite;

    public void InitCell(Transform transF, Image itemImage, Inventory inventory)
    {
        this.itemImage = itemImage;
        this.inventory = inventory;
        transformCell = transF;
        deafultSprite = GetComponent<Image>().sprite;
    }

    public DraggableItemCell CreateDragItem(Vector3 position)
    {
        DraggableItemCell dragPref = inventory.SpawnDraggableItemCell(transform);
        dragPref.image.sprite = this.itemImage.sprite;
        dragPref.item = this.itemInInveotory;
        return dragPref;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragEvent(eventData);
    }
    public virtual void OnDragEvent(PointerEventData eventData)
    {
        if (itemInInveotory == null)
            return;

        Vector3 evpos = eventData.position;
        evpos.z = 1;
        Vector3 pointerPos = GameManager.Inst.cameraManager.cam.ScreenToWorldPoint(evpos);

        if (InventoryTransfer.dragItem == null)
            InventoryTransfer.dragItem = CreateDragItem(pointerPos);

        InventoryTransfer.dragItem.transform.position = pointerPos;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent(eventData);
    }
    public virtual void OnPointerDownEvent(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent(eventData);
    }
    public virtual void OnPointerUpEvent(PointerEventData eventData)
    {
        if (InventoryTransfer.dragItem == null)
        {
            if (itemInInveotory != null)
            {
                UiController.Inst.panelItemInfo.FillPanel(itemInInveotory);
                UiController.Inst.OpenHidePanelItemInfo();
            }

            return;
        }

        if (InventoryTransfer.dragItemUnderCell != null)
        {
            if (InventoryTransfer.dragItemUnderCell.itemInInveotory == null)
            {
                if (InventoryTransfer.dragItemUnderCell.CheckTypeCell(InventoryTransfer.dragItem.item.item))
                {
                    InventoryTransfer.dragItemUnderCell.PutInCell(InventoryTransfer.dragItem.item);
                    DropFromCell();
                    RemoveFromInventoryIfNeed();
                }
            }
        }
        inventory.DestroyDraggableItemCell(InventoryTransfer.dragItem);
        InventoryTransfer.dragItem = null;
    }

    private void RemoveFromInventoryIfNeed()
    {
        if (!inventory.CheckItemStillInInventory(InventoryTransfer.dragItem.item))
            inventory.RemoveItemFromInventory(InventoryTransfer.dragItem.item);
    }

    protected bool CheckTypeCell(Item item)
    {
        if (forItemsWithType.Count == 0)
            return true;
        foreach (var type in forItemsWithType)
        {
            if (type == item.typeItem)
                return true;
        }
        return false;
    }

    public virtual void PutInCell(ItemInInventory item)
    {
        itemImage.sprite = item.item.imageOnInventory;
        item.itemCell = this;
        item.ownerInventory = inventory;
        itemInInveotory = item;
    }

    public virtual void DropFromCell()
    {
        itemImage.sprite = deafultSprite;
        itemInInveotory = null;
    }

    public void RemoveFromInventory()
    {
        inventory.RemoveItemFromInventory(InventoryTransfer.dragItem.item);
        DropFromCell();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent(eventData);
    }
    public virtual void OnPointerEnterEvent(PointerEventData eventData)
    {
        InventoryTransfer.dragItemUnderCell = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent(eventData);
    }
    public virtual void OnPointerExitEvent(PointerEventData eventData)
    {
        InventoryTransfer.dragItemUnderCell = null;
    }
}


