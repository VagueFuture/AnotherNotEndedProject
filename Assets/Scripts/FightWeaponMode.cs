using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightWeaponMode : FightMode
{
    float percent = 0;
    Vector2 winZone = Vector2.zero;
    Vector2 critZone = Vector2.zero;

    public override void InitFightMode(Character player, Action onFightEnd)
    {
        base.InitFightMode(player,onFightEnd);
        if (player.controller.EquipedItemWeapon.typeItem != ItemType.SpellsBook)
            fightInited = true;
        
    }
    public override void Attack(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd, bool needWhaitPressButton = true)
    {
        base.Attack(Attacker, Defender, onEnd, needWhaitPressButton);

        panelFighPhase.InitBrawl();

        float winPercent = 0;
        float critPercent = Attacker.stats.critChance;
        winPercent = DeffendAttackFormul(Attacker.stats.strength, Defender.stats.defendce);

        winZone = RandomPlaceOnCircl(winPercent);
        critZone = RandomPlaceOnCircl(critPercent);

        panelFighPhase.FillBrawlData(winZone, critZone, checkType);
    }

    protected override void BrawlBegin(MyCharacterController Attacker, MyCharacterController Defender, CheckType checkType, Action onEnd, int time = 3)
    {
        base.BrawlBegin(Attacker, Defender, checkType, onEnd, time);

        panelFighPhase.ShowCounter(time, panelFighPhase.counterText, () =>
        {
            if (corrWhait != null)
                StopCoroutine(corrWhait);
            corrWhait = WaitReady(Attacker, Defender, () =>
            {
                StartCircleCheck(checkType, Attacker.EquipedItemWeapon, onEnd);
            });
            StartCoroutine(corrWhait);
        });
    }

    public void StartCircleCheck(CheckType type, ItemWeapon weapon, Action onEnd)
    {
        if (corr != null)
            StopCoroutine(corr);

        corr = CheckCirclCor(onEnd, weapon, type != CheckType.Attak);

        StartCoroutine(corr);
    }

    public IEnumerator CheckCirclCor(Action onResult, ItemWeapon weapon, bool reverceArrow = false)
    {
        _timer = 0;
        screenPressed = false;
        while (!screenPressed)
        {
            _timer += Time.deltaTime / weapon.attackSpeed;
            if (weapon.infinityCheck)
                _timer = _timer % 1;
            else if (_timer >= 0.99f) screenPressed = true;

            panelFighPhase.RotateArrow(_timer, reverceArrow);

            yield return null;
        }
        if (reverceArrow)
            _timer = 1 - _timer;

        panelFighPhase.Hide();

        onResult?.Invoke();
    }

    public override CheckResult CheckSuccesTarget(CheckType checkType)
    {
        CheckResult result;
        bool hit = _timer >= winZone.x && _timer <= winZone.y;
        bool crit = _timer >= critZone.x && _timer <= critZone.y;
        result = hit ? CheckResult.Hit : CheckResult.Miss;
        result = crit ? CheckResult.Crit : result;
        return result;
    }

    public Vector2 RandomPlaceOnCircl(float percent)
    {
        float r = UnityEngine.Random.Range(0, 100 - percent);
        Vector2 res = new Vector2(0 + r, percent + r);
        res = res / 100;
        return res;
    }

    public float DeffendAttackFormul(float streng, float defend)
    {
        return (50 - ((defend / 100) - (streng / 100)) * 50) * 0.25f;
    }
}
