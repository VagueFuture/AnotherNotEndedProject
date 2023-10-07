using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expiriments : MonoBehaviour
{
    public Image img;
    public float rotate = 0; 
    void Update()
    {
        //img.transform.localRotation = Quaternion.Euler(0, 0, rotate);
    }

    [ContextMenu("asd")]
    public void asd()
    {
        //img.transform.Rotate(Vector3.forward, Mathf.Lerp(0,360,0.5f));
        
    }
}
