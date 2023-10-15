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
        panelFighPhase.InitMagic();
        if (player.controller.EquipedItemWeapon.typeItem == ItemType.SpellsBook)
            fightInited = true;

    }
    public override void Attack(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd, bool needWhaitPressButton = true)
    {
        panelFighPhase.InitMagic();
        base.Attack(Attacker, Defender, onEnd, needWhaitPressButton);
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

        if(Attacker.player)
            AttachActions(Attacker, Defender);
        else
            AttachActions(Defender, Attacker);

        GameManager.Inst.magickController.InitMagicMode(OnRuneSucces);

        panelFighPhase.ShowCounter(time, panelFighPhase.magicCounterText, () =>
        {
            if (corrWhait != null)
                StopCoroutine(corrWhait);
            corrWhait = WaitReady(Attacker, Defender, () =>
            {
                GameManager.Inst.magickController.ReadRuneCombitation(runes);

                if (Attacker.player)
                    DetachAction(Attacker,Defender);
                else
                    DetachAction(Defender, Attacker);

                SpellsTimerEnd(onEnd);
            });
            StartCoroutine(corrWhait);
        });
    }

    private void SpellsTimerEnd(Action OnEnd)
    {
        runes.Clear();
        GameManager.Inst.magickController.DeactivateMagicMode();
        panelFighPhase.Hide();
        OnEnd?.Invoke();
    }

    private void AttachActions(MyCharacterController Attacker, MyCharacterController Defender)
    {
        GameManager.Inst.OnSpellCasted += Attacker.CastSpell;
        GameManager.Inst.OnSpellCasted += Defender.ApplySpellToMe;
    }

    private void DetachAction(MyCharacterController Attacker, MyCharacterController Defender)
    {
        GameManager.Inst.OnSpellCasted -= Attacker.CastSpell;
        GameManager.Inst.OnSpellCasted -= Defender.ApplySpellToMe;
    }

    public override CheckResult CheckSuccesTarget(CheckType checkType)
    {
        CheckResult result;
        if (checkType == CheckType.Defend)
            result = CheckResult.Miss;
        else
            result = CheckResult.MagicHit;
        return result;
    }


}
