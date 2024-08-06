using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankData
{
    #region Fields
    int m_rank;
    string m_name;
    int m_stage;
    #endregion
    #region Properties
    public int Rank => m_rank;
    public string Name => m_name;
    public int Stage => m_stage;
    #endregion
    #region Init
    public RankData(int rank, string name, int stage)
    {
        m_rank = rank;
        m_name = name;
        m_stage = stage;
    }
    #endregion
}

public class RankManager
{
    #region Fields
    RankData m_myRankData;
    List<RankData> m_rankDatas = new List<RankData>();
    #endregion
    #region Properties
    public RankData MyRankData { get => m_myRankData; set => m_myRankData = value; }
    public List<RankData> RankDatas => m_rankDatas;
    #endregion
    #region Set
    public void SetData(JObject data)
    {
        m_rankDatas.Clear();

        foreach (JToken rankData in data.ToJArray("rank"))
            m_rankDatas.Add(new RankData(rankData.ToInt("rank"), rankData.ToStringValue("name"), rankData.ToInt("clearStage")));

        JObject myData = data.ToJObject("myRank");
        m_myRankData = new RankData(myData.ToInt("rank"), Managers.Data.Game.Name, myData.ToInt("clearStage"));
    }
    #endregion
}
