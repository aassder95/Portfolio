using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageItem : UIBase
{
    #region Enum
    enum Texts
    {
        StageText,
        ClearText,
    }
    #endregion
    #region Fields
    StageTemplate m_template;
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindText(typeof(Texts));

        GetText((int)Texts.StageText).SetText(110000009, $"{m_template.Stage}");
        GetText((int)Texts.ClearText).SetText(110000010);

        GetText((int)Texts.ClearText).gameObject.SetActive(Managers.Data.Game.ClearStage >= m_template.Stage);

        Button btn = GetComponent<Button>();
        btn.SubButtonClick(OnStage, this);
        btn.interactable = Managers.Data.Game.ClearStage + 1 >= m_template.Stage;

        return true;
    }
    #endregion
    #region Set
    public void SetStageTemplate(StageTemplate template)
    {
        m_template = template;
    }
    #endregion
    #region Callback
    void OnStage()
    {
        Debug.Log("[UIStageItem:OnStage] Click");

    }
    #endregion
}
