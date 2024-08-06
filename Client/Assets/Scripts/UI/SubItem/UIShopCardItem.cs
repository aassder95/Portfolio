using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopCardItem : UIBase
{
    #region Enum
    enum GameObjects
    {
        CardCountSlider
    }

    enum Texts
    {
        CardLevelText,
        CardBuyCountText,
        CardCountText,
        CostText,
    }

    enum Images
    {
        CardTearImage,
        CardImage,
        CostImage,
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

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetComponent<Button>().SubButtonClick(OnBuy, this);

        RefreshUI();

        return true;
    }

    public override void RefreshUI()
    {
        if (!m_isInit)
            Init();

        ShopTemplate shopTemplate = Managers.Template.GetShopTemplate(m_type);
        if (shopTemplate == null)
            return;

        m_data = Managers.Card.GetCardData(shopTemplate.TemplateId);

        GetText((int)Texts.CardLevelText).SetText(110000008, $"{m_data.Level}");
        GetText((int)Texts.CardBuyCountText).SetText($"x{shopTemplate.Count}");

        GetImage((int)Images.CardTearImage).color = Util.GetTierColor((Define.Tier)(m_data.Template.Tier));
        GetImage((int)Images.CardImage).SetSprite($"Fish/{m_data.Template.Name}");
        GetImage((int)Images.CostImage).SetSprite("Gold");
        GetText((int)Texts.CostText).SetText($"{shopTemplate.Price}");

        RefreshCardState();
        RefreshCardCnt();
    }
    #endregion
    #region InitSub
    void RefreshCardState()
    {
        bool IsPurchased = Managers.Shop.IsCardPurchased(m_type);
        GetComponent<Button>().interactable = !IsPurchased;

        if (IsPurchased)
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
    #region Callback
    void OnBuy()
    {
        Debug.Log("[UIShopCardItem:OnBuy] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        UIConfirm uiConfirm = Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.BuyCard);
        uiConfirm.TemplateId = m_type;
    }
    #endregion
}
