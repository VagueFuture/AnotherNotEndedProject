using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicTonnel : Tonnel
{
    public override void PlayerIsCome(Character player)
    {
        base.PlayerIsCome(player);
    }
    public override void ActionOnTonnel()
    {
        base.ActionOnTonnel();
        if (player.controller.stats.Health > 70)
            GameManager.Inst.storyController.playerGoldCount += (int)(player.controller.stats.Health - 70);

        player.controller.stats.Health = 70;
        LeaveTonnel();
    }
}
