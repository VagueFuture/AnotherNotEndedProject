using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonObj : MonoBehaviour
{
    public Image objImage;
    public Button button;
    private TonnelObject tonnelObject;

    public void Fill(TonnelObject obj)
    {
        tonnelObject = obj;
        if (objImage == null)
            objImage = GetComponent<Image>();

        objImage.sprite = obj.objSprite;
    }
}
