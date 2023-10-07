using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightMode : MonoBehaviour
{
    public enum CheckType { Attak, Defend, Skill }
    public enum CheckResult { Hit, Crit, Miss };

    float percent = 0;
    float _timer;
    Vector2 winZone = Vector2.zero;
    Vector2 critZone = Vector2.zero;

    bool screenPressed;
    bool fightEnded;
    PanelFighPhase panelFighPhase;
    Tonnel tonnelOwner;

    IEnumerator corr = null, corrWhait = null;

    private static FightMode _Instance;
    public static FightMode Inst
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<FightMode>();
            return _Instance;
        }
    }

    public void InitFightMode(Tonnel owner)
    {
        tonnelOwner = owner;
        GameManager.Inst.OnScreenPress += OnScreenPress;
        panelFighPhase = UiController.Inst.panelFighPhase;
    }

    public void OnScreenPress()
    {
        screenPressed = true;
    }

    public void Fight(MyCharacterController Attacker, MyCharacterController Defender)
    {
        if(Attacker.stats.agility>=Defender.stats.agility)
            Attack(Attacker, Defender, () => { Turn(Attacker, Defender); });
        else
            Attack(Defender, Attacker, () => { Turn(Defender, Attacker); });
    }

    public void Attack(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd, bool needWhaitPressButton = true)
    {
        panelFighPhase.Init();
        fightEnded = false;
        float winPercent = 0;
        float critPercent = Attacker.stats.critChance;
        winPercent = DeffendAttackFormul(Attacker.stats.strength, Defender.stats.defendce);


        winZone = RandomPlaceOnCircl(winPercent);
        critZone = RandomPlaceOnCircl(critPercent);

        CheckType checkType = Attacker.player ? CheckType.Attak : CheckType.Defend;
        panelFighPhase.FillData(winZone,critZone, checkType);

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

    private void BrawlBegin(MyCharacterController Attacker, MyCharacterController Defender, CheckType checkType, Action onEnd, int time = 3)
    {
        panelFighPhase.ShowCounter(time, () =>
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

    public void StartCircleCheck(CheckType type,ItemWeapon weapon, Action onEnd)
    {
        if (corr != null)
            StopCoroutine(corr);

        corr = CheckCirclCor(onEnd, weapon, type != CheckType.Attak);

        StartCoroutine(corr);
    }

    public IEnumerator WaitReady(MyCharacterController Attacker, MyCharacterController Defender, Action onEnd)
    {
        while (!Attacker.ready || !Defender.ready)
            yield return null;

        onEnd?.Invoke();
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

    public CheckResult CheckSuccesTarget()
    {
        //Debug.Log("_timer = " + _timer + " winZone.x " + winZone.x + " winZone.y" + winZone.y);
        CheckResult result;
        bool hit = _timer >= winZone.x && _timer <= winZone.y;
        bool crit = _timer >= critZone.x && _timer <= critZone.y;
        result = hit ? CheckResult.Hit : CheckResult.Miss;
        result = crit ? CheckResult.Crit : result;
        return result;
    }

    int countCheck = 0;
    public void Turn(MyCharacterController Attacker, MyCharacterController Defender)
    {
        Attacker.Attak();
        CheckResult result = CheckSuccesTarget();
        CheckType checkType = Attacker.player ? CheckType.Attak : CheckType.Defend;

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
                    Defender.GetDamage(Attacker.stats.strength*Attacker.stats.critPercent);
                else
                    Defender.Defend();
                break;
            case CheckResult.Miss:
                if (checkType == CheckType.Defend)
                    Defender.GetDamage(Attacker.stats.strength * Attacker.stats.critPercent);
                else
                    Defender.Defend();
                break;
        }

        if (!Defender.dead)
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
            tonnelOwner.LeaveTonnel();
        }
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
        return (50 - ((defend / 100) - (streng / 100)) * 50)*0.25f;
    }
}
