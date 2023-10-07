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

    private TonnelType region = TonnelType.NumOf;
    public Character character;
    public CameraManager cameraManager;
    public StoryController storyController;
    public GameItemGenerator gameItemGenerator = new GameItemGenerator();
    public Action<Stats> OnStatsUpdate;
    public Action<Tonnel> OnTonnelSpawned;
    public Action<Tonnel,Character> OnCharacterComeInTonnel;
    public Action<Tonnel> OnCharacterFinishTonnel;
    public Action<Character> OnCharacterJumpInPosition;
    public Action TonnelInfoShowed;
    public Action OnScreenPress;
    public Action<Item> OnItemInInventory;
    public Action<Item> OnItemRemoveFromInventory;
    public Action<Item,Inventory.PlaceType> OnItemEquiped;

    public List<Item> allTypeWeaponInGame = new List<Item>();
    public List<Item> allTypeKeysInGame = new List<Item>();
    public List<Item> allTypeUsedItemsInGame = new List<Item>();
    public List<Item> allJunkInGame = new List<Item>();
    public int coutnPlayerGold => storyController.playerGoldCount;

    private void Awake()
    {
    }

    void Start()
    {
        DialogSystem.GetText();
        OnCharacterComeInTonnel += CharacterComeInTonnel;
        OnStatsUpdate += UiController.Inst.panelStatsInfo.FillData;
        OnCharacterFinishTonnel += CharacterFinishTonnel;
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

    public TonnelType GetNextRoom()
    {
        int rand = UnityEngine.Random.Range(0, ((int)TonnelType.NumOf));
        return (TonnelType)Enum.GetValues(typeof(TonnelType)).GetValue(rand);
    }


}