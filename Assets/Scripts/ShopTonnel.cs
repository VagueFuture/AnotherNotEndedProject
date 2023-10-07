using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTonnel : Tonnel
{
    public override void ActionOnTonnel()
    {
        base.ActionOnTonnel();
        UiController.Inst.panelShop.ShowPanel();
        UiController.Inst.panelTonnelInfo.Hide();
    }
    public override void LeaveTonnel()
    {
        base.LeaveTonnel();
    }
}
