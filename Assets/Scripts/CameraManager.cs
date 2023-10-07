using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class CameraManager : MonoBehaviour
{
    public Camera cam;
    private Vector3 beforePos = Vector3.zero;
    [SerializeField] private Vector3 beforeRotation = Vector3.zero;
    [SerializeField] private Vector3 beforeCameraPos = Vector3.zero;
    Tween moveCamera, rotateCamera;
    bool camInDefoultPosition = true;
    private void Awake()
    {
        cam = Camera.main;
    }
    void Start()
    {
        GameManager.Inst.OnTonnelSpawned += OnNewTonnelSpawned;
        beforeCameraPos = transform.position;
        beforeRotation = transform.rotation.eulerAngles;
    }

    public void OnNewTonnelSpawned(Tonnel newTonnel)
    {
        MoveCameraWithNewTonnel(newTonnel);
    }

    private void MoveCameraWithNewTonnel(Tonnel newTonnel)
    {
        Vector3 tonnelPos = newTonnel.transform.position;

        if (beforePos == Vector3.zero)
            beforePos = tonnelPos;

        Vector3 offset = transform.position + (tonnelPos - beforePos);
        beforeCameraPos = offset;

        StopMoving();
        moveCamera = transform.DOMove(offset, 1)
            .OnComplete(() => { beforePos = tonnelPos; });
    }

    public void MoveCameraToPosition(Transform place, Action onMoveEnd)
    {
        StopAllMove();
        camInDefoultPosition = false;
        moveCamera = transform.DOMove(place.position, 1).OnComplete(() => { onMoveEnd?.Invoke(); });
        rotateCamera = transform.DORotate(place.rotation.eulerAngles, 1);
    }

    [ContextMenu("MoveBack")]
    public void MoveCameraToBeforePosition(Action onEnd = null)
    {
        if (camInDefoultPosition)
            return;
        camInDefoultPosition = true;
        StopAllMove();
        moveCamera = transform.DOMove(beforeCameraPos, 1).OnComplete(() => { onEnd?.Invoke(); });
        rotateCamera = transform.DORotate(beforeRotation, 1);
    }

    private void StopAllMove()
    {
        StopMoving();
        StopRotating();
    }
    private void StopMoving()
    {
        moveCamera.Kill();
    }
    private void StopRotating()
    {
        rotateCamera.Kill();
    }
}
