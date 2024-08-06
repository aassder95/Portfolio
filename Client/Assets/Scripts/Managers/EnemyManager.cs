using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager<EnemyBase>
{
    #region Fields
    private EnemyBase m_target;
    #endregion
    #region Properties
    public EnemyBase Target => m_target;
    #endregion
    #region Init
    public override void Clear()
    {
        Despawn(m_target);
        base.Clear();
    }
    #endregion
    #region Control
    public override GameObject Spawn(int id)
    {
        EnemyTemplate template = Managers.Template.GetEnemyTemplate(id);

        GameObject go = Managers.Resource.Instantiate($"Enemy/{template.Name}");
        go.transform.position = Constant.Stage.START_POS;

        EnemyBase enemy = go.GetOrAddComponent<EnemyBase>();
        enemy.Activate(template);
        AddCharacter(enemy);

        return go;
    }

    public override void Despawn(EnemyBase enemy)
    {
        if (m_target == enemy)
            ResetTarget();

        base.Despawn(enemy);
    }

    void ResetTarget()
    {
        m_target?.SetTarget(false);
        m_target = null;
    }
    #endregion
    #region Utility
    public void CheckTarget(EnemyBase target)
    {
        if (m_target == target)
            ResetTarget();
        else
            SetTarget(target);
    }
    #endregion
    #region Set
    void SetTarget(EnemyBase target)
    {
        m_target?.SetTarget(false);
        m_target = target;
        m_target.SetTarget(true);
    }
    #endregion
}
