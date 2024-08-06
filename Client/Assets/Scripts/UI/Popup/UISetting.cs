using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
    }

    enum Texts
    {
        TitleText,
        SoundText,
        BgmText,
        SfxText,
        GameText,
        DamageText,
        EffectText,
        LanguageText,
        KorText,
        EngText,
        GiveUpText,
    }

    enum Buttons
    {
        CloseButton,
        GiveUpButton,
    }

    enum Toggles
    {
        BgmToggle,
        SfxToggle,
        DamageToggle,
        EffectToggle,
        KorToggle,
        EngToggle,
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        gameObject.SetActive(false);

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindToggle(typeof(Toggles));

        m_popupPanel = GetObject((int)GameObjects.BackgroundPanel);

        InitButton();
        InitToggle();

        RefreshUI();

        gameObject.SetActive(true);

        return true;
    }

    public override void RefreshUI()
    {
        InitText();
    }
    #endregion
    #region InitSub
    void InitText()
    {
        GetText((int)Texts.TitleText).SetText(110000027);
        GetText((int)Texts.SoundText).SetText(110000028);
        GetText((int)Texts.BgmText).SetText(110000029);
        GetText((int)Texts.SfxText).SetText(110000030);
        GetText((int)Texts.GameText).SetText(110000031);
        GetText((int)Texts.DamageText).SetText(110000032);
        GetText((int)Texts.EffectText).SetText(110000033);
        GetText((int)Texts.LanguageText).SetText(110000034);
        GetText((int)Texts.KorText).SetText(110000035);
        GetText((int)Texts.EngText).SetText(110000036);
        GetText((int)Texts.GiveUpText).SetText(110000065);
    }

    void InitButton()
    {
        GetButton((int)Buttons.CloseButton).SubButtonClick(OnClose, this);

        Button giveUpBtn = GetButton((int)Buttons.GiveUpButton);
        giveUpBtn.SubButtonClick(OnGiveUp, this);
        if (Managers.Scene.CurScene.Type == Define.Scene.Home)
            giveUpBtn.gameObject.SetActive(false);
    }

    void InitToggle()
    {
        GetToggle((int)Toggles.BgmToggle).SubToggleClick(OnBgm, this, Managers.Data.Setting.IsBgm.Value);
        GetToggle((int)Toggles.SfxToggle).SubToggleClick(OnSfx, this, Managers.Data.Setting.IsSfx.Value);
        GetToggle((int)Toggles.DamageToggle).SubToggleClick(OnDamage, this, Managers.Data.Setting.IsDamage.Value);
        GetToggle((int)Toggles.EffectToggle).SubToggleClick(OnEffect, this, Managers.Data.Setting.IsEffect.Value);
        GetToggle((int)Toggles.KorToggle).SubToggleClick(OnKor, this, Managers.Data.Setting.Language.Value == Define.Language.Kor);
        GetToggle((int)Toggles.EngToggle).SubToggleClick(OnEng, this, Managers.Data.Setting.Language.Value == Define.Language.Eng);
    }
    #endregion
    #region Callback
    void OnClose()
    {
        Debug.Log("[UISetting:OnClose] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.CloseUIPopup(this);
    }

    void OnGiveUp()
    {
        Debug.Log("[UISetting:OnGiveUp] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.GiveUp);
    }

    void OnBgm(bool isOn)
    {
        Debug.Log($"[UISetting:OnBgm] {isOn}");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Data.Setting.IsBgm.Value = isOn;
    }

    void OnSfx(bool isOn)
    {
        Debug.Log($"[UISetting:OnSfx] {isOn}");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Data.Setting.IsSfx.Value = isOn;
    }

    void OnDamage(bool isOn)
    {
        Debug.Log($"[UISetting:OnDamage] {isOn}");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Data.Setting.IsDamage.Value = isOn;
    }

    void OnEffect(bool isOn)
    {
        Debug.Log($"[UISetting:OnEffect] {isOn}");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.Data.Setting.IsEffect.Value = isOn;
    }

    void OnKor(bool isOn)
    {
        Debug.Log($"[UISetting:OnKor] {isOn}");

        if (isOn)
        {
            Managers.Sound.Play(Define.Sound.Sfx, "Click");
            Managers.Data.Setting.Language.Value = Define.Language.Kor;
        }
    }

    void OnEng(bool isOn)
    {
        Debug.Log($"[UISetting:OnEng] {isOn}");

        if (isOn)
        {
            Managers.Sound.Play(Define.Sound.Sfx, "Click");
            Managers.Data.Setting.Language.Value = Define.Language.Eng;
        }
    }
    #endregion
}
