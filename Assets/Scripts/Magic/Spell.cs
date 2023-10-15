using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Magic/Spell")]
public class Spell : ScriptableObject
{
    public List<Rune> runesCombination = new List<Rune>();
    public Effect effect;
}
