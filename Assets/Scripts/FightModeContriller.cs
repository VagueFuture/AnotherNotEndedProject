using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightModeContriller : MonoBehaviour
{
    private static FightModeContriller _Instance;
    public static FightModeContriller Inst
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<FightModeContriller>();
            return _Instance;
        }
    }

    List<FightMode> fightModes = new List<FightMode>();
    public void AddFightMode(FightMode fightMode)
    {
        fightModes.Add(fightMode);
    }

    public void InitFight(Enemy enemy, Character player, Action onFightEnd)
    {
        foreach (var fightMode in fightModes) {
            fightMode.InitFightMode(player, onFightEnd);
            fightMode.Fight(player.controller, enemy.controller);
        }
    }
}
