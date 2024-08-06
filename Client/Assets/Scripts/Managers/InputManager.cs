using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class InputManager
{
    #region Fields
    Vector3 m_offset;
    EnemyBase m_selectedEnemy;
    ReactiveProperty<FishBase> m_selectedFish = new ReactiveProperty<FishBase>();
    #endregion
    #region Properties
    public IReadOnlyReactiveProperty<FishBase> SelectedFish => m_selectedFish;
    #endregion
    #region Init
    public void Clear()
    {
        m_selectedFish.Value = null;
        m_selectedEnemy = null;
    }

    public void Update()
    {
        if (Managers.UI.IsUIPopup())
            return;

        if (Input.GetMouseButtonDown(0))
            HandleMouseDown();
        else if (Input.GetMouseButtonUp(0))
            HandleMouseUp();
        else if (m_selectedFish.Value != null)
            HandleMouseDrag();
    }
    #endregion
    #region Control
    void HandleMouseDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null)
            return;

        FishBase fish = hit.collider.GetComponent<FishBase>();
        if (fish != null)
        {
            m_selectedFish.Value = fish;
            m_selectedFish.Value.OverlapCtrl.OnMouseDown();
            m_offset = fish.transform.position - (Vector3)hit.point;
            return;
        }

        m_selectedEnemy = hit.collider.GetComponent<EnemyBase>();
    }

    void HandleMouseUp()
    {
        if (m_selectedFish.Value != null)
        {
            m_selectedFish.Value.OverlapCtrl.OnMouseUp();
            m_selectedFish.Value = null;
        }
        else if (m_selectedEnemy != null)
        {
            Managers.Enemy.CheckTarget(m_selectedEnemy);
            m_selectedEnemy = null;
        }
    }

    void HandleMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + m_offset;
        m_selectedFish.Value.transform.position = new Vector3(pos.x, pos.y, 0);
    }
    #endregion
}
