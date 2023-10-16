using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMagicMode : FightMode
{
    
    private MagicPanel magicPanel;
    public override void InitFightMode(Character player, Action onFightEnd)
    {
        base.InitFightMode(player, onFightEnd);
        panelFighPhase.InitMagic();
        magicPanel = UiController.Inst.magicPanel;
        if (player.controller.EquipedItemWeapon.typeItem == ItemType.SpellsBook)
            fightInited = true;

    }
    public override void Attack(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd, bool needWhaitPressButton = true)
    {
        panelFighPhase.InitMagic();
        base.Attack(Attacker, Defender, onEnd, needWhaitPressButton);
    }

   
    protected override void BrawlBegin(MyCharacterController Attacker, MyCharacterController Defender, CheckType checkType, Action onEnd, int time = 10)
    {
        time = (int)(Attacker.EquipedItemWeapon.attackSpeed + 0.5f);
        base.BrawlBegin(Attacker, Defender, checkType, onEnd, time);

        if(Attacker.player)
            AttachActions(Attacker, Defender);
        else
            AttachActions(Defender, Attacker);

        magicPanel.InitMagicMode();

        panelFighPhase.ShowCounter(time, panelFighPhase.magicCounterText, () =>
        {
            if (corrWhait != null)
                StopCoroutine(corrWhait);
            corrWhait = WaitReady(Attacker, Defender, () =>
            {
                GameManager.Inst.magickController.ReadRuneCombitation();

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
