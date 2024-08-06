using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FishBase : MonoBehaviour, IPoolable
{
    #region Interface
    public int PoolCnt => 10;
    public GameObject GameObject => gameObject;
    #endregion
    #region Fields
    TargetController m_targetController;
    MissileController m_missileController;
    OverlapController m_overlapController;
    ReactiveProperty<FishTemplate> m_template = new ReactiveProperty<FishTemplate>();
    #endregion
    #region Properties
    public TargetController TargetCtrl => m_targetController;
    public MissileController MissileCtrl => m_missileController;
    public OverlapController OverlapCtrl => m_overlapController;
    public ReactiveProperty<FishTemplate> Template => m_template;
    #endregion
    #region Unity
    void Awake()
    {
        m_targetController = new TargetController(this);
        m_missileController = new MissileController(this);
        m_overlapController = new OverlapController(this);
        Managers.UI.CreateUIWorld<UIFishStar>(gameObject);
    }

    void Update()
    {
        if (Managers.Stage.IsGameOver)
            return;

        if (m_overlapController.IsDrag.Value)
            return;

        m_targetController.Update();
        m_missileController.Update();
    }
    #endregion
    #region Init
    public void Activate(FishTemplate template)
    {
        m_template.Value = template;
        GetComponent<SpriteRenderer>().color = Util.GetTierColor((Define.Tier)m_template.Value.Tier);
    }

    public void Deactivate()
    {
        m_template.Value = null;
        m_targetController.Clear();
        m_missileController.Clear();
        m_overlapController.Clear();
    }
    #endregion
    #region UnityCallback
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_template.Value.AttRange);
    }
    #endregion
}
