using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeMode
{
    public Action<int, int, int> CalculateDataUpdate;

    Inventory playerCells, sellerCells, tradePlace;
    int countTraderItems = 5;
    Dictionary<InventoryCell, Item> shopCells = new Dictionary<InventoryCell, Item>();

    int needPay = 0;
    int playerExchange = 0;
    int sellerExchange = 0;
    public void Init(Inventory playerCells, Inventory sellerCells, Inventory tradePlace)
    {
        this.playerCells = playerCells;
        this.sellerCells = sellerCells;
        this.tradePlace = tradePlace;

        tradePlace.Initiate();
        tradePlace.ResetInventory();

        playerCells.Initiate();
        playerCells.ResetInventory();

        sellerCells.Initiate();
        sellerCells.ResetInventory();

        GenerateTraderProducts();
        FillPlayerCells();
        SetCallBackCells();
    }

    private void GenerateTraderProducts()
    {
        List<Item> items = GameManager.Inst.GetAllGameItems();
        UnityEngine.Random.Range(1, countTraderItems);
        for (int i = 0; i < countTraderItems; i++)
        {
            int r = UnityEngine.Random.Range(0, items.Count);
            Item item = GameManager.Inst.gameItemGenerator.GenerateItem(items[r]);

            item.tredeStatus = TredeStatus.SellerOwwner;
            sellerCells.AddItemToInventory(item);
        }
    }

    private void FillPlayerCells()
    {
        playerCells.ResetInventory();
        List<Item> playerItems = UiController.Inst.playerInventory.GetItemsFromInventory();
        foreach (var item in playerItems)
        {
            playerCells.AddItemToInventory(item);
        }
    }


    private void SetCallBackCells()
    {
        foreach (var cell in tradePlace.GetCellsFromInventory)
        {
            InventoryShopCell shopCell = (InventoryShopCell)cell;
            shopCell.itemAdded = ItemAddedInCells;
        }
    }

    private void ItemAddedInCells(InventoryCell cell, Item item)
    {
        if (shopCells.ContainsKey(cell))
        {
            shopCells[cell] = item;
        }
        else
        {
            shopCells.Add(cell, item);
        }
        CalculatePrice();
    }

    private void CalculatePrice()
    {
        playerExchange = 0;
        sellerExchange = 0;
        foreach (var cell in shopCells)
        {
            if (cell.Value == null) continue;

            switch (cell.Value.tredeStatus)
            {
                case TredeStatus.PlayerOwner:
                    playerExchange += cell.Value.Price;
                    break;
                case TredeStatus.SellerOwwner:
                    sellerExchange += cell.Value.Price;
                    break;
            }
        }
        needPay = sellerExchange -  playerExchange;
        CalculateDataUpdate?.Invoke(playerExchange, sellerExchange, needPay);
    }

    public void PressTreadButton()
    {
        if (needPay > GameManager.Inst.coutnPlayerGold) return;

        GameManager.Inst.storyController.playerGoldCount -= needPay;

        foreach (var cell in shopCells)
        {
            if (cell.Value == null) continue;

            switch (cell.Value.tredeStatus)
            {
                case TredeStatus.PlayerOwner:
                    UiController.Inst.playerInventory.RemoveItemFromInventory(cell.Value);
                    sellerCells.AddItemToInventory(cell.Value);
                    break;
                case TredeStatus.SellerOwwner:
                    cell.Value.tredeStatus = TredeStatus.PlayerOwner;
                    UiController.Inst.playerInventory.AddItemToInventory(cell.Value);
                    break;
            }
        }
        tradePlace.ResetInventory();
        //Init(playerCells,sellerCells,tradePlace);
        FillPlayerCells();
        sellerCells.UpdateInventory();
    }

    public void Leave()
    {
        tradePlace.ClearInventory();
        sellerCells.ClearInventory();
    }
}
