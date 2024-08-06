using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Util
{
    #region Coroutines
    public static IEnumerator FadeOut(float duration, params Graphic[] graphics)
    {
        float alpha = graphics[0].color.a;
        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime / duration * Time.timeScale;
            foreach (var graphic in graphics)
            {
                graphic.SetAlpha(alpha);
            }
            yield return null;
        }
    }

    public static IEnumerator FadeIn(float duration, params Graphic[] graphics)
    {
        float alpha = graphics[0].color.a;
        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime / duration * Time.timeScale;
            foreach (var graphic in graphics)
            {
                graphic.SetAlpha(alpha);
            }
            yield return null;
        }
    }
    #endregion
    #region Get
    public static Color GetTierColor(Define.Tier tier)
    {
        switch (tier)
        {
            case Define.Tier.Common:
                return GetColor(169, 169, 169);
            case Define.Tier.Uncommon:
                return GetColor(144, 238, 144);
            case Define.Tier.Rare:
                return GetColor(135, 206, 235);
            case Define.Tier.Epic:
                return GetColor(186, 85, 211);
            case Define.Tier.Legendary:
                return GetColor(220, 60, 0);
            case Define.Tier.Mythology:
                return GetColor(31, 166, 41);
        }

        return Color.black;
    }

    public static Color GetColor(int r, int g, int b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public static string GetTimeToUTCMidnight()
    {
        DateTime utcNow = DateTime.UtcNow;
        DateTime nextUTCMidnight = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day).AddDays(1);
        TimeSpan timeToMidnight = nextUTCMidnight - utcNow;
        if (timeToMidnight.TotalSeconds < 0)
        {
            nextUTCMidnight = nextUTCMidnight.AddDays(1);
            timeToMidnight = nextUTCMidnight - utcNow;
        }

        return string.Format("{0:D2}:{1:D2}:{2:D2}", timeToMidnight.Hours, timeToMidnight.Minutes, timeToMidnight.Seconds);
    }

    public static string FormatNumber(int value)
    {
        if (value >= 1000000000)
        {
            return (value / 1000000000f).ToString("0.##") + "B";
        }
        else if (value >= 1000000)
        {
            return (value / 1000000f).ToString("0.##") + "M";
        }
        else if (value >= 1000)
        {
            return (value / 1000f).ToString("0.##") + "K";
        }

        return value.ToString("0");
    }

    public static string FormatText(int id, params string[] args)
    {
        string txt = Managers.Template.GetText(id);
        return args.Length > 0 ? string.Format(txt, args) : txt;
    }
    #endregion
}
