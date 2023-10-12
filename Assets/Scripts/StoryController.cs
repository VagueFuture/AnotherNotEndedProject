using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryController : MonoBehaviour
{
    public InventoryCell weaponCell, shieldCell;
    public List<Item> itemTest = new List<Item>();
    int endedRoomCount = 0;
    public int lvlCount = 0;
    public int LvlCount
    {
        get
        {
            return lvlCount;
        }
        set
        {
            GenerateTonnels();
            lvlCount = value;
        }
    }
    public int playerGoldCount
    {
        get
        {
            return _playerGoldCount; 
        }
        set
        {
            _playerGoldCount = value;
            UpdateUIControllerInfo();
        }
    }
    public int _playerGoldCount = 1;
    public int countRoom => endedRoomCount;
    private IEnumerator  Start()
    {
        UiController.Inst.ShowStoryControllerInfo(endedRoomCount, playerGoldCount);
        GameManager.Inst.OnCharacterComeInTonnel += OnCharacterComeInTonnel;

        

        yield return null;
        GenerateTonnels();
        UiController.Inst.playerInventory.AddItemToInventory(GameManager.Inst.gameItemGenerator.GenerateCopy(itemTest[0]),weaponCell);
        UiController.Inst.playerInventory.AddItemToInventory(GameManager.Inst.gameItemGenerator.GenerateCopy(itemTest[1]), shieldCell);
    }

    public void GenerateTonnels()
    {
        MapSpawner.instance.GenerateTonnelsOnThisLevl();
    }

    public void OnCharacterComeInTonnel(Tonnel tonnel, Character character)
    {
        endedRoomCount += 1;
        UpdateUIControllerInfo();
    }

    public void UpdateUIControllerInfo()
    {
        UiController.Inst.ShowStoryControllerInfo(endedRoomCount, playerGoldCount);
    }
}