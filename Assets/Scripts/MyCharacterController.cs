using System;
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
    [SerializeField] ItemWeapon equipedWeapon, fist;
    [SerializeField] ItemWeapon equipedShield;
    GameObject equipweaponObj;
    GameObject equipShieldObj;
    [SerializeField] Transform weaponPlace, shieldPlace, spellPlace;
    private GameObject activeSpellEffect;
    public ItemWeapon EquipedItemWeapon
    {
        get
        {
            if (equipedWeapon == null)
                return fist;
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
            if (equipedShield == null)
                return fist;
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
        if (stats.invule > 0)
        {
            stats.invule--;
            return;
        }

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
            case ItemType.SpellsBook:
                return 3;
        }
        return 0;
    }

    public void CastSpell(Spell spell)
    {
        if (spell == null) return;
        if (spell.effectOnSelf)
            ApplyEffect(spell.effect);
        activeSpellEffect = Instantiate(spell.visualEffectCast, spellPlace);
        StartCoroutine(WaitCur(spell.spellTime, () => {
            Destroy(activeSpellEffect);
        }));
    }

    public void ApplySpellToMe(Spell spell)
    {
        activeSpellEffect = Instantiate(spell.visualEffectResult, spellPlace);
        if(!spell.effectOnSelf)
            ApplyEffect(spell.effect);
        StartCoroutine(WaitCur(spell.spellTime, () => {
            Destroy(activeSpellEffect);
        }));
    }

    IEnumerator WaitCur(float time, Action onEnd)
    {
        yield return new WaitForSeconds(time);
        onEnd?.Invoke();
    }
}