using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UiController : MonoBehaviour
{
    private static UiController _Instance;
    public static UiController Inst
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<UiController>();
            return _Instance;
        }
    }

    public GameObject buttonForward;

    public UiStoryController uiStoryController;

    public PanelTonnelInfo panelTonnelInfo;

    public PanelFighPhase panelFighPhase;

    public PanelStatsInfo panelStatsInfo;

    public PanelShop panelShop;

    public ItemInfoPanel panelItemInfo;

    public LootingPanel lootingPanel;

    public Inventory playerInventory;

    public PanelHaveTonnels panelHaveTonnels;

    public void OnPressButtonForward()
    {
        panelTonnelInfo.Hide();
        lootingPanel.ResetLoot();
        //MapSpawner.instance.SpawnTonnelForward();
        buttonForward.SetActive(false);
    }

    public void NextRoomAccess()
    {
        buttonForward.SetActive(true);
        panelHaveTonnels.ShowPanel();
        panelTonnelInfo.Hide();
    }

    public void OpenHidePanelItemInfo()
    {
        panelItemInfo.OpenPanel();
    }

    public void HidePanellooting()
    {
        if (lootingPanel.open)
        {
            lootingPanel.HidePanel();
        }
    }

    public void ShowStoryControllerInfo(int endRoomCount, int goldCount)
    {
        uiStoryController.FillPanel(endRoomCount, goldCount);
    }

    #region TonnelInfo
    public void ShowTonnelInfo(TonnelInfo tonnelInfo)
    {
        panelTonnelInfo.FillPanel(tonnelInfo);
        panelTonnelInfo.Show();
    }
    #endregion
    public void OpenStatsInfoPanel()
    {
        panelStatsInfo.ShowPanel(GameManager.Inst.character.controller.stats);
    }

    public void CloseStatsInfoPanel()
    {
        panelStatsInfo.HidePanel();
    }

    public void ClosePanelShop()
    {
        panelShop.ClosePanel();
        buttonForward.SetActive(true);
    }

    public void PressDealTradeButton()
    {
        panelShop.PressButtonTrade();
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GameManager.Inst.OnScreenPress?.Invoke();
    }
}

public class TonnelInfo
{
    public Sprite tonnelImage;
    public string tonnelText;
    public List<InTonnelObject> inTonnelObjects = new List<InTonnelObject>();

    public bool showed;

    public Action ActivteTonnel = null;
    public Action SkipTonnel = null;

    public int priority = 0;
}

[System.Serializable]
public class PanelTonnelInfo
{
    [SerializeField] private RectTransform selfPanel;
    [SerializeField] private Image tonnelImage;
    [SerializeField] private Text tonnelText;
    [SerializeField] private Button buttonAtivatinTonnel, buttonSkipTonnel;
    [SerializeField] private Transform tonnelObjHolder;
    [SerializeField] private ButtonObj buttonObjPref;

    private bool showed;
    private List<ButtonObj> buttonObjs = new List<ButtonObj>();
    public void FillPanel(TonnelInfo tonnelInfo)
    {
        FillInfoPanel(tonnelInfo.tonnelImage, tonnelInfo.tonnelText);
        FillTonnelObjectsPanel(tonnelInfo.inTonnelObjects);
        buttonAtivatinTonnel.onClick.RemoveAllListeners();
        buttonSkipTonnel.onClick.RemoveAllListeners();

        buttonAtivatinTonnel.gameObject.SetActive(false);
        buttonSkipTonnel.gameObject.SetActive(false);

        if (tonnelInfo.ActivteTonnel != null)
        {
            buttonAtivatinTonnel.onClick.AddListener(() => { tonnelInfo.ActivteTonnel?.Invoke(); });
            buttonAtivatinTonnel.gameObject.SetActive(true);
        }
        if (tonnelInfo.SkipTonnel != null)
        {
            buttonSkipTonnel.onClick.AddListener(() => { tonnelInfo.SkipTonnel?.Invoke(); });
            buttonSkipTonnel.gameObject.SetActive(true);
        }
        
    }

    private void FillInfoPanel(Sprite spr, string textKey)
    {
        tonnelImage.sprite = spr;
        tonnelText.text = DialogSystem.GetDialogFromKey(textKey);
    }

    private void FillTonnelObjectsPanel(List<InTonnelObject> inTonnelObjects)
    {
        ResetLootObjects();
        if (inTonnelObjects.Count > 0)
        {
            foreach (var tonnelObj in inTonnelObjects)
            {
                ButtonObj newButton = GameObject.Instantiate(buttonObjPref, tonnelObjHolder);
                newButton.Fill(tonnelObj.myObject);
                buttonObjs.Add(newButton);
                newButton.button.onClick.AddListener(() =>
                {
                    if(tonnelObj.myObject is TonnelObjectLooted)
                        UiController.Inst.lootingPanel.ShowPanel((TonnelObjectLooted)tonnelObj.myObject);
                });
            }
        }
    }

