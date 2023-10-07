using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTonnel : Tonnel
{
    public Enemy enemy;
    private Character player;

    public override void StartVitrual()
    {
        base.StartVitrual();
        enemy.GenerateEquip();
    }

    public override void PlayerIsCome(Character player)
    {
        base.PlayerIsCome(player);
        this.player = player;
        
    }

    public override void ActionOnTonnel()
    {
        base.ActionOnTonnel();
        StartAction();
    }

    public void StartAction()
    {
        if (!jumpEnded || !showEnded || !actionStarted)
            return;

       
        FightMode.Inst.InitFightMode(this);
        UiController.Inst.panelTonnelInfo.Hide();
        FightMode.Inst.Fight(player.controller, enemy.controller);
    }

    public override void OnCharacterJumpInPosition(Character character)
    {
        base.OnCharacterJumpInPosition(character);
        StartAction();
    }

    public override void TonnelInfoShowed()
    {
        base.TonnelInfoShowed();
        StartAction();
    }

    public override void SkipTonnel()
    {
        base.SkipTonnel();
        float r = Random.Range(0, enemy.controller.stats.strength);
        player.controller.GetDamage(r);
        enemy.controller.Attak();
    }
}
