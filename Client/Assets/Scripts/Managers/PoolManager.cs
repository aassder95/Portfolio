using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable
{
    #region Properties
    int PoolCnt { get; }
    GameObject GameObject { get; }
    #endregion
    #region Init
    void Deactivate();
    #endregion
}

public class PoolManager
{
    class Pool
    {
        #region Fields
        GameObject m_origin;
        GameObject m_root;
        Stack<IPoolable> m_pools = new Stack<IPoolable>();
        #endregion
        #region Properties
        public GameObject Original => m_origin;
        public GameObject Root => m_root;
        #endregion
        #region Init
        public void Init(GameObject origin)
        {
            IPoolable poolable = origin.GetComponent<IPoolable>();
            if (poolable == null)
                return;

            m_origin = origin;
            m_root = new GameObject($"{origin.name}Root");

            for (int i = 0; i < poolable.PoolCnt; i++)
                Push(Create());
        }

        public void Clear()
        {
            m_pools.Clear();

            foreach (Transform child in m_root.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            GameObject.Destroy(m_root);
        }
        #endregion
        #region Control
        IPoolable Create()
        {
            GameObject go = Object.Instantiate(m_origin);
            go.name = m_origin.name;

            return go.GetComponent<IPoolable>();
        }

        public void Push(IPoolable poolable)
        {
            if (poolable == null)
                return;

            poolable.Deactivate();
            poolable.GameObject.transform.SetParent(m_root.transform);
            poolable.GameObject.SetActive(false);

            m_pools.Push(poolable);
        }

        public IPoolable Pop(Transform parent)
        {
            IPoolable poolable = m_pools.Count > 0 ? m_pools.Pop() : Create();

            poolable.GameObject.transform.SetParent(parent ?? Managers.Scene.CurScene.transform);
            poolable.GameObject.SetActive(true);

            return poolable;
        }
        #endregion
    }

    #region Fields
    GameObject m_root;
    Dictionary<string, Pool> m_pools = new Dictionary<string, Pool>();
    #endregion
    #region Init
    public void Init()
    {
        m_root = new GameObject("@PoolRoot");
    }

    public void Clear()
    {
        foreach (Pool pool in m_pools.Values)
            pool.Clear();

        m_pools.Clear();

        foreach (Transform child in m_root.transform)
            GameObject.Destroy(child.gameObject);

        Managers.Resource.Destroy(m_root);
        m_root = null;
    }
    #endregion
    #region Control
    public void Create(GameObject origin)
    {
        Pool pool = new Pool();
        pool.Init(origin);
        pool.Root.transform.SetParent(m_root.transform);

        m_pools.Add(origin.name, pool);
    }

    public void Push(IPoolable poolable)
    {
        string name = poolable.GameObject.name;
        if (!m_pools.ContainsKey(name))
        {
            GameObject.Destroy(poolable.GameObject);
            return;
        }

        m_pools[name].Push(poolable);
    }

    public IPoolable Pop(GameObject origin, Transform parent = null)
    {
        if (!m_pools.ContainsKey(origin.name))
            Create(origin);

        return m_pools[origin.name].Pop(parent);
    }
    #endregion
    #region Get
    public GameObject GetOriginal(string name)
    {
        if (m_pools == null)
            return null;

        return m_pools.TryGetValue(name, out Pool pool) ? pool.Original : null;
    }
    #endregion
}
