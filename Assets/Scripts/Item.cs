using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ItemData", menuName = "Items/Defult")]
public class Item : ScriptableObject
{
    public Sprite imageOnInventory;
    public ItemType typeItem;
    public TredeStatus tredeStatus = TredeStatus.PlayerOwner;
    public int price;
    public int Price
    {
        get
        {
            if (tredeStatus == TredeStatus.PlayerOwner) {
                int sellPrice = (int)(price * 0.15f);
                return sellPrice < 1 ? 1 : sellPrice;
            }
            if (tredeStatus == TredeStatus.SellerOwwner)
                return price;//(int)(price * 2);

            return price;
        }

        set
        {
            price = value;
        }
    }
    public GameObject modelTypePrefub;
}
