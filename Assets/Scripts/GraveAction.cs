using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveAction : CustomAction
{
    [SerializeField] Enemy corps;
    [SerializeField] InTonnelObject coffin;
    [SerializeField] Animator myAnimator;

    public override void Action()
    {
        base.Action();
        Digging();
    }

    private void Digging()
    {
        int r = Random.Range(0, 101);
        if (r >= 60)
        {
            myAnimator.SetTrigger("corps");
            tonnelOwner.SetEnemy(corps, tonnelOwner, player);
        }
        else
        {
            myAnimator.SetTrigger("coffin");
            tonnelOwner.AddTonnelObject(coffin);
        }
        onEnd?.Invoke();
    }
}
