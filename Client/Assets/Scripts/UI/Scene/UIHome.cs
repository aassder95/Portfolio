using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : UIScene
{
    #region Enum
    enum GameObjects
    {
        HomeMenuPanel,
        Deck,
        DeckPanel,
        TabPanel,
        CardContent,
        Home,
        ChatScrollView,
        ChatContent,
        ChatInput,
        Shop,
        ShopCardPanel,
    }

    enum Texts
    {
        GoldText,
        GemText,
        SingleStageText,
        InfiniteStageText,
        ChatPlaceholder,
        ShopCardRefreshText,
    }

    enum Images
    {
        GoldImage,
        GemImage,
    }

    enum Buttons
    {
        MyInfoButton,
        SettingButton,
        SingleStageButton,
        SingleStageListButton,
        SingleStageRankButton,
        InfiniteStageButton,
        InfiniteStageRankButton,
    }
    #endregion
    #region Fields
    Define.UIHomeMenuItem m_curHomeMenuType;
    Define.Tier m_curCardTabType;
    GameObject m_chatContent;
    ScrollRect m_chatScrollRect;
    TMP_InputField m_chatInput;
    TextMeshProUGUI m_timeTxt;
    List<UICardItem> m_uiDeckCardItems = new List<UICardItem>();
    List<UICardItem> m_uiCardItems = new List<UICardItem>();
    List<UIShopCardItem> m_uiShopCardItems = new List<UIShopCardItem>();
    Coroutine m_chatUpdateCoroutine;
    #endregion
    #region Properties
    public GameObject ChatContent => m_chatContent;
    #endregion
    #region Unity
    void Update()
    {
        m_timeTxt.SetText(110000007, Util.GetTimeToUTCMidnight());
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_curHomeMenuType = Define.UIHomeMenuItem.None;
        m_curCardTabType = Define.Tier.None;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        InitObject();
        InitImage();
        InitButton();
        InitEvent();

        RefreshUI();

        m_chatUpdateCoroutine = StartCoroutine(ChatUpdateRoutine());

        SetHomeMenuType(Define.UIHomeMenuItem.Home);

        return true;
    }

    public override void RefreshUI()
    {
        InitText();
    }

    public override void Clear()
    {
        if (m_chatUpdateCoroutine != null)
        {
            StopCoroutine(m_chatUpdateCoroutine);
            m_chatUpdateCoroutine = null;
        }
    }
    #endregion
    #region InitSub
    void InitObject()
    {
        GameObject homeMenuPanel = GetObject((int)GameObjects.HomeMenuPanel);
        GameObject deckPanel = GetObject((int)GameObjects.DeckPanel);
        GameObject tabPanel = GetObject((int)GameObjects.TabPanel);
        GameObject cardContent = GetObject((int)GameObjects.CardContent);
        GameObject shopCardPanel = GetObject((int)GameObjects.ShopCardPanel);
        m_chatScrollRect = GetObject((int)GameObjects.ChatScrollView).GetComponent<ScrollRect>();
        m_chatContent = GetObject((int)GameObjects.ChatContent);
        m_chatInput = GetObject((int)GameObjects.ChatInput).GetComponent<TMP_InputField>();

        Managers.UI.CreateUISubItems<UIHomeMenuItem>(homeMenuPanel, 3);
        Managers.UI.CreateUISubItems<UICardTabItem>(tabPanel, 6);

        foreach (int id in Managers.Card.DeckIds)
        {
            UICardItem uiSubItem = Managers.UI.InstantiateUI<UICardItem>("SubItem/UICardItem", deckPanel);
            uiSubItem.Type = id;
            uiSubItem.IsDeck = true;
            m_uiDeckCardItems.Add(uiSubItem);
        }

        foreach (CardData data in Managers.Card.CardDatas)
        {
            UICardItem uiSubItem = Managers.UI.InstantiateUI<UICardItem>("SubItem/UICardItem", cardContent);
            uiSubItem.Type = data.Id;
            m_uiCardItems.Add(uiSubItem);
        }

        foreach (int id in Managers.Shop.AllCards)
        {
            UIShopCardItem uiSubItem = Managers.UI.InstantiateUI<UIShopCardItem>("SubItem/UIShopCardItem", shopCardPanel);
            uiSubItem.Type = id;
            m_uiShopCardItems.Add(uiSubItem);
        }

        m_chatInput.SubEndEdit(OnEndEdit, this);
    }

    void InitText()
    {
        GetText((int)Texts.SingleStageText).SetText(110000005);
        GetText((int)Texts.InfiniteStageText).SetText(110000006);
        GetText((int)Texts.ChatPlaceholder).SetText("...");
        m_timeTxt = GetText((int)Texts.ShopCardRefreshText);
    }

    void InitImage()
    {
        GetImage((int)Images.GoldImage).SetSprite("Gold");
        GetImage((int)Images.GemImage).SetSprite("Gem");
    }

    void InitButton()
    {
        GetButton((int)Buttons.MyInfoButton).SubButtonClick(OnMyInfo, this);
        GetButton((int)Buttons.SettingButton).SubButtonClick(OnSetting, this);
        GetButton((int)Buttons.SingleStageButton).SubButtonClick(OnSingleStage, this);
        GetButton((int)Buttons.SingleStageListButton).SubButtonClick(OnSingleStageList, this);
        GetButton((int)Buttons.SingleStageRankButton).SubButtonClick(OnSingleStageRank, this);
        GetButton((int)Buttons.InfiniteStageButton).SubButtonClick(OnInfiniteStage, this);
        GetButton((int)Buttons.InfiniteStageRankButton).SubButtonClick(OnInfiniteStageRank, this);
    }

    void InitEvent()
    {
        Managers.UI.OnNewMessage.Subscribe(_ => OnNewMessage()).AddTo(this);
        Managers.Data.Game.Gold.Subscribe(OnGoldChanged).AddTo(this);
        Managers.Data.Game.Gem.Subscribe(OnGemChanged).AddTo(this);
    }

    void RefreshHomeMenu()
    {
        switch (m_curHomeMenuType)
        {
            case Define.UIHomeMenuItem.Deck:
                RefreshUICardItem();
                break;
            case Define.UIHomeMenuItem.Shop:
                RefreshUIShopCardItem();
                break;
        }

        GetObject((int)GameObjects.Deck).SetActive(m_curHomeMenuType == Define.UIHomeMenuItem.Deck);
        GetObject((int)GameObjects.Home).SetActive(m_curHomeMenuType == Define.UIHomeMenuItem.Home);
        GetObject((int)GameObjects.Shop).SetActive(m_curHomeMenuType == Define.UIHomeMenuItem.Shop);
    }

    public void RefreshUICardItem()
    {
        m_uiCardItems.ForEach(item => item.RefreshUI());
        m_uiDeckCardItems.ForEach(item => item.RefreshUI());
    }

    public void RefreshUIDeckCardItem()
    {
        for (int i = 0; i < m_uiDeckCardItems.Count; i++)
        {
            UICardItem cardItem = m_uiDeckCardItems[i];
            if (cardItem.Type != Managers.Card.DeckIds[i])
            {
                cardItem.Type = Managers.Card.DeckIds[i];
                cardItem.RefreshUI();
            }
        }
    }

    public void RefreshUIShopCardItem()
    {
        m_uiShopCardItems.ForEach(item => item.RefreshUI());
    }

    void ClearHomeMenu()
    {
        switch (m_curHomeMenuType)
        {
            case Define.UIHomeMenuItem.Deck:
                ClearDeck();
                break;
            case Define.UIHomeMenuItem.Home:
                ClearHome();
                break;
        }
    }

    void ClearDeck()
    {
        Managers.Card.ClearSelectedDeckCardItem();

        m_curCardTabType = Define.Tier.None;
        SetCardTabType(m_curCardTabType);
    }

    void ClearHome()
    {
        ClearChat();
    }

    void ClearChat()
    {
        m_chatInput.text = "";
        m_chatInput.placeholder.gameObject.SetActive(true);
    }
    #endregion
    #region Coroutines
    IEnumerator ChatUpdateRoutine()
    {
        while (true)
        {
            Managers.Network.RequestChat();
            yield return new WaitForSeconds(10f);
        }
    }
    #endregion
    #region Set
    public void SetHomeMenuType(Define.UIHomeMenuItem type)
    {
        if (m_curHomeMenuType == type)
            return;

        ClearHomeMenu();

        m_curHomeMenuType = type;

        RefreshHomeMenu();
    }

    public void SetCardTabType(Define.Tier type)
    {
        bool isSameType = m_curCardTabType == type;
        m_curCardTabType = isSameType ? Define.Tier.None : type;

        foreach (UICardItem cardItem in m_uiCardItems)
        {
            bool isActive = isSameType || type == (Define.Tier)Managers.Card.GetCardData(cardItem.Type).Template.Tier;
            cardItem.gameObject.SetActive(isActive);
        }
    }
    #endregion
    #region Callback
    void OnGoldChanged(int value)
    {
        GetText((int)Texts.GoldText).SetText(Util.FormatNumber(value));
    }

    void OnGemChanged(int value)
    {
        GetText((int)Texts.GemText).SetText(Util.FormatNumber(value));
    }

    void OnNewMessage()
    {
        Canvas.ForceUpdateCanvases();
        m_chatScrollRect.verticalNormalizedPosition = 0.0f;
    }

    void OnEndEdit(string input)
    {
        if (string.IsNullOrEmpty(input) || !Input.GetKeyDown(KeyCode.Return))
            return;

        Debug.Log($"[UIHome:OnEndEdit] Submitted : {input}");

        ClearChat();

        Managers.Network.RequestChatSend(input);
    }

    void OnMyInfo()
    {
        Debug.Log("[UIHome:OnMyInfo] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.ShowUIPopup<UIMyInfo>();
    }

    void OnSetting()
    {
        Debug.Log("[UIHome:OnSetting] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.ShowUIPopup<UISetting>();
    }

    void OnSingleStage()
    {
        Debug.Log("[UIHome:OnSingleStage] Click");

        if (Managers.Template.GetSingleStageTemplate(Managers.Data.Game.ClearStage + 1) == null)
        {
            return;
        }

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Scene.LoadGameScene(Define.StageType.Single);
    }

    void OnSingleStageList()
    {
        Debug.Log("[UIHome:OnSingleStageList] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.ShowUIPopup<UIStage>();
    }

    void OnSingleStageRank()
    {
        Debug.Log("[UIHome:OnSingleStageRank] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Network.RequestRank(Define.StageType.Single);
    }

    void OnInfiniteStage()
    {
        Debug.Log("[UIHome:OnInfiniteStage] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Scene.LoadGameScene(Define.StageType.Infinite);
    }

    void OnInfiniteStageRank()
    {
        Debug.Log("[UIHome:OnInfiniteStageRank] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Network.RequestRank(Define.StageType.Infinite);
    }
    #endregion
}
