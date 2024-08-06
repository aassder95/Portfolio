using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIName : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
        NameInput,
    }

    enum Texts
    {
        TitleText,
        DescText,
        NamePlaceholder,
        ConfirmText,
    }

    enum Buttons
    {
        ConfirmButton,
    }
    #endregion
    #region Fields
    TMP_InputField m_nameInput;
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
        m_nameInput = GetObject((int)GameObjects.NameInput).GetComponent<TMP_InputField>();

        GetText((int)Texts.TitleText).SetText(110000059);
        GetText((int)Texts.DescText).SetText(110000060);
        GetText((int)Texts.ConfirmText).SetText(110000039);

        GetButton((int)Buttons.ConfirmButton).SubButtonClick(OnConfirm, this);

        return true;
    }
    #endregion
    #region UnityCallback
    public override void OnPointerClick(PointerEventData eventData)
    {

    }
    #endregion
    #region Callback
    void OnConfirm()
    {
        if (string.IsNullOrEmpty(m_nameInput.text))
        {
            Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NameIsNull);
            return;
        }

        if (m_nameInput.text.Length > 8)
        {
            Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NameIsLonger);
            return;
        }

        Debug.Log($"[UIName:OnConfirm] Submitted : {m_nameInput.text}");
        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Network.RequestCreateUser(m_nameInput.text);
    }
    #endregion
}
