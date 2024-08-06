using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController
{
    #region Fields
    int m_curWaypoint = -1;
    EnemyBase m_enemy;
    List<Vector3> m_waypoints;
    #endregion
    #region Properties
    public int CurWaypoint
    {
        get => m_curWaypoint;
        set => m_curWaypoint = value % m_waypoints.Count;
    }
    int NextWaypoint => (CurWaypoint + 1) % m_waypoints.Count;
    #endregion
    #region Init
    public MovementController(EnemyBase enemy)
    {
        m_enemy = enemy;
        m_waypoints = Managers.Grid.Waypoints;
    }

    public void Update()
    {
        MoveToNextWaypoint();
    }

    public void Clear()
    {
        CurWaypoint = -1;
    }
    #endregion
    #region Action
    void MoveToNextWaypoint()
    {
        float moveDist = m_enemy.Template.MoveSpeed * Time.deltaTime;

        if (moveDist >= Vector3.Distance(m_enemy.transform.position, m_waypoints[NextWaypoint]))
        {
            m_enemy.transform.position = m_waypoints[NextWaypoint];
            CurWaypoint++;
        }
        else
        {
            Vector3 dir = (m_waypoints[NextWaypoint] - m_enemy.transform.position).normalized;
            m_enemy.transform.position += dir * moveDist;
        }
    }
    #endregion
    #region Get
    public float GetProgressToNextWaypoint()
    {
        float totalDistance = Vector2.Distance(m_waypoints[CurWaypoint], m_waypoints[NextWaypoint]);
        float currentDistance = Vector2.Distance(m_enemy.transform.position, m_waypoints[NextWaypoint]);

        return 1.0f - (currentDistance / totalDistance);
    }
    #endregion
}
