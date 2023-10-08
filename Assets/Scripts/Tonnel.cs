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
    public int number_step = 0;

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
        DoActionStep(number_step);
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

            if (number != 0)
                if (step.positionToJump != null)
                    GameManager.Inst.character.JumpInTonnel(step.positionToJump);

            if (step.positionForCamera != null)
                GameManager.Inst.cameraManager.MoveCameraToPosition(step.positionForCamera, () => { step.DoStep(this, player); });
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

    public void AddTonnelObject(InTonnelObject newObj)
    {
        tonnelObjects.Add(newObj);
    }

    public void SetEnemy(Enemy enemy, Tonnel tonnel, Character player)
    {
        if (enemy == null) return;

        TonnelInfo tonnelInfo = enemy.Init();
        tonnelInfo.ActivteTonnel = () =>
        {
            InitFightMode(enemy, tonnel, player, GameManager.Inst.ShowTonnelInfosStack);
        };
        tonnelInfo.SkipTonnel = () =>
        {
            GameManager.Inst.ShowTonnelInfosStack();
        };

        GameManager.Inst.AddTonnelInfoInSteck(tonnelInfo);
    }

    private void InitFightMode(Enemy enemy, Tonnel tonnel, Character player, Action onFightEnd)
    {
        FightMode.Inst.InitFightMode(tonnel, onFightEnd);
        UiController.Inst.panelTonnelInfo.Hide();
        FightMode.Inst.Fight(player.controller, enemy.controller);
    }
}

[System.Serializable]
public class ActionSteps
{
    public Sprite tonnelImage;
    public string textKey = "none";
    public Transform positionToJump, positionForCamera;
    public Animator animator;
    public string animTrigger = "Show";
    public CustomAction customAction;
    public Enemy enemy;
    public Item giveItem;
    public bool needButtonSkip;
    public List<InTonnelObject> addNewObj = new List<InTonnelObject>();
    public List<InTonnelObject> removeObj = new List<InTonnelObject>();

    public void DoStep(Tonnel tonnel, Character player)
    {
        PushTonnelInfo(tonnel);

        ShowAnimation();

        GiveItem();

        DoCustonAction(tonnel, player);

        tonnel.SetEnemy(enemy, tonnel, player);

        GameManager.Inst.ShowTonnelInfosStack();
    }

    public void PushTonnelInfo(Tonnel tonnel)
    {
        TonnelInfo tonnelInfo = new TonnelInfo()
        {
            tonnelImage = this.tonnelImage,
            tonnelText = this.textKey
        };
        List<InTonnelObject> tonnelObjects = GetTonnelObjects(tonnel.tonnelObjects);
        tonnelInfo.inTonnelObjects = tonnelObjects;

        if (tonnel.number_step <= tonnel.actionSteps.Count)
            tonnelInfo.ActivteTonnel = () =>
            {
                tonnel.ActionOnTonnel();
                UiController.Inst.panelTonnelInfo.Hide();
            };

        if (needButtonSkip)
            tonnelInfo.SkipTonnel = tonnel.SkipTonnel;

        GameManager.Inst.AddTonnelInfoInSteck(tonnelInfo);
    }

    private void ShowAnimation()
    {
        if (animator != null)
            animator.SetTrigger(animTrigger);
    }

    private List<InTonnelObject> GetTonnelObjects(List<InTonnelObject> haveObjects)
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

    private void GiveItem()
    {
        if (giveItem != null)
            UiController.Inst.playerInventory.AddItemToInventory(giveItem);
    }

    private void DoCustonAction(Tonnel tonnel, Character player)
    {
        if (customAction == null) return;
        TonnelInfo tonnelInfo = customAction.Init(tonnel, player, GameManager.Inst.ShowTonnelInfosStack);
        GameManager.Inst.AddTonnelInfoInSteck(tonnelInfo);
    }
}