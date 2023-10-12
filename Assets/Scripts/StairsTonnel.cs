using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsTonnel : Tonnel
{
    public override void PlayerIsCome(Character player)
    {
        base.PlayerIsCome(player);
        GameManager.Inst.storyController.LvlCount++;
    }
}
