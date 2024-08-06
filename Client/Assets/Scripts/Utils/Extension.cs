using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public static class Extension
{
    #region Control
    public static T FindChild<T>(this GameObject go, string name = null, bool isRecursive = false) where T : UnityEngine.Object
    {
        return isRecursive
            ? go.GetComponentsInChildren<T>().FirstOrDefault(comp => (string.IsNullOrEmpty(name) || comp.name == name))
            : go.transform.Find(name)?.GetComponent<T>();
    }

    public static GameObject FindChild(this GameObject go, string name = null, bool isRecursive = false)
    {
        return go.FindChild<Transform>(name, isRecursive)?.gameObject;
    }
    #endregion
    #region Get
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return go.GetComponent<T>() ?? go.AddComponent<T>();
    }

    public static int ToInt(this DataRow row, string columnName)
    {
        return int.Parse(row[columnName].ToString());
    }

    public static float ToFloat(this DataRow row, string columnName)
    {
        return float.Parse(row[columnName].ToString());
    }

    public static string ToStringValue(this DataRow row, string columnName)
    {
        return row[columnName].ToString();
    }

    public static T ToValue<T>(this JObject jObject, string key, T defaultValue = default)
    {
        if (jObject[key] == null)
        {
            Debug.LogError($"[Extension:ToValue] Key '{key}' is null");
            return defaultValue;
        }
        return jObject[key].Value<T>();
    }

    public static bool ToBool(this JObject jObject, string key)
    {
        return jObject.ToValue<bool>(key);
    }

    public static int ToInt(this JObject jObject, string key)
    {
        return jObject.ToValue<int>(key);
    }

    public static float ToFloat(this JObject jObject, string key)
    {
        return jObject.ToValue<float>(key);
    }

    public static string ToStringValue(this JObject jObject, string key)
    {
        return jObject[key]?.ToString();
    }

    public static JObject ToJObject(this JToken jToken, string key)
    {
        return jToken[key] as JObject;
    }

    public static JArray ToJArray(this JToken jToken, string key)
    {
        return jToken[key] as JArray;
    }

    public static int ToInt(this JToken jToken, string key)
    {
        return jToken.Value<int>(key);
    }

    public static string ToStringValue(this JToken jToken, string key)
    {
        return jToken[key]?.ToString();
    }
    #endregion
    #region Set
    public static void SetText(this TextMeshProUGUI text, int id, params string[] args)
    {
        text.SetText(Util.FormatText(id, args));
    }

    public static void SetSprite(this Image img, string name)
    {
        img.sprite = Managers.Resource.Load<Sprite>($"Textures/UI/{name}");
    }

    public static void SetSprite(this Button btn, string name)
    {
        btn.GetComponent<Image>()?.SetSprite(name);
    }

    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
    #endregion
    #region Callback
    public static void SubButtonClick(this Button btn, Action onClick, Component owner)
    {
        btn.OnClickAsObservable().Subscribe(_ => onClick()).AddTo(owner);
    }

    public static void SubToggleClick(this Toggle toggle, Action<bool> onClick, Component owner, bool isOn = false)
    {
        toggle.isOn = isOn;
        toggle.OnValueChangedAsObservable().Skip(1).Subscribe(isOn => onClick(isOn)).AddTo(owner);
    }

    public static void SubEndEdit(this TMP_InputField input, Action<string> onEndEdit, Component owner)
    {
        input.onEndEdit.AsObservable().Subscribe(input => onEndEdit(input)).AddTo(owner);
    }
    #endregion
}
