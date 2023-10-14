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
        GameManager.Inst.runeChecker = this;
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
[System.Serializable]
public class Rune
{
    public List<DirectRecord> runeDragSteps = new List<DirectRecord>();
    public Sprite runeSprite;

    public Sprite ChechAtSame(List<DirectRecord> resultDrag)
    {
        /*float fullDistanceResult = GetFullDistance(resultDrag);
        float myRuneFullDistance = GetFullDistance(runeDragSteps);
        float percent = myRuneFullDistance / fullDistanceResult;*/

        if (resultDrag.Count != runeDragSteps.Count) return null;

        bool notSame = false;

        for (int i = 0; i < runeDragSteps.Count; i++)
        {
            if (resultDrag[i].directionType != runeDragSteps[i].directionType)
                notSame = true;
        }

        if (notSame)
            return null;
        else
            return runeSprite;
    }

  /*  public float GetFullDistance(List<DirectRecord> runeDragSteps)
    {
        float allDistance = 0;
        foreach (var r in runeDragSteps)
            allDistance += r.distance;

        return allDistance;
    }*/
}
