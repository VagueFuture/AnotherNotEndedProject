using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMode : MonoBehaviour
{
    [SerializeField] GameObject magicCastManager;
    public RuneChecker runeChecker;
    Action<Rune> listner; 
    private void Start()
    {
        magicCastManager.gameObject.SetActive(false);
    }

    public void InitMagicMode(Action<Rune> OnRuneFind)
    {
        magicCastManager.gameObject.SetActive(true);
        runeChecker.OnRuneSuccesDraw += OnRuneFind;
        listner = OnRuneFind;
    }

    public void DeactivateMagicMode()
    {
        magicCastManager.SetActive(false);
        runeChecker.OnRuneSuccesDraw -= listner;
    }

    public void ReadRuneCombitation(List<Rune> runes)
    {

    }

}
