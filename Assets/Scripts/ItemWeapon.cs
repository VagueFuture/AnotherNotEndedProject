using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemWeapon", menuName = "Items/Weapon")]
public class ItemWeapon : Item
{
    public float stength;
    public float defendce;
    public int checkCount = 1;
    public bool infinityCheck = false;
    public float attackSpeed = 1;
    public Effect magickEffect;
}
