using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InTonnelObject : MonoBehaviour
{
    public Tonnel parentTonnel;
    public TonnelObject myObject;

    private void Start()
    {
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
