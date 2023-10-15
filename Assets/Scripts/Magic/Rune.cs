using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rune", menuName = "Magic/Rune")]
public class Rune : ScriptableObject
{
    public List<DirectRecord> runeDragSteps = new List<DirectRecord>();
    public Sprite runeSprite;

    public Sprite ChechAtSame(List<DirectRecord> resultDrag)
    {
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
}