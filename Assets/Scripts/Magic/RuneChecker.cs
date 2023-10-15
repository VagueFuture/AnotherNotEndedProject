using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class RuneChecker : MonoBehaviour
{
    [SerializeField] List<Rune> runes = new List<Rune>();
    public Action<Rune> OnRuneSuccesDraw;
    void Start()
    {
        GameManager.Inst.OnDragRuneEnd += CheckRune;
    }

    public void CheckRune(List<DirectRecord> resultDrag)
    {
        foreach (var rune in runes)
        {
            Sprite runeSprite = rune.ChechAtSame(resultDrag);
            if (runeSprite != null)
            {
                OnRuneSuccesDraw?.Invoke(rune);
                return;
            }
        }
    }
}

