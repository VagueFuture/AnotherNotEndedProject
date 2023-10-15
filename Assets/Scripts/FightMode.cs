using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMode :MonoBehaviour
{
    public enum CheckType { Attak, Defend, Skill }
    public enum CheckResult { Hit, Crit, MagicHit, Miss };

    protected float _timer;
    protected bool screenPressed, fightInited;
    protected PanelFighPhase panelFighPhase;
    protected CheckType checkType;

    protected IEnumerator corr = null, corrWhait = null;

    Action onFightEnd;

    private void Start()
    {
        FightModeContriller.Inst.AddFightMode(this);
    }

    public virtual void InitFightMode(Character Attacker, Action onFightEnd)
    {
        GameManager.Inst.OnScreenPress += OnScreenPress;
        panelFighPhase = UiController.Inst.panelFighPhase;
        this.onFightEnd = onFightEnd;
    }

    public void OnScreenPress()
    {
        screenPressed = true;
    }

    public void Fight(MyCharacterController Attacker, MyCharacterController Defender)
    {
        if (!fightInited) return;

        if (Attacker.stats.agility >= Defender.stats.agility)
        {
            checkType = Attacker.player ? CheckType.Attak : CheckType.Defend;
            Attack(Attacker, Defender, () => { Turn(Attacker, Defender); });
        }
        else
        {
            checkType = Defender.player ? CheckType.Attak : CheckType.Defend;
            Attack(Defender, Attacker, () => { Turn(Defender, Attacker); });
        }
        fightInited = false;
    }

    public virtual void Attack(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd, bool needWhaitPressButton = true)
    {
        if (needWhaitPressButton)
        {
            panelFighPhase.brawlButton.onClick.RemoveAllListeners();
            panelFighPhase.brawlButton.onClick.AddListener(() =>
            {
                BrawlBegin(Attacker, Defender, checkType, onEnd);
                panelFighPhase.brawlButton.gameObject.SetActive(false);
            });
        }
        else
        {
            BrawlBegin(Attacker, Defender, checkType, onEnd,0);
            panelFighPhase.brawlButton.gameObject.SetActive(false);
        }
    }
    
    protected virtual void BrawlBegin(MyCharacterController Attacker, MyCharacterController Defender, CheckType checkType, Action onEnd, int time = 3)
    {
    }

    public IEnumerator WaitReady(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd)
    {
        while (!Attacker.ready || !Defender.ready)
            yield return null;

        onEnd?.Invoke();
    }

    public virtual CheckResult CheckSuccesTarget(CheckType checkType)
    {
        CheckResult result = CheckResult.Miss;
        return result;
    }

    int countCheck = 0;
    public void Turn(MyCharacterController Attacker, MyCharacterController Defender)
    {
        Attacker.Attak();
        CheckType checkType = Attacker.player ? CheckType.Attak : CheckType.Defend;
        CheckResult result = CheckSuccesTarget(checkType);

        switch (result)
        {
            case CheckResult.Hit:
                if (checkType == CheckType.Attak)
                    Defender.GetDamage(Attacker.stats.strength);
                else
                    Defender.Defend();
                break;
            case CheckResult.Crit:
                if (checkType == CheckType.Attak)
                    Defender.GetDamage(Attacker.stats.strength * Attacker.stats.critPercent);
                else
                    Defender.Defend();
                break;
            case CheckResult.Miss:
                if (checkType == CheckType.Defend)
                    Defender.GetDamage(Attacker.stats.strength * Attacker.stats.critPercent);
                else
                    Defender.Defend();
                break;
            case CheckResult.MagicHit:
                break;
        }

        if (!Defender.dead && !Attacker.dead)
        {
            countCheck++;
            if (countCheck < Attacker.EquipedItemWeapon.checkCount)
            {
                Attack(Attacker, Defender, () => { Turn(Attacker, Defender); },false);
            }
            else
            {
                countCheck = 0;
                Attack(Defender, Attacker, () => { Turn(Defender, Attacker); });
            }
        }
        else
        {
            GameManager.Inst.storyController.playerGoldCount += (int)(Defender.stats.strength + Defender.stats.defendce)*2;
            onFightEnd?.Invoke();
        }
    }
}
