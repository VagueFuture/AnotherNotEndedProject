using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootMode : MonoBehaviour
{
    Inventory lootBoxInventory;
    TonnelObject activObject;
    Dictionary<TonnelObject, List<Item>> lootHolders = new Dictionary<TonnelObject, List<Item>>();

    public void ResetLootMode()
    {
        lootHolders.Clear();
    }
    public void Init(Inventory lootBoxInventory, TonnelObject obj)
    {
        this.activObject = obj;
        this.lootBoxInventory = lootBoxInventory;
        lootBoxInventory.Initiate();
        lootBoxInventory.ResetInventory();

        if (!lootHolders.ContainsKey(obj))
        {
            GenerateLoot();
            lootHolders.Add(obj, this.lootBoxInventory.GetItemsFromInventory());
        }
        else
        {
            FillExistItems(lootHolders[obj]);
        }
    }

    public void RemoveItemFromLoot(Item item)
    {
        lootBoxInventory.RemoveItemFromInventory(item);
        lootHolders[activObject] = lootBoxInventory.GetItemsFromInventory();
        lootBoxInventory.UpdateInventory();
    }

    private void GenerateLoot()
    {
        List<Item> items = GameManager.Inst.GetGameItemsForLoot();
        int countItemsInBox = UnityEngine.Random.Range(0,7);
        UnityEngine.Random.Range(1, countItemsInBox);
        for (int i = 0; i < countItemsInBox; i++)
        {
            int r = UnityEngine.Random.Range(0, items.Count);
            Item item = GameManager.Inst.gameItemGenerator.GenerateItem(items[r]);

            lootBoxInventory.AddItemToInventory(item);
        }
    }

    private void FillExistItems(List<Item> items)
    {
        foreach(var item in items)
        {
            lootBoxInventory.AddItemToInventory(item);
        }
    }
}
