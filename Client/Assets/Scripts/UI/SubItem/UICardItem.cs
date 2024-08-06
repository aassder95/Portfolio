using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UICardItem : UIBase
{
    #region Enum
    enum GameObjects
    {
        CardCountSlider
    }

    enum Texts
    {
        CardLevelText,
        CardCountText,
    }

    enum Images
    {
        CardTearImage,
        CardImage,
    }
    #endregion
    #region Fields
    bool m_isDeck = false;
    bool m_isSelected = false;
    Outline m_outline;
    CardData m_data;
    #endregion
    #region Properties
    public bool IsDeck { get => m_isDeck; set => m_isDeck = value; }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_outline = GetComponent<Outline>();

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetComponent<Button>().SubButtonClick(OnInfo, this);

        RefreshUI();

        return true;
    }

    public override void RefreshUI()
    {
        if (!m_isInit)
            Init();

        m_data = Managers.Card.GetCardData(m_type);

        GetText((int)Texts.CardLevelText).SetText(110000008, $"{m_data.Level}");

        GetImage((int)Images.CardTearImage).color = Util.GetTierColor((Define.Tier)(m_data.Template.Tier));
        GetImage((int)Images.CardImage).SetSprite($"Fish/{m_data.Template.Name}");

        RefreshCardState();
        RefreshCardCnt();
    }
    #endregion
    #region InitSub
    void RefreshCardState()
    {
        bool isOpen = m_data.Level > 0;
        GetComponent<Button>().interactable = isOpen;

        if (!isOpen)
        {
            Color tearImageColor = GetImage((int)Images.CardTearImage).color;
            Color imageColor = GetImage((int)Images.CardImage).color;
            tearImageColor.a = 0.5f;
            imageColor.a = 0.5f;

            GetImage((int)Images.CardTearImage).color = tearImageColor;
            GetImage((int)Images.CardImage).color = imageColor;
        }
    }

    void RefreshCardCnt()
    {
        Slider cardCnt = GetObject((int)GameObjects.CardCountSlider).GetComponent<Slider>();

        FishLevelTemplate nextFishLeveltemplate = Managers.Template.GetFishLevelTemplate(m_data.Level + 1);
        if (nextFishLeveltemplate != null)
        {
            cardCnt.value = m_data.CardCnt / (float)nextFishLeveltemplate.CardCount;
            GetText((int)Texts.CardCountText).SetText($"{m_data.CardCnt}/{nextFishLeveltemplate.CardCount}");
        }
        else
        {
            cardCnt.value = 1.0f;
            GetText((int)Texts.CardCountText).SetText($"{m_data.CardCnt}");
        }
    }
    #endregion
    #region Set
    public void SetSelected(bool isSelected)
    {
        m_isSelected = isSelected;
        m_outline.effectColor = m_isSelected ? Color.red : Color.clear;
    }
    #endregion
    #region Callback
    void OnInfo()
    {
        Debug.Log("[UICardItem:OnInfo] Click");

        if (m_isDeck)
        {
            SetSelected(!m_isSelected);
            Managers.Sound.Play(Define.Sound.Sfx, "Click");

            if (m_isSelected)
                Managers.Card.SetSelectedDeckCardItem(this);
            else
                Managers.Card.SetSelectedDeckCardItem(null);
        }
        else
        {
            if (Managers.Card.SelectedDeckCardItem != null)
            {
                if (Managers.Card.DeckIds.Contains(m_type))
                    return;

                Managers.Sound.Play(Define.Sound.Sfx, "Click");
                Managers.Network.RequestDeckChange(Managers.Card.SelectedDeckCardItem.Type, m_type);
            }
            else
            {
                Managers.Sound.Play(Define.Sound.Sfx, "Click");
                UICardInfo uiCardInfo = Managers.UI.ShowUIPopup<UICardInfo>();
                uiCardInfo.Type = m_type;
            }
        }
    }
    #endregion
}
