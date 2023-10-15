using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Effect
{
    public EffectStatus status;
    public int effectCount = 10;
    public void DoEffect(MyCharacterController character)
    {
        switch (status)
        {
            case EffectStatus.Nothing:
                break;
            case EffectStatus.Heal:
                character.stats.Health += effectCount;
                break;
            case EffectStatus.MaxHPBuff:
                character.stats.maxHealth += effectCount;
                break;
            case EffectStatus.GetGold:
                GameManager.Inst.storyController.playerGoldCount += effectCount;
                break;
            case EffectStatus.BuffAgulity:
                character.stats.agility += effectCount;
                break;
            case EffectStatus.BuffCrit:
                character.stats.critPercent += (float)effectCount / 10;
                break;
            case EffectStatus.BuffCritChance:
                character.stats.critChance += (float)effectCount / 10;
                break;
            case EffectStatus.BuffVision:
                character.stats.vision += effectCount;
                break;
            case EffectStatus.Damage:
                character.GetDamage(effectCount);
                break;
            case EffectStatus.shield:
                character.stats.invule += effectCount;
                break;
        }
    }

    public void DropEffect(MyCharacterController character)
    {
        switch (status)
        {
            case EffectStatus.Nothing:
                break;
            case EffectStatus.Heal:
                break;
            case EffectStatus.MaxHPBuff:
                character.stats.maxHealth -= effectCount;
                break;
            case EffectStatus.GetGold:
                break;
            case EffectStatus.BuffAgulity:
                character.stats.agility -= effectCount;
                break;
            case EffectStatus.BuffCrit:
                character.stats.critPercent -= (float)effectCount / 10;
                break;
            case EffectStatus.BuffCritChance:
                character.stats.critChance -= (float)effectCount / 10;
                break;
            case EffectStatus.BuffVision:
                character.stats.vision -= effectCount;
                break;
        }
    }
}

