using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Karcer : Tonnel
{
    public override void PlayerIsCome(Character player)
    {
        base.PlayerIsCome(player);
    }

    public override void ActionOnTonnel()
    {  
        base.ActionOnTonnel();
        StartAction();
    }

    public void StartAction()
    {

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

   

}
