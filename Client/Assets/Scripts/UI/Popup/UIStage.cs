using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStage : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
        StageContent,
    }

    enum Texts
    {
        TitleText,
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
        GameObject stage = GetObject((int)GameObjects.StageContent);

        List<StageTemplate> stageTemplates = Managers.Template.GetSingleStageTemplates();
        foreach (StageTemplate template in stageTemplates)
        {
            UIStageItem uiSubItem = Managers.UI.InstantiateUI<UIStageItem>("SubItem/UIStageItem", stage);
            uiSubItem.SetStageTemplate(template);
        }

        GetText((int)Texts.TitleText).SetText(110000038);

        GetButton((int)Buttons.CloseButton).SubButtonClick(OnClose, this);

        return true;
    }
    #endregion
    #region Callback
    void OnClose()
    {
        Debug.Log("[UIStage:OnClose] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.CloseUIPopup(this);
    }
    #endregion
}
