using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private string textKey;
    public MyCharacterController controller;

    public TonnelInfo Init()
    {
        GenerateEquip();
        TonnelInfo tonnelInfo = new TonnelInfo()
        {
            tonnelImage = sprite,
            tonnelText = textKey
        };
        return tonnelInfo;
    }

    public void GenerateEquip()
    {
        int countW = GameManager.Inst.allTypeWeaponInGame.Count;
        int r = Random.Range(0, countW);
        controller.EquipedItemWeapon = (ItemWeapon)GameManager.Inst.gameItemGenerator.GenerateItem(GameManager.Inst.allTypeWeaponInGame[r]);
        r = Random.Range(0, countW);
        controller.EquipedItemShield = (ItemWeapon)GameManager.Inst.gameItemGenerator.GenerateItem(GameManager.Inst.allTypeWeaponInGame[r]);
        controller.stats.Health = Random.Range(1, GameManager.Inst.storyController.countRoom)+r;

        r = Random.Range(0, GameManager.Inst.storyController.countRoom);
        controller.stats.agility = r;
    }
}
