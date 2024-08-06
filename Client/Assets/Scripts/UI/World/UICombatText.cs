using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICombatText : UIBase, IPoolable
{
    #region Enum
    enum Texts
    {
        CombatText,
    }
    #endregion
    #region Interface
    public int PoolCnt => 20;
    public GameObject GameObject => gameObject;
    #endregion
    #region Fields
    int m_value;
    Vector3 m_pos;
    TextMeshProUGUI m_combatTxt;
    Coroutine m_fadeOutCoroutine;
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindText(typeof(Texts));

        m_combatTxt = GetText((int)Texts.CombatText);

        return true;
    }

    public void Deactivate()
    {
        StopCoroutine(m_fadeOutCoroutine);
    }
    #endregion
    #region Control
    public void ShowCombatText()
    {
        if (!m_isInit)
            Init();

        transform.position = m_pos + GetPos();
        m_combatTxt.color = GetColor();
        m_combatTxt.fontSize = GetFontSize();
        m_combatTxt.SetText(GetText());

        m_fadeOutCoroutine = StartCoroutine(FadeOut());
    }
    #endregion
    #region Coroutines
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(Constant.UI.Combat.STAY_DURATION);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * GetOffsetY();
        Color startColor = m_combatTxt.color;
        Color endColor = startColor;
        endColor.a = 0.0f;

        float elapsedTime = 0.0f;
        float duration = Constant.UI.Combat.FADE_DURATION;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            m_combatTxt.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        m_combatTxt.color = endColor;
        Managers.Resource.Destroy(gameObject);
    }
    #endregion
    #region Get
    Vector3 GetPos()
    {
        switch ((Define.UICombatText)m_type)
        {
            case Define.UICombatText.AttDamage:
            case Define.UICombatText.CriDamage:
                return Vector3.up * 0.15f;
        }

        return Vector3.zero;
    }

    string GetText()
    {
        switch ((Define.UICombatText)m_type)
        {
            case Define.UICombatText.Pearl:
                return $"+{m_value}";
        }

        return $"{m_value}";
    }

    Color GetColor()
    {
        switch ((Define.UICombatText)m_type)
        {
            case Define.UICombatText.CriDamage:
                return Color.red;
            case Define.UICombatText.Pearl:
                return Util.GetColor(74, 78, 209);
        }

        return Color.white;
    }

    float GetFontSize()
    {
        switch ((Define.UICombatText)m_type)
        {
            case Define.UICombatText.AttDamage:
                return Constant.UI.Combat.DAMAGE_SIZE;
            case Define.UICombatText.CriDamage:
                return Constant.UI.Combat.CRI_DAMAGE_SIZE;
            case Define.UICombatText.Pearl:
                return Constant.UI.Combat.PEARL_SIZE;
        }

        return 20.0f;
    }

    float GetOffsetY()
    {
        switch ((Define.UICombatText)m_type)
        {
            case Define.UICombatText.AttDamage:
            case Define.UICombatText.CriDamage:
                return Constant.UI.Combat.DAMAGE_OFFSET_Y;
        }

        return 0.0f;
    }
    #endregion
    #region Set
    public void SetData(Vector3 pos, int value)
    {
        m_pos = pos;
        m_value = value;
    }
    #endregion
}
