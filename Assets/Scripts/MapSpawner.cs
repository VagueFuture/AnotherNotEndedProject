using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public static MapSpawner instance;

    public List<Tonnel> mapPartsVariants = new List<Tonnel>();
    Vector3 lastPosition = Vector3.zero;
    private List<Tonnel> generatedMapParts = new List<Tonnel>();
    int countRoomSpawned = 0, countRoomCashs = 5, countDestroyed = 0;

    int countRoomInLevl = 5;
    private List<Tonnel> tonnelsOnThisLvl = new List<Tonnel>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
    }

    public void GenerateTonnelsOnThisLevl()
    {
        tonnelsOnThisLvl.Clear();
        tonnelsOnThisLvl.Add(GetTonnel(TonnelType.Stairs));
        for(int i = 0; i < countRoomInLevl; i++)
        {
            tonnelsOnThisLvl.Add(GenerateTonnel());
        }

        GameManager.Inst.OnTonnelInThisLvlUpdate?.Invoke(tonnelsOnThisLvl);
        countRoomInLevl = (int)((float)countRoomInLevl * 0.5f + countRoomInLevl);
    } 

    private Tonnel GenerateTonnel()
    {
        TonnelType type = GameManager.Inst.GetNextRoom();
        type = UnityEngine.Random.Range(0,5) == 0? TonnelType.ThroughTunnel : TonnelType.Shop;
        type = UnityEngine.Random.Range(0, 5) == 0 ? TonnelType.Enemy : type;
        type = UnityEngine.Random.Range(0, 50) == 25 ? TonnelType.Karcer : type;
        type = UnityEngine.Random.Range(0, 10) == 0 ? TonnelType.Medic: type;
        type = UnityEngine.Random.Range(0, 10) == 0 ? TonnelType.Graves : type;

        return GetTonnel(type);
    }

    public void SpawnTonnelForward(Tonnel tonnel)
    {
        SpawnTonnel(tonnel.tonnelType);
        RemoveTonnelFromThisLvl(tonnel);
    }

    private void RemoveTonnelFromThisLvl(Tonnel tonnel)
    {
        if (tonnelsOnThisLvl.Contains(tonnel))
        {
            tonnelsOnThisLvl.Remove(tonnel);
        }
        GameManager.Inst.OnTonnelInThisLvlUpdate?.Invoke(tonnelsOnThisLvl);
    }

    private Tonnel GetTonnel(TonnelType type)
    {
        Tonnel newTonnel = mapPartsVariants.Find(x => x.tonnelType == type);
        return newTonnel;
    }

    private void SpawnTonnel(TonnelType type)
    {
        lastPosition.y = GameManager.Inst.storyController.LvlCount;
        lastPosition.x += 1;
        
        Tonnel newTonnel = Instantiate(mapPartsVariants.Find(x => x.tonnelType == type).gameObject, lastPosition, new Quaternion(), transform).GetComponent<Tonnel>();
        generatedMapParts.Add(newTonnel);
        newTonnel.SpawnEnded += () =>
        GameManager.Inst.OnTonnelSpawned?.Invoke(newTonnel);
        if (countRoomSpawned >= countRoomCashs)
        {
            Destroy(generatedMapParts[0+countDestroyed].gameObject);
            countDestroyed++;
        }
        countRoomSpawned++;
    }
}
