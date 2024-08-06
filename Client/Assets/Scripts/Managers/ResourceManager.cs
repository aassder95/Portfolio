using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    #region Fields
    public Dictionary<string, Sprite> m_sprites = new Dictionary<string, Sprite>();
    #endregion
    #region Control
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(Sprite))
            return LoadSprite(path) as T;
        else if (typeof(T) == typeof(GameObject))
            return LoadGameObject(path) as T;

        return LoadResource<T>(path);
    }

    Sprite LoadSprite(string path)
    {
        if (m_sprites.TryGetValue(path, out Sprite sp))
            return sp;

        Sprite sprite = LoadResource<Sprite>(path);
        if (sprite != null)
            m_sprites.Add(path, sprite);

        return sprite;
    }

    GameObject LoadGameObject(string path)
    {
        string name = path;
        int index = name.LastIndexOf('/');
        if (index >= 0)
            name = name.Substring(index + 1);

        GameObject go = Managers.Pool.GetOriginal(name);
        if (go != null)
            return go;

        return LoadResource<GameObject>(path);
    }

    T LoadResource<T>(string path) where T : Object
    {
        T res = Resources.Load<T>(path);
        if (res == null)
        {
            Debug.LogError($"[ResourceManager:Load] Failed to load resource at path: {path}");
            return null;
        }

        return res;
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        if (prefab == null)
            return null;

        if (prefab.GetComponent<IPoolable>() != null)
            return Managers.Pool.Pop(prefab, parent).GameObject;

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;

        return go;
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        return Instantiate(Load<GameObject>($"Prefabs/{path}"), parent);
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        IPoolable poolable = go.GetComponent<IPoolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go);
    }
    #endregion
}
