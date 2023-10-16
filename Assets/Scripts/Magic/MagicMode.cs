using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMode : MonoBehaviour
{
    [SerializeField] GameObject magicCastManager;
    public RuneChecker runeChecker;
    public Spells_Checker spells_Checker;
    private List<Rune> runes = new List<Rune>();
    List<Action<Rune>> listners = new List<Action<Rune>>();
    public Action<List<Rune>> runesUpdate;
    private void Start()
    {
        magicCastManager.gameObject.SetActive(false);
    }

    public void InitMagicMode(Action<Rune> OnRuneFind)
    {
        DeactivateMagicMode();
        magicCastManager.gameObject.SetActive(true);
        
        runeChecker.OnRuneSuccesDraw += OnRuneSucces;
        listners.Add(OnRuneSucces);
        if (OnRuneFind != null)
        {
            runeChecker.OnRuneSuccesDraw += OnRuneFind;
            listners.Add(OnRuneFind);
        }
    }

    public void DeactivateMagicMode()
    {
        magicCastManager.SetActive(false);
        foreach(var listner in listners)
            runeChecker.OnRuneSuccesDraw -= listner;
    }

    public void ReadRuneCombitation()
    {
        Spell spell = spells_Checker.CheckSpells(runes);
        runes.Clear();
        runesUpdate?.Invoke(runes);
        if (spell == null) return;

        GameManager.Inst.OnSpellCasted?.Invoke(spell);
    }

    private void OnRuneSucces(Rune rune)
    {
        runes.Add(rune);
        runesUpdate?.Invoke(runes);
    }

}
