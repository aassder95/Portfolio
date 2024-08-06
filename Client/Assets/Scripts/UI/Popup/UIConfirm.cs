using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIConfirm : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
    }

    enum Texts
    {
        TitleText,
        DescText,
        ConfirmCenterText,
        ConfirmLeftText,
        ConfirmRightText,
    }

    enum Buttons
    {
        CloseButton,
        ConfirmCenterButton,
        ConfirmLeftButton,
        ConfirmRightButton,
    }
    #endregion
    #region Fields
    int m_templateId;
    #endregion
    #region Properties
    public int TemplateId { get => m_templateId; set => m_templateId = value; }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        m_popupPanel = GetObject((int)GameObjects.BackgroundPanel);

        InitText();
        InitButton();

        return true;
    }
    #endregion
    #region InitSub
    void InitText()
    {
        GetText((int)Texts.TitleText).SetText(GetTitle());
        GetText((int)Texts.DescText).SetText(GetDesc());
        GetText((int)Texts.ConfirmCenterText).SetText(GetCenter());
        GetText((int)Texts.ConfirmLeftText).SetText(GetLeft());
        GetText((int)Texts.ConfirmRightText).SetText(GetRight());
    }

    void InitButton()
    {
        GetButton((int)Buttons.CloseButton).SubButtonClick(OnClose, this);
        GetButton((int)Buttons.ConfirmCenterButton).SubButtonClick(OnCenter, this);
        GetButton((int)Buttons.ConfirmLeftButton).SubButtonClick(OnLeft, this);
        GetButton((int)Buttons.ConfirmRightButton).SubButtonClick(OnRight, this);

        ButtonActive();
    }
    #endregion
    #region Control
    void LevelUp()
    {
        CardData data = Managers.Card.GetCardData(m_templateId);
        FishLevelTemplate nextFishLeveltemplate = Managers.Template.GetFishLevelTemplate(data.Level + 1);

        if (Managers.Data.IsEnoughGold(nextFishLeveltemplate.Gold))
            Managers.Network.RequestCardLevelUp(data.Id);
    }
    #endregion
    #region Utility
    void ButtonActive()
    {
        GetButton((int)Buttons.ConfirmCenterButton).gameObject.SetActive(isCenter());
        GetButton((int)Buttons.ConfirmLeftButton).gameObject.SetActive(!isCenter());
        GetButton((int)Buttons.ConfirmRightButton).gameObject.SetActive(!isCenter());
    }
    #endregion
    #region Is
    bool isCenter()
    {
        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
            case Define.UIConfirm.Lose:
            case Define.UIConfirm.Result:
            case Define.UIConfirm.GiveUp:
            case Define.UIConfirm.LevelUp:
            case Define.UIConfirm.BuyCard:
                return false;
        }

        return true;
    }
    #endregion
    #region Get
    string GetTitle()
    {
        int id = 0;
        string txt = "";

        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
            case Define.UIConfirm.WinEnd:
                id = 110000047;
                break;
            case Define.UIConfirm.Lose:
                id = 110000051;
                break;
            case Define.UIConfirm.Result:
                id = 110000068;
                break;
            default:
                id = 110000056;
                break;
        }

        return id > 0 ? Managers.Template.GetText(id) : txt;
    }

    string GetDesc()
    {
        int id = 0;
        string txt = "";

        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
            case Define.UIConfirm.WinEnd:
                txt = Util.FormatText(110000048, $"{Util.FormatNumber(Managers.Stage.Template.Gold)}");
                break;
            case Define.UIConfirm.Lose:
                id = 110000052;
                break;
            case Define.UIConfirm.Result:
                txt = Util.FormatText(110000048, $"{Util.FormatNumber(Managers.Stage.Template.Gold * Managers.Stage.Wave.Value)}");
                break;
            case Define.UIConfirm.GiveUp:
                id = 110000041;
                break;
            case Define.UIConfirm.LevelUp:
                {
                    UICardInfo uiCardInfo = Managers.UI.FindUIPopup<UICardInfo>();
                    CardData data = Managers.Card.GetCardData(uiCardInfo.Type);

                    FishLevelTemplate nextFishLeveltemplate = Managers.Template.GetFishLevelTemplate(data.Level + 1);
                    txt = Util.FormatText(110000055, $"{Util.FormatNumber(nextFishLeveltemplate.Gold)}");
                }
                break;
            case Define.UIConfirm.NameIsNull:
                id = 110000062;
                break;
            case Define.UIConfirm.NameIsLonger:
                id = 110000063;
                break;
            case Define.UIConfirm.NotFoundTemplate:
                id = 110000061;
                break;
            case Define.UIConfirm.NotEnoughGold:
                id = 110000057;
                break;
            case Define.UIConfirm.NotEnoughGem:
                id = 110000054;
                break;
            case Define.UIConfirm.NotEnoughCard:
                id = 110000037;
                break;
            case Define.UIConfirm.MaxCardLevel:
                id = 110000064;
                break;
            case Define.UIConfirm.BuyCard:
                {
                    ShopTemplate template = Managers.Template.GetShopTemplate(m_templateId);
                    txt = Util.FormatText(110000066, $"{template.Price}");
                }
                break;
        }

        return id > 0 ? Managers.Template.GetText(id) : txt;
    }

    string GetCenter()
    {
        int id = 0;
        string txt = "";

        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinEnd:
                id = 110000050;
                break;
            default:
                id = 110000039;
                break;
        }

        return id > 0 ? Managers.Template.GetText(id) : txt;
    }

    string GetLeft()
    {
        int id = 0;
        string txt = "";

        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
                id = 110000049;
                break;
            case Define.UIConfirm.Lose:
            case Define.UIConfirm.Result:
                id = 110000053;
                break;
            default:
                id = 110000039;
                break;
        }

        return id > 0 ? Managers.Template.GetText(id) : txt;
    }

    string GetRight()
    {
        int id = 0;
        string txt = "";

        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
            case Define.UIConfirm.Lose:
            case Define.UIConfirm.Result:
                id = 110000050;
                break;
            default:
                id = 110000040;
                break;
        }

        return id > 0 ? Managers.Template.GetText(id) : txt;
    }
    #endregion
    #region UnityCallback
    public override void OnPointerClick(PointerEventData eventData)
    {
        if ((Define.UIConfirm)m_type == Define.UIConfirm.WinNext ||
            (Define.UIConfirm)m_type == Define.UIConfirm.WinEnd ||
            (Define.UIConfirm)m_type == Define.UIConfirm.Lose ||
            (Define.UIConfirm)m_type == Define.UIConfirm.Result)
            return;

        base.OnPointerClick(eventData);
    }
    #endregion
    #region Callback
    void OnClose()
    {
        Debug.Log("[UIConfirm:OnClose] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
            case Define.UIConfirm.WinEnd:
            case Define.UIConfirm.Lose:
            case Define.UIConfirm.Result:
                Managers.Scene.LoadScene(Define.Scene.Home);
                break;
            default:
                Managers.UI.CloseUIPopup(this);
                break;
        }
    }

    void OnCenter()
    {
        Debug.Log("[UIConfirm:OnCenter] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinEnd:
                Managers.Scene.LoadScene(Define.Scene.Home);
                break;
            default:
                Managers.UI.CloseUIPopup(this);
                break;
        }
    }

    void OnLeft()
    {
        Debug.Log("[UIConfirm:OnLeft] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        switch ((Define.UIConfirm)m_type)
        {
            case Define.UIConfirm.WinNext:
            case Define.UIConfirm.Lose:
                Managers.Scene.LoadGameScene(Define.StageType.Single);
                break;
            case Define.UIConfirm.Result:
                Managers.Scene.LoadGameScene(Define.StageType.Infinite);
                break;
            case Define.UIConfirm.GiveUp:
                Managers.Stage.GameOver();
                break;
            case Define.UIConfirm.LevelUp:
                LevelUp();
                break;
            case Define.UIConfirm.BuyCard:
                Managers.Network.RequestShopBuyItem(m_templateId);
                break;
        }
    }

    void OnRight()
    {
        Debug.Log("[UIConfirm:OnRight] Click");

        OnClose();
    }
    #endregion
}
