using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRankItem : UIBase
{
    #region Enum
    enum Texts
    {
        RankText,
        NameText,
        ScoreText,
    }
    #endregion
    #region Fields
    RankData m_data;
    #endregion
    #region Properties
    public RankData RankData { set => m_data = value; }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindText(typeof(Texts));

        GetText((int)Texts.RankText).SetText($"{m_data.Rank}");
        GetText((int)Texts.NameText).SetText($"{m_data.Name}");
        GetText((int)Texts.ScoreText).SetText($"{m_data.Stage}");

        return true;
    }
    #endregion
}
