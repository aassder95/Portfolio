using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IPoolable
{
    #region Interface
    public int PoolCnt => 20;
    public GameObject GameObject => gameObject;
    #endregion
    #region Fields
    EnemyTemplate m_template;
    DamageController m_damageController;
    MovementController m_movementController;
    UIHpBar m_uiHpBar;
    #endregion
    #region Properties
    public EnemyTemplate Template => m_template;
    public DamageController DamageCtrl => m_damageController;
    public MovementController MovementCtrl => m_movementController;
    #endregion
    #region Unity
    void Awake()
    {
        m_damageController = new DamageController(this);
        m_movementController = new MovementController(this);
        m_uiHpBar = Managers.UI.CreateUIWorld<UIHpBar>(gameObject);
    }

    void Update()
    {
        if (Managers.Stage.IsGameOver)
            return;

        m_movementController.Update();
    }
    #endregion
    #region Init
    public void Activate(EnemyTemplate template)
    {
        m_template = template;
        m_damageController.Init();
    }

    public void Deactivate()
    {
        m_damageController.Clear();
        m_movementController.Clear();
        m_uiHpBar.Clear();
        m_template = null;
    }
    #endregion
    #region Set
    public void SetTarget(bool isTarget)
    {
        GetComponent<SpriteRenderer>().color = isTarget ? Color.red : Color.white;
    }
    #endregion
}
