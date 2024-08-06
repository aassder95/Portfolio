using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class GameData
{
    #region Fields
    public string Name { get; set; }
    public int ClearStage { get; set; }
    #endregion
    #region Properties
    public ReactiveProperty<int> Gold { get; set; } = new ReactiveProperty<int>();
    public ReactiveProperty<int> Gem { get; set; } = new ReactiveProperty<int>();
    #endregion
}

[Serializable]
public class SettingData
{
    #region Properties
    public string UUID { get; set; } = Guid.NewGuid().ToString();
    public ReactiveProperty<bool> IsBgm { get; set; } = new ReactiveProperty<bool>(true);
    public ReactiveProperty<bool> IsSfx { get; set; } = new ReactiveProperty<bool>(true);
    public ReactiveProperty<bool> IsDamage { get; set; } = new ReactiveProperty<bool>(true);
    public ReactiveProperty<bool> IsEffect { get; set; } = new ReactiveProperty<bool>(true);
    public ReactiveProperty<Define.Language> Language { get; set; } = new ReactiveProperty<Define.Language>(Define.Language.Kor);
    #endregion
    #region Control
    public string ToJson()
    {
        JObject jsonObj = new JObject
        {
            ["UUID"] = UUID,
            ["IsBgm"] = IsBgm.Value,
            ["IsSfx"] = IsSfx.Value,
            ["IsDamage"] = IsDamage.Value,
            ["IsEffect"] = IsEffect.Value,
            ["Language"] = (int)Language.Value,
        };

        return jsonObj.ToString(Formatting.None);
    }

    public void FromJson(string json)
    {
        JObject jObject = JObject.Parse(json);
        UUID = jObject.ToStringValue("UUID");
        IsBgm.Value = jObject.ToBool("IsBgm");
        IsSfx.Value = jObject.ToBool("IsSfx");
        IsDamage.Value = jObject.ToBool("IsDamage");
        IsEffect.Value = jObject.ToBool("IsEffect");
        Language.Value = (Define.Language)jObject.ToInt("Language");
    }
    #endregion
}

public class DataManager
{
    #region Fields
    GameData m_gameData = new GameData();
    SettingData m_settingData = new SettingData();
    #endregion
    #region Properties
    public GameData Game => m_gameData;
    public SettingData Setting => m_settingData;
    #endregion
    #region Init
    public void Init()
    {
        LoadSettingData();
        InitEvent();
    }
    #endregion
    #region InitSub
    void InitEvent()
    {
        m_settingData.IsBgm.Skip(1).Subscribe(_ => SaveSettingData());
        m_settingData.IsSfx.Skip(1).Subscribe(_ => SaveSettingData());
        m_settingData.IsDamage.Skip(1).Subscribe(_ => SaveSettingData());
        m_settingData.IsEffect.Skip(1).Subscribe(_ => SaveSettingData());
        m_settingData.Language.Skip(1).Subscribe(_ => SaveSettingData());
        m_settingData.Language.Skip(1).Subscribe(_ => OnChangeLanguage());
    }
    #endregion
    #region Control
    void LoadSettingData()
    {
        Debug.Log("[DataManager:LoadSettingData]");

        if (PlayerPrefs.HasKey(Constant.Path.Name.SETTING_DATA))
        {
            string json = PlayerPrefs.GetString(Constant.Path.Name.SETTING_DATA);
            m_settingData.FromJson(json);
        }
        else
        {
            SaveSettingData();
        }
    }

    void SaveSettingData()
    {
        Debug.Log("[DataManager:SaveSettingData]");

        string json = m_settingData.ToJson();
        PlayerPrefs.SetString(Constant.Path.Name.SETTING_DATA, json);
        PlayerPrefs.Save();
    }
    #endregion
    #region Is
    public bool IsEnoughGold(int value)
    {
        if (m_gameData.Gold.Value < value)
        {
            Managers.UI.ShowUIComfirmPopup(Define.UIConfirm.NotEnoughGold);
            return false;
        }

        return true;
    }
    #endregion
    #region Set
    public void SetData(JObject data)
    {
        JObject stageData = data.ToJObject("stage");

        m_gameData.Name = data.ToStringValue("name");
        m_gameData.Gold.Value = data.ToInt("gold");
        m_gameData.Gem.Value = data.ToInt("gem");
        m_gameData.ClearStage = stageData.ToInt("clearStage");
    }
    #endregion
    #region Callback
    void OnChangeLanguage()
    {
        Managers.UI.RefreshUI();
    }
    #endregion
}
