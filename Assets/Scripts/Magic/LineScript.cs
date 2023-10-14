using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LineScript : MonoBehaviour
{
    [SerializeField] DirectionFingerCapture dFC;
    [SerializeField] GameObject drowableSprite;
    private Vector3 mousePos;
    private GameObject particle;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        dFC.OnMouseDrag += OnMouseDrag;
        dFC.OnMouseUp += OnMouseUp;
        dFC.OnMouseDown += OnMouseDown;
        dFC.OnDirectionChanged += OnDirectionChanged;
    }

    void OnMouseDown(Vector3 position)
    {

        position.z = 1f;
        mousePos = cam.ScreenToWorldPoint(position);
        particle = Instantiate(drowableSprite, mousePos, Quaternion.identity);
    }

    private void OnMouseUp(Vector3 position)
    {
        position.z = 1f;
        mousePos = cam.ScreenToWorldPoint(position);
        Destroy(particle);
    }

    private void OnMouseDrag(Vector3 position)
    {
        position.z = 1f;
        mousePos = cam.ScreenToWorldPoint(position);
        particle.transform.DOMove(mousePos, 0.05f); 
    }

    public void OnDirectionChanged()
    {

    }

 
}