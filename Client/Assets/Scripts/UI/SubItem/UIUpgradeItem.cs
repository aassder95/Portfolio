using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIUpgradeItem : UIBase
{
    #region Enum
    enum Texts
    {
        UpgradeCostText,
        UpgradeLevelText,
    }

    enum Images
    {
        UpgradeTearImage,
        UpgradeImage,
    }
    #endregion
    #region Fields
    Button m_upgradeBtn;
    InGameCardData m_data;
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        m_data = Managers.Card.GetInGameCardData(m_type);
        m_upgradeBtn = GetComponent<Button>();

        BindText(typeof(Texts));
        BindImage(typeof(Images));

        GetImage((int)Images.UpgradeTearImage).color = Util.GetTierColor((Define.Tier)m_data.Template.Tier);
        GetImage((int)Images.UpgradeImage).SetSprite($"Fish/{m_data.Template.Name}");

        m_upgradeBtn.SubButtonClick(OnUpgrade, this);

        m_data.UpgradeLv.Subscribe(OnUpgradeLvChanged).AddTo(this);
        m_data.UpgradeCost.Subscribe(OnUpgradeCostChanged).AddTo(this);
        Managers.Game.TotalPearl.Subscribe(OnPearlCntChanged).AddTo(this);

        return true;
    }
    #endregion
    #region Callback
    void OnUpgradeLvChanged(int value)
    {
        if (value == Constant.Game.Test.MAX_UPGRADE)
            m_upgradeBtn.interactable = false;

        GetText((int)Texts.UpgradeLevelText).SetText($"{value}");
    }

    void OnUpgradeCostChanged(int value)
    {
        if (m_data.UpgradeLv.Value == Constant.Game.Test.MAX_UPGRADE)
            GetText((int)Texts.UpgradeCostText).SetText(110000058);
        else
            GetText((int)Texts.UpgradeCostText).SetText($"{value}");
    }

    void OnPearlCntChanged(int value)
    {
        m_upgradeBtn.interactable = value >= m_data.UpgradeCost.Value && m_data.UpgradeLv.Value < Constant.Game.Test.MAX_UPGRADE;
    }

    void OnUpgrade()
    {
        Debug.Log("[UIFishUpgradeItem:OnUpgrade] Click");

        if (!Managers.Game.DecreasePearl(m_data.UpgradeCost.Value))
            return;

        m_data.IncreaseUpgradeLv();
    }
    #endregion
}
