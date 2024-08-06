using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardTabItem : UIBase
{
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        GetComponent<Button>().SubButtonClick(OnTab, this);
        GetComponent<Image>().color = Util.GetTierColor((Define.Tier)m_type);

        return true;
    }
    #endregion
    #region Callback
    void OnTab()
    {
        Debug.Log("[UICardTabItem:OnTab] Click");

        UIHome uiHome = Managers.UI.FindUIScene<UIHome>();
        if (uiHome == null)
            return;

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        uiHome.SetCardTabType((Define.Tier)m_type);
    }
    #endregion
}
