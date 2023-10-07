using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCanUse", menuName = "Items/CanUse")]
public class ItemCanUsed : Item
{
    public Effect effect;
    public void UseItem(MyCharacterController character)
    {
        character.ApplyEffect(effect);
    }
}
