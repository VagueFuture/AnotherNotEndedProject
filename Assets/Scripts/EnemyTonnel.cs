using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTonnel : Tonnel
{

    public override void StartVitrual()
    {
        base.StartVitrual();
    }

    public override void PlayerIsCome(Character player)
    {
        base.PlayerIsCome(player);   
    }

    public override void ActionOnTonnel()
    {
        base.ActionOnTonnel();
    }


    public override void OnCharacterJumpInPosition(Character character)
    {
        base.OnCharacterJumpInPosition(character);
    }

    public override void TonnelInfoShowed()
    {
        base.TonnelInfoShowed();
    }

    public override void SkipTonnel()
    {
        base.SkipTonnel();
        /*float r = Random.Range(0, enemy.controller.stats.strength);
        player.controller.GetDamage(r);
        enemy.controller.Attak();*/
    }
}
