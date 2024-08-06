using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeMenuItem : UIBase
{
    #region Enum
    enum Images
    {
        HomeMenuImage,
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindImage(typeof(Images));

        GetImage((int)Images.HomeMenuImage).SetSprite(GetSpriteName());

        GetComponent<Button>().SubButtonClick(OnTab, this);

        return true;
    }
    #endregion
    #region Get
    string GetSpriteName()
    {
        string name = "Icon_";
        switch ((Define.UIHomeMenuItem)m_type)
        {
            case Define.UIHomeMenuItem.Deck:
                return name + "Charts";
            case Define.UIHomeMenuItem.Home:
                return name + "Home";
            case Define.UIHomeMenuItem.Shop:
                return name + "Shop";
        }

        return name + "Cancel";
    }
    #endregion
    #region Callback
    void OnTab()
    {
        Debug.Log("[UIHomeMenuItem:OnTab] Click");

        UIHome uiHome = Managers.UI.FindUIScene<UIHome>();
        if (uiHome == null)
            return;

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        uiHome.SetHomeMenuType((Define.UIHomeMenuItem)m_type);
    }
    #endregion
}
