using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMagicMode : FightMode
{
    private List<Rune> runes = new List<Rune>();
    public override void InitFightMode(Character player, Action onFightEnd)
    {
        base.InitFightMode(player, onFightEnd);
        if (player.controller.EquipedItemWeapon.typeItem == ItemType.SpellsBook)
            fightInited = true;

    }
    public override void Attack(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd, bool needWhaitPressButton = true)
    {
        base.Attack(Attacker, Defender, onEnd, needWhaitPressButton);
        panelFighPhase.InitMagic();
        panelFighPhase.FillMagicData(runes);
    }

    private void OnRuneSucces(Rune rune)
    {
        runes.Add(rune);
        panelFighPhase.FillMagicData(runes);
    }

    protected override void BrawlBegin(MyCharacterController Attacker, MyCharacterController Defender, CheckType checkType, Action onEnd, int time = 10)
    {
        time = (int)(Attacker.EquipedItemWeapon.attackSpeed + 0.5f);
        base.BrawlBegin(Attacker, Defender, checkType, onEnd, time);

        
        GameManager.Inst.magickController.InitMagicMode(OnRuneSucces);

        panelFighPhase.ShowCounter(time, panelFighPhase.magicCounterText, () =>
        {
            if (corrWhait != null)
                StopCoroutine(corrWhait);
            corrWhait = WaitReady(Attacker, Defender, () =>
            {
                SpellsTimerEnd(onEnd);
               
            });
            StartCoroutine(corrWhait);
        });
    }

    private void SpellsTimerEnd(Action OnEnd)
    {
        GameManager.Inst.magickController.DeactivateMagicMode();
        panelFighPhase.Hide();
        OnEnd?.Invoke();
    }

    public override CheckResult CheckSuccesTarget()
    {
        CheckResult result = CheckResult.MagicHit;
        return result;
    }
}