    public void ResetLootObjects()
    {
        foreach (var but in buttonObjs)
            GameObject.Destroy(but.gameObject);

        buttonObjs.Clear();
    }
    public void Show()
    {
        if (showed)
            Hide(() => { ShowAnim(); });
        else
            ShowAnim();
    }

    private void ShowAnim()
    {
        showed = true;
        selfPanel.DOAnchorPosX(50, 1).OnComplete(() => { GameManager.Inst.TonnelInfoShowed?.Invoke(); });
    }

    public void Hide(Action onComplited = null)
    {
        showed = false;
        selfPanel.DOAnchorPosX(-50, 1).OnComplete(() => { onComplited?.Invoke(); });
    }
}
[System.Serializable]
public class PanelFighPhase
{
    [SerializeField] private GameObject panelSelf;
    [SerializeField] private Image checkCircl;
    [SerializeField] private Image victoryZone, critZone;
    [SerializeField] private Transform rotatingArrow;
    public Button brawlButton;
    [SerializeField] Text counterText;
    public GameObject arrowLeftDir, arrowRightDir;
    public void Init()
    {
        brawlButton.gameObject.SetActive(true);
        counterText.gameObject.SetActive(false);
        RotateArrow(0, false);
        arrowLeftDir.SetActive(false);
        arrowRightDir.SetActive(false);
    }
    public void FillData(Vector3 winZone, Vector3 critZone, FightMode.CheckType checkType)
    {
        float rotation = Mathf.Lerp(0, 360, winZone.x);
        victoryZone.transform.localRotation = Quaternion.Euler(0, 0, -rotation);
        rotation = Mathf.Lerp(0, 360, critZone.x);
        this.critZone.transform.localRotation = Quaternion.Euler(0, 0, -rotation);

        victoryZone.fillAmount = winZone.y - winZone.x;
        this.critZone.fillAmount = critZone.y - critZone.x;

        panelSelf.SetActive(true);
        this.critZone.gameObject.SetActive(false);

        arrowLeftDir.SetActive(false);
        arrowRightDir.SetActive(false);
        if (checkType == FightMode.CheckType.Attak)
        {
            arrowRightDir.SetActive(true);
            this.critZone.gameObject.SetActive(true);
        }
        else
        {
            arrowLeftDir.SetActive(true);
        }
        }

    public void RotateArrow(float euler, bool reverse)
    {
        float rotation = Mathf.Lerp(0, 360, euler);
        if (reverse)
            rotation *= -1;
        rotatingArrow.localRotation = Quaternion.Euler(0, 0, -rotation);
    }

    public void Hide()
    {
        panelSelf.SetActive(false);
    }

    public void ShowCounter(int count, Action onEnd)
    {
        counterText.transform.localScale = Vector3.zero;
        counterText.gameObject.SetActive(true);
        if (count > 0)
        {
            counterText.text = count + "";
            counterText.transform.DOScale(Vector3.one, 1).OnComplete(() =>
            {
                count--;
                ShowCounter(count, onEnd);
            });
        }
        else
        {
            counterText.gameObject.SetActive(false);
            onEnd?.Invoke();
        }
    }
}
[System.Serializable]
public class PanelStatsInfo
{
    [SerializeField]
    public GameObject panelSelf;
    [SerializeField]
    public Text strengthText, defenceText, agilityText, luckText, healthText;

    public void ShowPanel(Stats stats)
    {
        FillData(stats);
        panelSelf.SetActive(true);
    }

    public void HidePanel()
    {
        panelSelf.SetActive(false);
    }

    public void FillData(Stats stats)
    {
        strengthText.text = stats.strength + "";
        defenceText.text = stats.defendce + "";
        agilityText.text = stats.agility + "";
        luckText.text = stats.luck + "";
        healthText.text = stats.Health + "";
    }
}
[System.Serializable]
public class PanelShop
{
    public GameObject panelSelf;
    private TradeMode tradeMode;
    public Inventory playerCells;
    public Inventory traderCells;
    public Inventory sellByCells;
    public bool open;
    public Text playerExchange, sellerExchange, needPay, playerGoldCount;

    public void ShowPanel()
    {
        tradeMode = new TradeMode();
        tradeMode.Init(playerCells, traderCells, sellByCells);
        tradeMode.CalculateDataUpdate = UpdateUiData;
        panelSelf.SetActive(true);
        UpdateUiData(0, 0, 0);
        open = true;
    }

    public void PressButtonTrade()
    {
        tradeMode.PressTreadButton();
    }

    public void UpdateUiData(int playerExchange, int sellerExchange, int needPay)
    {
        this.playerExchange.text = playerExchange + "";
        this.sellerExchange.text = sellerExchange + "";
        this.needPay.text = -needPay + "";
        this.playerGoldCount.text = GameManager.Inst.coutnPlayerGold + "";
    }

    public void ClosePanel()
    {
        panelSelf.SetActive(false);
        open = false;
        Leave();
    }

