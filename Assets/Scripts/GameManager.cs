using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyUinityEvent : UnityEvent { }
public class GameManager : MonoBehaviour
{
    private static GameManager _Instance;
    public static GameManager Inst
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<GameManager>();
            return _Instance;
        }
    }

    public Character character;
    public CameraManager cameraManager;
    public StoryController storyController;
    public MagicMode magickController;
    public GameItemGenerator gameItemGenerator = new GameItemGenerator();
    public Action<Stats> OnStatsUpdate;
    public Action<Tonnel> OnTonnelSpawned;
    public Action<Tonnel, Character> OnCharacterComeInTonnel;
    public Action<Tonnel> OnCharacterFinishTonnel;
    public Action<Character> OnCharacterJumpInPosition;
    public Action TonnelInfoShowed;
    public Action OnScreenPress;
    public Action<Item> OnItemInInventory;
    public Action<Item> OnItemRemoveFromInventory;
    public Action<Item, Inventory.PlaceType> OnItemEquiped;
    public Action<List<Tonnel>> OnTonnelInThisLvlUpdate;
    public Action<Spell> OnSpellCasted;

    public Action<List<DirectRecord>> OnDragRuneEnd;

    public List<Item> allTypeWeaponInGame = new List<Item>();
    public List<Item> allTypeKeysInGame = new List<Item>();
    public List<Item> allTypeUsedItemsInGame = new List<Item>();
    public List<Item> allJunkInGame = new List<Item>();
    public int coutnPlayerGold => storyController.playerGoldCount;

    private Stack<TonnelInfo> tonnelInfoStack = new Stack<TonnelInfo>();

    private void Awake()
    {
    }

    void Start()
    {
        DialogSystem.GetText();
        OnCharacterComeInTonnel += CharacterComeInTonnel;
        if (UiController.Inst != null)
            OnStatsUpdate += UiController.Inst.panelStatsInfo.FillData;
        OnCharacterFinishTonnel += CharacterFinishTonnel;
        OnTonnelInThisLvlUpdate += TonnelInThisLvlUpdate;
    }

    private void TonnelInThisLvlUpdate(List<Tonnel> tonnels)
    {
        UiController.Inst.panelHaveTonnels.ShowTonnelButtons(tonnels);
    }

    public List<Item> GetAllGameItems()
    {
        List<Item> result = new List<Item>();
        foreach (var i in allTypeWeaponInGame)
            result.Add(i);
        foreach (var i in allTypeUsedItemsInGame)
            result.Add(i);
        return result;
    }

    public List<Item> GetGameItemsForLoot()
    {
        List<Item> result = new List<Item>();
        foreach (var i in allJunkInGame)
            result.Add(i);
        foreach (var i in allTypeUsedItemsInGame)
            result.Add(i);
        return result;
    }

    private void CharacterFinishTonnel(Tonnel tonnel)
    {
        UiController.Inst.NextRoomAccess();
    }

    private void CharacterComeInTonnel(Tonnel newTonnel, Character character)
    {
        newTonnel.PlayerIsCome(character);
    }

    public void AddTonnelInfoInSteck(TonnelInfo tonnelInfo)
    {
        tonnelInfoStack.Push(tonnelInfo);
    }

    public void ShowTonnelInfosStack()
    {
        if (tonnelInfoStack.Count > 0)
            UiController.Inst.ShowTonnelInfo(tonnelInfoStack.Pop());
    }

    public TonnelType GetNextRoom()
    {
        int rand = UnityEngine.Random.Range(0, ((int)TonnelType.NumOf));
        return (TonnelType)Enum.GetValues(typeof(TonnelType)).GetValue(rand);
    }

}
