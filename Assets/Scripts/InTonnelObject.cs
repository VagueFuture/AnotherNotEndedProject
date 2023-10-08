using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InTonnelObject : MonoBehaviour
{
    public Tonnel parentTonnel;
    public TonnelObject myObject;
    public bool spawnOnStart = true; 
    private void Start()
    {
        if (!spawnOnStart) return;

        int r = Random.Range(0, 2);
        if(r == 1)
        {
            Destroy(gameObject);
            return;
        }
        parentTonnel = GetComponentInParent<Tonnel>();
        parentTonnel.AddTonnelObject(this);
    }
}
