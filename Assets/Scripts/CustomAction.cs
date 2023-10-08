using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAction : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private string textKey;
    protected Tonnel tonnelOwner;
    protected Character player;
    protected Action onEnd;

    public virtual TonnelInfo Init(Tonnel tonnel,Character player, Action onEnd)
    {
        tonnelOwner = tonnel;
        this.player = player;
        this.onEnd = onEnd;

        TonnelInfo tonnelInfo = new TonnelInfo()
        {
            tonnelImage = sprite,
            tonnelText = textKey
        };

        tonnelInfo.ActivteTonnel = Action;
        tonnelInfo.SkipTonnel = SkipAction;
        return tonnelInfo;
    }

    public virtual void Action()
    {
        
    }

    public virtual void SkipAction()
    {

    }

}
