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
    protected bool jumpEnded = true, showEnded = true;
    public bool actionStarted;
    public List<InTonnelObject> tonnelObjects = new List<InTonnelObject>();

    protected Character player;
    #region VirtualMetods
    public virtual void StartVitrual()
    {

    }

    public virtual void PlayerIsCome(Character player)
    {
        DoActionStep(0);
        AttachActions();
        this.player = player;
    }


    public virtual void ActionOnTonnel()
    {
        actionStarted = true;
        if (!jumpEnded || !showEnded || !actionStarted)
            return;
        number_step++;
        DoActionStep(number_step);
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

            GameManager.Inst.character.JumpInTonnel(step.positionToJump.position);

            if (step.positionForCamera != null)
                GameManager.Inst.cameraManager.MoveCameraToPosition(step.positionForCamera, () => { step.DoStep(this,player); });
            else
                step.DoStep(this, player);
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
    public Enemy enemy;
    public Item giveItem;
    public bool needButtonContinue, needButtonSkip;
    public List<InTonnelObject> addNewObj = new List<InTonnelObject>();
    public List<InTonnelObject> removeObj = new List<InTonnelObject>();

    public void DoStep(Tonnel tonnel, Character player)
    {
        Action enemyForward = null;
        ShowAnimation();
        List<InTonnelObject> tonnelObjects = GetTonnelObjects(tonnel.tonnelObjects);


        if (giveItem != null)
            GiveItem();

        if (enemy != null)
        {
            enemyForward = () =>
            {
                enemy.GenerateEquip();
                FightMode.Inst.InitFightMode(tonnel, () => { enemy = null; });
                UiController.Inst.panelTonnelInfo.Hide();
                FightMode.Inst.Fight(player.controller, enemy.controller);
            };
            tonnel.ShowInfo(tommelImage, textKey, tonnelObjects, () => { enemyForward?.Invoke(); }, () => { tonnel.SkipTonnel(); });
            return;
        }


        if (needButtonContinue && needButtonSkip)
        {
            tonnel.actionStarted = false;
            tonnel.ShowInfo(tommelImage, textKey, tonnelObjects, () => { tonnel.ActionOnTonnel(); enemyForward?.Invoke(); }, () => { tonnel.SkipTonnel(); });
        }
        else
        if (needButtonContinue)
        {
            tonnel.actionStarted = false;
            tonnel.ShowInfo(tommelImage, textKey, tonnelObjects, () => { tonnel.ActionOnTonnel(); enemyForward?.Invoke(); });
        }
        else
        {
            tonnel.ShowInfo(tommelImage, textKey, tonnelObjects);
        }
    }

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