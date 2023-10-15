using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells_Checker : MonoBehaviour
{
    public List<Spell> spells = new List<Spell>();

    public Spell CheckSpells(List<Rune> runes)
    {
        foreach(var spell in spells)
        {
            if (spell.runesCombination.Count != runes.Count) continue;

            bool notSpell = false;
            for(int i = 0; i < spell.runesCombination.Count; i++)
            {
                if (spell.runesCombination[i] != runes[i]) { notSpell = true; break; }
            }

            if (!notSpell)
                return spell;
        }

        return null;
    }
}