    public void Leave()
    {
        tradeMode.Leave();
        GameManager.Inst.cameraManager.MoveCameraToBeforePosition();
    }
}
[System.Serializable]
public class ItemInfoPanel
{
    public GameObject panelSelf;
    public GameObject pageWeapon, pageItem, pageCanUse;
    public Image itemImage;
    public Text itemDescription,
        strenght,
        defence,
        attakSpeed,
        checkCount,
        price,
        weaponEffect,
        canUseEffect;
    public Button useButton, takeButton;
    private bool open;
    private UiController uiController => UiController.Inst;

    public void OpenPanel()
    {
        open = !open;
        panelSelf.SetActive(open);
    }
    public void FillPanel(ItemInInventory iteminInventory)
    {
        pageWeapon.SetActive(false);
        pageItem.SetActive(false);
        pageCanUse.SetActive(false);

        takeButton.gameObject.SetActive(false);

        Item item = iteminInventory.item;

        itemImage.sprite = item.imageOnInventory;
        price.text = item.Price + "";
        itemDescription.text = item.name + "Need Add Description";

        if (item is ItemWeapon)
        {
            pageWeapon.SetActive(true);
            ItemWeapon weapon = (ItemWeapon)item;
            strenght.text = "str " + weapon.stength;
            defence.text = "df " + weapon.defendce + "";
            attakSpeed.text = "as " + weapon.attackSpeed + "";
            checkCount.text = "chk " + weapon.checkCount + "";
            weaponEffect.text = "efc " + weapon.magickEffect.status.ToString();
        }
        else
        if (item is ItemCanUsed)
        {
            pageCanUse.SetActive(true);
            ItemCanUsed itemCanUse = (ItemCanUsed)item;
            canUseEffect.text = "efc " + itemCanUse.effect.status.ToString();
            useButton.gameObject.SetActive(item.tredeStatus == TredeStatus.PlayerOwner && !uiController.panelShop.open);
            useButton.onClick.RemoveAllListeners();
            useButton.onClick.AddListener(() =>
            {
                GameManager.Inst.character.controller.ApplyEffect(itemCanUse.effect);
                uiController.playerInventory.RemoveItemFromInventory(item);
                uiController.playerInventory.UpdateInventory();
                OpenPanel();
            });
        }
        else
        {
            pageItem.SetActive(true);
        }
        CheckItemHolder(iteminInventory);
    }

    public void CheckItemHolder(ItemInInventory item)
    {
        if (item.ownerInventory is LootInventory)
        {
            takeButton.gameObject.SetActive(true);
            takeButton.onClick.RemoveAllListeners();
            useButton.gameObject.SetActive(false);
            takeButton.onClick.AddListener(() =>
            {
                uiController.playerInventory.AddItemToInventory(item.item);
                uiController.lootingPanel.RemoveItem(item.item);
                OpenPanel();
            });
        }
    }
}
[System.Serializable]
public class LootingPanel
{
    public Inventory lootingCells;
    public bool open;
    public GameObject panelSelf;
    private LootMode lootMode = new LootMode();

    public void ShowPanel(TonnelObjectLooted obj)
    {
        lootMode.Init(lootingCells, obj);
        open = true;
        panelSelf.SetActive(true);
    }


    public void HidePanel()
    {
        open = false;
        panelSelf.SetActive(false);
    }

    public void ResetLoot()
    {
        lootMode.ResetLootMode();
    }

    public void RemoveItem(Item item)
    {
        lootMode.RemoveItemFromLoot(item);
    }
}
[System.Serializable]
public class UiStoryController
{
    [SerializeField]
    private Text endRoomCount, goldCount;

    public void FillPanel(int endRoomCount, int goldCount)
    {
        this.endRoomCount.text = endRoomCount + "";
        this.goldCount.text = goldCount + "";
    }
}
[System.Serializable]
public class PanelHaveTonnels
{
    [SerializeField] private GameObject panelSelf;
    [SerializeField] private BtnSelectionTonnel btnPrefab;
    [SerializeField] private Transform content;
    List<BtnSelectionTonnel> btnSelectionTonnels = new List<BtnSelectionTonnel>();

    public void ShowPanel()
    {
        panelSelf.SetActive(true);
    }

    public void HidePanel()
    {
        panelSelf.SetActive(false);
    }

    public void ShowTonnelButtons(List<Tonnel> tonnels)
    {
        DropAllButtons();
        tonnels.Reverse();
        int visibleSost = GameManager.Inst.character.controller.stats.vision;
        int j = 0;
        foreach(var tonnel in tonnels)
        {
            if (j >= visibleSost)
                break;
            BtnSelectionTonnel btn = GameObject.Instantiate(btnPrefab, content);
            btn.Init(tonnel, ClickOnButton);
            btnSelectionTonnels.Add(btn);
            j++;
        }
    }

    private void DropAllButtons()
    {
        foreach (var btn in btnSelectionTonnels)
            GameObject.Destroy(btn.gameObject);

        btnSelectionTonnels.Clear();
    }

    private void ClickOnButton(Tonnel tonnel)
    {
        MapSpawner.instance.SpawnTonnelForward(tonnel);
        HidePanel();
    }
}