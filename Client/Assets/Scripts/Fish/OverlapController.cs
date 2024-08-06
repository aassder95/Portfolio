using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class OverlapController
{
    #region Fields
    bool m_isSell = false;
    Vector3 m_originPos;
    FishBase m_fish;
    BoxCollider2D m_boxColl;
    ReactiveProperty<bool> m_isDrag = new ReactiveProperty<bool>();
    #endregion
    #region Properties
    public bool IsSell { set => m_isSell = value; }
    public IReadOnlyReactiveProperty<bool> IsDrag => m_isDrag;
    #endregion
    #region Init
    public OverlapController(FishBase fish)
    {
        m_fish = fish;
        m_boxColl = m_fish.GetComponent<BoxCollider2D>();
    }

    public void Clear()
    {
        m_isDrag.Value = false;
        m_isSell = false;
    }
    #endregion
    #region Utility
    void CheckOverlap()
    {
        if (IsOverlap())
            Managers.Fish.Despawn(m_fish);
        else
            m_fish.transform.position = m_originPos;
    }
    #endregion
    #region Is
    bool IsOverlap()
    {
        Collider2D[] colls = Physics2D.OverlapBoxAll(m_fish.transform.position, m_boxColl.bounds.size, 0);
        FishBase overlapFish = colls.Select(coll => coll.GetComponent<FishBase>()).
            FirstOrDefault(fish => IsSameTypeAndStar(fish));

        if (overlapFish == null)
            return false;

        overlapFish.Template.Value = Managers.Template.GetNextStarFishTemplate(overlapFish.Template.Value);

        return true;
    }

    bool IsSameTypeAndStar(FishBase overlapFish)
    {
        if (overlapFish == null || overlapFish.gameObject == m_fish.gameObject)
            return false;

        if (m_fish.Template.Value.Star == Constant.Game.MAX_STAR || overlapFish.Template.Value.Star == Constant.Game.MAX_STAR ||
            m_fish.Template.Value.Star != overlapFish.Template.Value.Star ||
            m_fish.Template.Value.Type != overlapFish.Template.Value.Type ||
            m_fish.Template.Value.Kind != overlapFish.Template.Value.Kind)
            return false;

        return true;
    }
    #endregion
    #region Set
    public void SetPosition(Vector3 pos)
    {
        m_originPos = pos;
        m_fish.transform.position = m_originPos;
    }
    #endregion
    #region Callback
    public void OnMouseDown()
    {
        m_isDrag.Value = true;
    }

    public void OnMouseUp()
    {
        if (m_isSell)
        {
            Managers.Fish.Despawn(m_fish);
            Managers.Game.IncreasePearl(m_fish.Template.Value.Pearl);
        }
        else
        {
            CheckOverlap();
        }

        m_isDrag.Value = false;
    }
    #endregion
}
