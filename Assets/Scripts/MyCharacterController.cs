using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCharacterController : MonoBehaviour
{
    public bool player;
    public Stats stats;
    public Animator myAnim;
    public bool dead = false;
    [HideInInspector]
    public bool ready = true;
    [SerializeField] ItemWeapon equipedWeapon;
    [SerializeField] ItemWeapon equipedShield;
    GameObject equipweaponObj;
    GameObject equipShieldObj;
    [SerializeField] Transform weaponPlace, shieldPlace;
    public ItemWeapon EquipedItemWeapon
    {
        get
        {
            return equipedWeapon;
        }
        set
        {
            if (value != null)
            {
                stats.strength += value.stength;
                ApplyEffect(value.magickEffect);
                equipweaponObj = SpawnEquip(value, weaponPlace);
            }
            else if (equipedWeapon != null)
            {
                stats.strength -= equipedWeapon.stength;
                DropEffect(equipedWeapon.magickEffect);
                DestroyWeapon();
            }

            
            equipedWeapon = value;
            if(player)
                GameManager.Inst.OnStatsUpdate?.Invoke(stats);
        }
    }

    public ItemWeapon EquipedItemShield
    {
        get
        {
            return equipedShield;
        }
        set
        {
            if (value != null)
            {
                stats.defendce += value.defendce;
                ApplyEffect(value.magickEffect);
                equipShieldObj = SpawnEquip(value, shieldPlace);
            }
            else if (equipedShield != null)
            {
                stats.defendce -= equipedShield.defendce;
                DropEffect(equipedShield.magickEffect);
                DestroyShield();
            }
            equipedShield = value;
            if (player)
                GameManager.Inst.OnStatsUpdate?.Invoke(stats);
        }
    }
    public GameObject SpawnEquip(ItemWeapon itemWeapon, Transform place)
    {
        return Instantiate(itemWeapon.modelTypePrefub,place).gameObject;
    }

    public void DestroyWeapon()
    {
        if (equipweaponObj == null) return;
        Destroy(equipweaponObj);
        equipedWeapon = null;
    }

    public void DestroyShield()
    {
        if (equipShieldObj == null) return;
        Destroy(equipShieldObj);
        equipShieldObj = null;
    }

    public void ApplyEffect(Effect effect)
    {
        effect.DoEffect(this);
        GameManager.Inst.OnStatsUpdate?.Invoke(GameManager.Inst.character.controller.stats);
    }

    public void DropEffect(Effect effect)
    {
        effect.DropEffect(this);
        GameManager.Inst.OnStatsUpdate?.Invoke(GameManager.Inst.character.controller.stats);
    }

    public void GetDamage(float damage)
    {
        stats.Health -= damage;
        if (stats.Health <= 0)
        {
            Dead();
        }
        else
        {
            Hitted();
        }
    }

    public void Hitted()
    {
        ready = false;
        myAnim.SetTrigger("Hited");
    }

    public void Attak()
    {
        ready = false;
        myAnim.SetTrigger("Attack");
        myAnim.SetInteger("AttackType", GetWeaponIndex(EquipedItemWeapon.typeItem));
    }

    public void Defend()
    {
        ready = false;
        myAnim.SetTrigger("Defend");
    }

    public void Dead()
    {
        dead = true;
        ready = false;
        myAnim.SetTrigger("Dead");
    }

    public void InIdleSost()
    {
        ready = true;
    }

    public int GetWeaponIndex(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Sword:
                return 0;
            case ItemType.Spear:
                return 1;
            case ItemType.Shield:
                return 2;
        }
        return 0;
    }
}