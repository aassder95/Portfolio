using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIHpBar : UIBase
{
    #region Enum
    enum GameObjects
    {
        HpBar,
    }
    #endregion
    #region Fields
    float m_curHpRatio = 1.0f;
    float m_targetHpRatio;
    Slider m_hpBar;
    EnemyBase m_enemyBase;
    #endregion
    #region Unity
    void Update()
    {
        m_curHpRatio = Mathf.Lerp(m_curHpRatio, m_targetHpRatio, Time.deltaTime * 10.0f);
        m_hpBar.value = m_curHpRatio;
    }
    #endregion
    #region Init
    protected override bool Init()
    {
        if (!base.Init())
            return false;

        BindObject(typeof(GameObjects));

        m_hpBar = GetObject((int)GameObjects.HpBar).GetComponent<Slider>();

        float offsetY = 0.2f;
        transform.position += Vector3.down * (transform.parent.GetComponent<BoxCollider2D>().bounds.size.y - offsetY);

        m_enemyBase = transform.parent.GetComponent<EnemyBase>();
        m_enemyBase.DamageCtrl.Hp.Subscribe(OnHpChanged).AddTo(this);

        return true;
    }

    public override void Clear()
    {
        m_curHpRatio = 1.0f;
    }
    #endregion
    #region Callback
    void OnHpChanged(int hp)
    {
        m_targetHpRatio = hp / (float)m_enemyBase.Template.Hp;
    }
    #endregion
}
