using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tonnel : MonoBehaviour
{
    public TonnelType tonnelType;
    [HideInInspector]
    public Transform placeForPlayerStep;

    public Action SpawnEnded;

    public List<ActionSteps> actionSteps = new List<ActionSteps>();
    protected int number_step = 0;

    protected IEnumerator corr = null;
    protected bool jumpEnded = true, showEnded = true, actionStarted;
    public List<InTonnelObject> tonnelObjects = new List<InTonnelObject>();

    #region VirtualMetods
    public virtual void StartVitrual()
    {

    }

    public virtual void PlayerIsCome(Character player)
    {
        DoActionStep(0);
        AttachActions();
    }


    public virtual void ActionOnTonnel()
    {
        actionStarted = true;
    }

    public virtual void SkipTonnel()
    {
        DetachActions();
        MoveCameraBack();
    }

    public virtual void LeaveTonnel()
    {
        DetachActions();
        MoveCameraBack();
    }

    private void MoveCameraBack()
    {
        GameManager.Inst.cameraManager.MoveCameraToBeforePosition();
    }
    public virtual void OnCharacterJumpInPosition(Character character)
    {
        jumpEnded = true;
    }

    public virtual void TonnelInfoShowed()
    {
        showEnded = true;
    }

    #endregion
    public void DoActionStep(int number)
    {
        if (actionSteps.Count > number)
        {
            ActionSteps step = actionSteps[number];
            step.ShowAnimation();
            tonnelObjects = step.GetTonnelObjects(tonnelObjects);

            if (step.positionToJump != null && number!=0)
                GameManager.Inst.character.JumpInTonnel(step.positionToJump.position);
            
            Action showTonnelInfo = () =>
            {
                if (step.giveItem != null)
                    step.GiveItem();
                if (step.needButtonContinue && step.needButtonSkip)
                {
                    actionStarted = false;
                    ShowInfo(step.tommelImage, step.textKey, tonnelObjects, () => { ActionOnTonnel(); }, () => { SkipTonnel(); });
                }
                else
                if (step.needButtonContinue)
                {
                    actionStarted = false;
                    ShowInfo(step.tommelImage, step.textKey, tonnelObjects, () => { ActionOnTonnel(); });
                }
                else
                {
                    ShowInfo(step.tommelImage, step.textKey, tonnelObjects);
                }
            };

            if (step.positionForCamera != null)
                GameManager.Inst.cameraManager.MoveCameraToPosition(step.positionForCamera,showTonnelInfo);
            else
                showTonnelInfo?.Invoke();
        }
        else
            LeaveTonnel();
    }

    public void EndAnimSpawn()
    {
        SpawnEnded?.Invoke();
    }

    private void Awake()
    {
        if (actionSteps.Count <= 0 || actionSteps[number_step].positionToJump == null)
            placeForPlayerStep = transform;
        else
            placeForPlayerStep = actionSteps[number_step].positionToJump;
    }

    void Start()
    {
        StartVitrual();
    }

    private void AttachActions()
    {
        GameManager.Inst.OnCharacterJumpInPosition += OnCharacterJumpInPosition;
        GameManager.Inst.TonnelInfoShowed += TonnelInfoShowed;
    }

    private void DetachActions()
    {
        GameManager.Inst.OnCharacterJumpInPosition -= OnCharacterJumpInPosition;
        GameManager.Inst.TonnelInfoShowed -= TonnelInfoShowed;
        GameManager.Inst.OnCharacterFinishTonnel?.Invoke(this);
    }

    public void ShowInfo(Sprite tonnelImage, string key, List<InTonnelObject> inTonnelObjects, Action ActionOnTonnel, Action SkipTonnel)
    {
        UiController.Inst.ShowTonnelInfo(tonnelImage, key, inTonnelObjects, ActionOnTonnel, SkipTonnel);
    }

    public void ShowInfo(Sprite tonnelImage, string key, List<InTonnelObject> inTonnelObjects, Action ActionOnTonnel)
    {
        UiController.Inst.ShowTonnelInfo(tonnelImage, key, inTonnelObjects, ActionOnTonnel);
    }

    public void ShowInfo(Sprite tonnelImage, string key, List<InTonnelObject> inTonnelObjects)
    {
        UiController.Inst.ShowTonnelInfo(tonnelImage, key, inTonnelObjects);
    }

    public void AddTonnelObject(InTonnelObject newObj)
    {
        tonnelObjects.Add(newObj);
    }
}

[System.Serializable]
public class ActionSteps
{
    public Sprite tommelImage;
    public string textKey = "none";
    public Transform positionToJump, positionForCamera;
    public Animator animator;
    public string animTrigger = "Show";
    public GameObject enemy;
    public Item giveItem;
    public bool needButtonContinue, needButtonSkip;
    public List<InTonnelObject> addNewObj = new List<InTonnelObject>();
    public List<InTonnelObject> removeObj = new List<InTonnelObject>();

    public void ShowAnimation()
    {
        if (animator != null)
            animator.SetTrigger(animTrigger);
    }

    public List<InTonnelObject> GetTonnelObjects(List<InTonnelObject> haveObjects)
    {
        List<InTonnelObject> result = new List<InTonnelObject>(haveObjects);
        foreach (var obj in addNewObj)
        {
            result.Add(obj);
        }
        foreach (var obj in removeObj)
        {
            if (result.Contains(obj))
            {
                result.Remove(obj);
            }
        }
        return result;
    }

    public void GiveItem()
    {
        UiController.Inst.playerInventory.AddItemToInventory(giveItem);
    }
}