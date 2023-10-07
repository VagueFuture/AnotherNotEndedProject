using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: Peredelat vce nafig
public class GameItemGenerator
{
    public int defoultSwordPrice = 5;
    int chanceSpecialStat = 25;
    public Item GenerateItem(Item original)
    {
        if (CheckTypeItem<ItemWeapon>(original))
        {
            return GenerateWeaponStats(original);
        }
        else if(CheckTypeItem<ItemCanUsed>(original))
        {
            return GenerateItemCanUse((ItemCanUsed)original);
        }
        else
        {
            return GenerateCopy(original);
        }
    }

    public ItemCanUsed GenerateItemCanUse(ItemCanUsed original)
    {
        ItemCanUsed item = new ItemCanUsed()
        {
            imageOnInventory = original.imageOnInventory,
            typeItem = original.typeItem,
            tredeStatus = TredeStatus.PlayerOwner,
            Price = original.Price,
            modelTypePrefub = original.modelTypePrefub,
            effect = original.effect
        };
        float r = Random.Range(1, original.effect.effectCount);
        item.effect.effectCount = (int)r;
        item.Price = (int)r;
        return item;
    }

    public ItemWeapon GenerateWeaponStats(Item original)
    {
        List<EffectStatus> weaponStatusEffects = new List<EffectStatus>();
        weaponStatusEffects.Add(EffectStatus.Nothing);
        weaponStatusEffects.Add(EffectStatus.MaxHPBuff);
        weaponStatusEffects.Add(EffectStatus.BuffAgulity);
        weaponStatusEffects.Add(EffectStatus.BuffCrit);

        ItemWeapon item = new ItemWeapon()
        {
            imageOnInventory = original.imageOnInventory,
            typeItem = original.typeItem,
            tredeStatus = TredeStatus.PlayerOwner,
            Price = original.Price,
            modelTypePrefub = original.modelTypePrefub
        };
        int r = Random.Range(0, weaponStatusEffects.Count);
        item.magickEffect = new Effect();
        item.magickEffect.status = weaponStatusEffects[r];
        item.magickEffect.effectCount = Random.Range(1, 10);

        int countRoom = GameManager.Inst.storyController.countRoom;
        float delta = Random.Range(-90, 100);
        float damage = countRoom + countRoom * (delta / 100);
        delta = Random.Range(-90, 100);
        float defence = countRoom + countRoom * (delta / 100);
        int checkCount = Random.Range(1, 4);
        float attackSpeed = (float)(3-(2.3*(damage/100))%3);
        attackSpeed = attackSpeed < 0.3f ? 0.3f : attackSpeed;
        float price = (damage * defoultSwordPrice) + (attackSpeed * defoultSwordPrice) * checkCount;
        item.stength = damage;
        item.defendce = defence;
        item.checkCount = checkCount;
        item.attackSpeed = attackSpeed;
        item.Price = (int)price;

        return item;
    }

    public Item GenerateCopy(Item original)
    {
        if (CheckTypeItem<ItemWeapon>(original))
        {
            ItemWeapon originalW = (ItemWeapon)original;
            ItemWeapon item = new ItemWeapon()
            {
                imageOnInventory = originalW.imageOnInventory,
                typeItem = originalW.typeItem,
                tredeStatus = TredeStatus.PlayerOwner,
                magickEffect = originalW.magickEffect,
                Price = originalW.Price,
                modelTypePrefub = originalW.modelTypePrefub,
                stength = originalW.stength,
                checkCount = originalW.checkCount,
                infinityCheck = originalW.infinityCheck,
                attackSpeed = originalW.attackSpeed
        };
            return item;
        }
        if (CheckTypeItem<ItemKey>(original))
        {
            ItemKey item = new ItemKey()
            {
                imageOnInventory = original.imageOnInventory,
                typeItem = original.typeItem,
                tredeStatus = TredeStatus.PlayerOwner,
                Price = original.Price,
                modelTypePrefub = original.modelTypePrefub
            };
            return item;
        }

        if (CheckTypeItem<ItemCanUsed>(original))
        {
            ItemCanUsed itemCU = (ItemCanUsed)original;
            ItemCanUsed item = new ItemCanUsed()
            {
                imageOnInventory = itemCU.imageOnInventory,
                typeItem = itemCU.typeItem,
                tredeStatus = TredeStatus.PlayerOwner,
                Price = itemCU.Price,
                modelTypePrefub = itemCU.modelTypePrefub,
                effect = itemCU.effect
            };
            return item;
        }

        Item Nitem = new Item()
        {
            imageOnInventory = original.imageOnInventory,
            typeItem = original.typeItem,
            tredeStatus = TredeStatus.PlayerOwner,
            Price = original.Price,
            modelTypePrefub = original.modelTypePrefub
        };
        return Nitem;
    }

    protected bool CheckTypeItem<T>(Item item) where T : Item
    {
        if (item is T)
            return true;
        else
            return false;
    }
}
