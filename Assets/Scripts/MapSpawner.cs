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

    public void SpawnTonnelForward()
    {
        TonnelType type = GameManager.Inst.GetNextRoom();
        type = UnityEngine.Random.Range(0,5) == 0? TonnelType.ThroughTunnel : TonnelType.Shop;
        type = UnityEngine.Random.Range(0, 2) == 0 ? TonnelType.Enemy : type;
        type = UnityEngine.Random.Range(0, 50) == 25 ? TonnelType.Karcer : type;
        type = UnityEngine.Random.Range(0, 10) == 0 ? TonnelType.Medic: type;
        
        SpawnTonnel(type);
    }

    public void SpawnTonnelForward(TonnelType type)
    {
        SpawnTonnel(type);
    }

    private void SpawnTonnel(TonnelType type)
    {
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
