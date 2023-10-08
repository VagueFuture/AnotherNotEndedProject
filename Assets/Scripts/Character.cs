using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

public class Character : MonoBehaviour
{
    public MyCharacterController controller;

    private void Start()
    {
        GameManager.Inst.OnTonnelSpawned += ActionWithNewTonnel;
        GameManager.Inst.OnItemEquiped += OnItemEquiped;
    }

    public void ActionWithNewTonnel(Tonnel newTonnel)
    {
        JumpToTonnel(newTonnel);
    }

    public void OnItemEquiped(Item newitem, Inventory.PlaceType pType)
    {
        ItemWeapon item = (ItemWeapon)newitem;
        switch (pType)
        {
            case Inventory.PlaceType.Weapon:
                controller.EquipedItemWeapon = item;
                break;
            case Inventory.PlaceType.Shield:
                controller.EquipedItemShield = item;
                break;
        }

    }

    public void JumpInTonnel(Transform position)
    {
        transform.DOJump(position.position, 0.5f, 1, 1)
            .OnComplete(() => { GameManager.Inst.OnCharacterJumpInPosition?.Invoke(this); });
        transform.DORotate(position.rotation.eulerAngles, 1);
    }

    public void JumpToTonnel(Tonnel target)
    {
        transform.DOJump(target.placeForPlayerStep.position, 0.5f, 1, 1)
            .OnComplete(() => { GameManager.Inst.OnCharacterComeInTonnel?.Invoke(target, this); });
        transform.DORotate(target.placeForPlayerStep.rotation.eulerAngles, 1);
    }

}
[System.Serializable]
public class Stats
{
    public float maxHealth = 100;
    private float health = 100;
    public float Health {
        get
        {
            return health;
        }
        set
        {
            if ((value) <= maxHealth)
                health = value;
            else
                health = maxHealth;
        }
    }
    public float strength = 1;
    public float defendce = 1;
    public float luck = 0;
    public float agility = 0;
    public float critChance = 1;
    public float critPercent = 1.5f;
}
