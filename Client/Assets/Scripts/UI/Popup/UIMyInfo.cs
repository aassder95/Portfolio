using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMyInfo : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
    }

    enum Texts
    {
        TitleText,
        NameText,
        NameDescText,
        VersionText,
        VersionDescText,
        UuidText,
        UuidDescText,
    }

    enum Buttons
    {
        CloseButton,
    }
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

        GetText((int)Texts.TitleText).SetText(110000022);
        GetText((int)Texts.NameText).SetText(110000023);
        GetText((int)Texts.NameDescText).SetText(Managers.Data.Game.Name);
        GetText((int)Texts.VersionText).SetText(110000024);
        GetText((int)Texts.VersionDescText).SetText(Application.version);
        GetText((int)Texts.UuidText).SetText(110000025);
        GetText((int)Texts.UuidDescText).SetText(Managers.Data.Setting.UUID);

        GetButton((int)Buttons.CloseButton).SubButtonClick(OnClose, this);

        return true;
    }
    #endregion
    #region Callback
    void OnClose()
    {
        Debug.Log("[UIMyInfo:OnClose] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.CloseUIPopup(this);
    }
    #endregion
}
