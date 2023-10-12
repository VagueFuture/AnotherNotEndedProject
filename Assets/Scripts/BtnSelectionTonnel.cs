using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnSelectionTonnel : MonoBehaviour
{
    [SerializeField] private Image tonnelImage;
    [SerializeField] private Text tonnelName;
    [SerializeField] private Button button;
    private Tonnel tonnelSelf;

    public void Init(Tonnel tonnel, Action<Tonnel> onPress)
    {
        tonnelImage.sprite = tonnel.tonnelImage;
        tonnelName.text = tonnel.tonnelType.ToString();
        tonnelSelf = tonnel;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(()=> { onPress?.Invoke(tonnel); });
    }
}
