using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardInfo : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
        CardCountSlider,
    }

    enum Texts
    {
        TitleText,
        CardCountText,
        LevelText,
        LevelDescText,
        AttackText,
        AttackDescText,
        AttackCountText,
        AttackCountDescText,
        AttackSpeedText,
        AttackSpeedDescText,
        AttackRangeText,
        AttackRangeDescText,
        CriticalRateText,
        CriticalRateDescText,
        CriticalDamageText,
        CriticalDamageDescText,
        LevelUpText,
    }

    enum Images
    {
        CardTearImage,
        CardImage,
    }

    enum Buttons
    {
        LevelUpButton,
        CloseButton,
    }
    #endregion
    #region Fields
    CardData m_data;
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_data = Managers.Card.GetCardData(m_type);

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));

        m_popupPanel = GetObject((int)GameObjects.BackgroundPanel);

        InitText();
        InitImage();
        InitButton();

        RefreshUI();

        return true;
    }

    public override void RefreshUI()
    {
        Slider cardCnt = GetObject((int)GameObjects.CardCountSlider).GetComponent<Slider>();

        FishLevelTemplate nextFishLeveltemplate = Managers.Template.GetFishLevelTemplate(m_data.Level + 1);

        if (nextFishLeveltemplate != null)
        {
            cardCnt.value = m_data.CardCnt / (float)nextFishLeveltemplate.CardCount;
            GetText((int)Texts.CardCountText).SetText($"{m_data.CardCnt}/{nextFishLeveltemplate.CardCount}");
            GetText((int)Texts.LevelDescText).SetText(110000013, $"{m_data.Level}", $"{nextFishLeveltemplate.Level}");
            GetText((int)Texts.AttackDescText).SetText(110000013, $"{m_data.DamageMin}~{m_data.DamageMax}", $"{m_data.NextDamageMin}~{m_data.NextDamageMax}");
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(m_data.CardCnt >= nextFishLeveltemplate.CardCount);
        }
        else
        {
            cardCnt.value = 1.0f;
            GetText((int)Texts.CardCountText).SetText($"{m_data.CardCnt}");
            GetText((int)Texts.LevelDescText).SetText($"{m_data.Level}");
            GetText((int)Texts.AttackDescText).SetText($"{m_data.DamageMin}~{m_data.DamageMax}");
            GetButton((int)Buttons.LevelUpButton).gameObject.SetActive(false);
        }
    }
    #endregion
    #region InitSub
    void InitText()
    {
        GetText((int)Texts.TitleText).SetText(110000012);
        GetText((int)Texts.LevelText).SetText(110000014);
        GetText((int)Texts.AttackText).SetText(110000015);
        GetText((int)Texts.AttackCountText).SetText(110000016);
        GetText((int)Texts.AttackCountDescText).SetText($"{m_data.Template.MissileCount}");
        GetText((int)Texts.AttackSpeedText).SetText(110000017);
        GetText((int)Texts.AttackSpeedDescText).SetText($"{m_data.Template.AttSpeed}");
        GetText((int)Texts.AttackRangeText).SetText(110000018);
        GetText((int)Texts.AttackRangeDescText).SetText($"{m_data.Template.AttRange}");
        GetText((int)Texts.CriticalRateText).SetText(110000019);
        GetText((int)Texts.CriticalRateDescText).SetText($"{m_data.Template.CriRate}%");
        GetText((int)Texts.CriticalDamageText).SetText(110000020);
        GetText((int)Texts.CriticalDamageDescText).SetText($"{m_data.Template.CriDamage}%");
        GetText((int)Texts.LevelUpText).SetText(110000021);
    }

    void InitImage()
    {
        GetImage((int)Images.CardTearImage).color = Util.GetTierColor((Define.Tier)(m_data.Template.Tier));
        GetImage((int)Images.CardImage).SetSprite($"Fish/{m_data.Template.Name}");
    }

    void InitButton()
    {
        GetButton((int)Buttons.LevelUpButton).SubButtonClick(OnLevelUp, this);
        GetButton((int)Buttons.CloseButton).SubButtonClick(OnClose, this);
    }
    #endregion
    #region Callback
    void OnLevelUp()
    {
        Debug.Log("[UICardInfo:OnLevelUp] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        UIConfirm uiConfirm = Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.LevelUp);
        uiConfirm.TemplateId = m_type;
    }

    void OnClose()
    {
        Debug.Log("[UICardInfo:OnClose] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.CloseUIPopup(this);
    }
    #endregion
}
