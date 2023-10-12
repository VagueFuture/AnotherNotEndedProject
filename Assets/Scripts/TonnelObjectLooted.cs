using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TonelObjectLooted", menuName = "TonelObjects/Looted")]
public class TonnelObjectLooted : TonnelObject
{
    public int count;
    public List<Item> ItemsCanSpawnedInside = new List<Item>();
}
