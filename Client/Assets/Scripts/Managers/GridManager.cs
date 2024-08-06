using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager
{
    class GridBase
    {
        #region Fields
        protected List<Vector3> m_itemPos;
        #endregion
        #region Properties
        public List<Vector3> ItemPos => m_itemPos;
        #endregion
        #region Init
        public GridBase(Vector2Int size, Vector2 offset, Vector2 worldOffset = default, Vector2 parentPos = default)
        {
            m_itemPos = new List<Vector3>(size.x * size.y);
            CalcGridItemPos(size, offset, worldOffset, parentPos);
        }

        public virtual void Clear()
        {
            m_itemPos.Clear();
        }
        #endregion
        #region Utility
        void CalcGridItemPos(Vector2Int size, Vector2 offset, Vector2 worldOffset, Vector2 parentPos)
        {
            Vector2 halfOffset = new Vector2(
                (size.x % 2 == 0) ? (offset.x / 2.0f) : 0.0f,
                (size.y % 2 == 0) ? (offset.y / 2.0f) : 0.0f);

            Vector2 startPos = new Vector2(
                -(offset.x * ((size.x - 1) / 2)) - halfOffset.x,
                (offset.y * ((size.y - 1) / 2)) + halfOffset.y);

            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector2 pos = new Vector2(startPos.x + x * offset.x, startPos.y - y * offset.y);
                    m_itemPos.Add(pos + parentPos + worldOffset);
                }
            }
        }
        #endregion
    }

    class GridWaypoint : GridBase
    {
        #region Init
        public GridWaypoint(Vector2Int size, Vector2 offset, Vector2 worldOffset = default, Vector2 parentPos = default) : base(size, offset, worldOffset, parentPos)
        {
            for (int i = 0; i < m_itemPos.Count / 4; i++)
            {
                Vector2 tmp = m_itemPos[i];
                m_itemPos[i] = m_itemPos[m_itemPos.Count / 2 - i - 1];
                m_itemPos[m_itemPos.Count / 2 - i - 1] = tmp;
            }
        }
        #endregion
    }

    class GridFishSlot : GridBase
    {
        #region Fields
        GameObject[] m_fishes;
        #endregion
        #region Properties
        public GameObject[] Fishes => m_fishes;
        #endregion
        #region Init
        public GridFishSlot(Vector2Int size, Vector2 offset, Vector2 worldOffset = default, Vector2 parentPos = default) : base(size, offset, worldOffset, parentPos)
        {
            m_fishes = new GameObject[size.x * size.y];
        }

        public override void Clear()
        {
            base.Clear();
            System.Array.Clear(m_fishes, 0, m_fishes.Length);
        }
        #endregion
    }

    #region Fields
    GameObject m_root;
    GridWaypoint m_waypoint;
    List<GridFishSlot> m_fishSlots;
    #endregion
    #region Properties
    public List<Vector3> Waypoints => m_waypoint.ItemPos;
    #endregion
    #region Init
    public void Init()
    {
        m_root = new GameObject("@GridFishSlot");
        m_waypoint = new GridWaypoint(Constant.Stage.Waypoint.CNT, Constant.Stage.Waypoint.OFFSET, Constant.Stage.Waypoint.WORLD_OFFSET);

        GridBase slotGrid = new GridBase(Constant.Stage.Slot.CNT, Constant.Stage.Slot.OFFSET);
        m_fishSlots = new List<GridFishSlot>(slotGrid.ItemPos.Count);
        for (int i = 0; i < slotGrid.ItemPos.Count; i++)
        {
            GridFishSlot slot = new GridFishSlot(Constant.Stage.Fish.CNT, Constant.Stage.Fish.OFFSET, Constant.Stage.Slot.WORLD_OFFSET, slotGrid.ItemPos[i]);
            m_fishSlots.Add(slot);
        }
    }

    public void Clear()
    {
        foreach (var slot in m_fishSlots)
            slot.Clear();

        m_fishSlots.Clear();
        m_waypoint.Clear();
        Managers.Resource.Destroy(m_root);

        m_waypoint = null;
        m_root = null;
    }
    #endregion
    #region Control
    public void PlaceFishInRandPos(GameObject go)
    {
        while (true)
        {
            int slotIndex = Random.Range(0, m_fishSlots.Count);
            GridFishSlot slot = m_fishSlots[slotIndex];

            int fishIndex = Random.Range(0, slot.Fishes.Length);
            if (slot.Fishes[fishIndex] == null)
            {
                PlaceFishInSlot(go, slotIndex, fishIndex);
                return;
            }
        }
    }

    public void RepositionFishes()
    {
        List<GameObject> fishes = m_fishSlots.SelectMany(slot => slot.Fishes).OrderBy(_ => Random.value).ToList();

        for (int i = 0; i < fishes.Count; i++)
        {
            int slotIndex = i / m_fishSlots[0].Fishes.Length;
            int fishIndex = i % m_fishSlots[slotIndex].Fishes.Length;
            m_fishSlots[slotIndex].Fishes[fishIndex] = fishes[i];
            PlaceFishInSlot(fishes[i], slotIndex, fishIndex);
        }
    }

    void PlaceFishInSlot(GameObject go, int slotIndex, int fishIndex)
    {
        if (go == null)
            return;

        go.name = $"Fish_{slotIndex}_{fishIndex}";
        go.transform.SetParent(m_root.transform);

        FishBase fishBase = go.GetOrAddComponent<FishBase>();
        fishBase.OverlapCtrl.SetPosition(m_fishSlots[slotIndex].ItemPos[fishIndex]);

        m_fishSlots[slotIndex].Fishes[fishIndex] = go;
    }

    public void RemoveFish(GameObject go)
    {
        foreach (GridFishSlot slot in m_fishSlots)
        {
            for (int i = 0; i < slot.Fishes.Length; i++)
            {
                if (slot.Fishes[i] != go)
                    continue;

                slot.Fishes[i] = null;
                return;
            }
        }
    }
    #endregion
    #region Is
    public bool HasAvailFishes()
    {
        return m_fishSlots.Any(slot => slot.Fishes.Contains(null));
    }
    #endregion
}
