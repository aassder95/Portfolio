using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRank : UIPopup
{
    #region Enum
    enum GameObjects
    {
        BackgroundPanel,
        RankContent,
    }

    enum Texts
    {
        TitleText,
        RankText,
        NameText,
        ScoreText,
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
        GameObject rank = GetObject((int)GameObjects.RankContent);

        foreach (RankData data in Managers.Rank.RankDatas)
        {
            UIRankItem uiSubItem = Managers.UI.InstantiateUI<UIRankItem>("SubItem/UIRankItem", rank);
            uiSubItem.RankData = data;
        }

        RankData myData = Managers.Rank.MyRankData;
        GetText((int)Texts.TitleText).SetText(110000026);
        GetText((int)Texts.RankText).SetText(myData.Rank == 0 ? "-" : myData.Rank.ToString());
        GetText((int)Texts.NameText).SetText($"{myData.Name}");
        GetText((int)Texts.ScoreText).SetText(myData.Stage == 0 ? "-" : myData.Stage.ToString());

        GetButton((int)Buttons.CloseButton).SubButtonClick(OnClose, this);

        return true;
    }
    #endregion
    #region Callback
    void OnClose()
    {
        Debug.Log("[UIRank:OnClose] Click");

        Managers.Sound.Play(Define.Sound.Sfx, "Click");
        Managers.UI.CloseUIPopup(this);
    }
    #endregion
}
