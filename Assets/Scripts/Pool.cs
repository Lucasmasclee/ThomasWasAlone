using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    private Stack<T> pooledObjects;
    private T loadedObject;

    public Pool(string path, int initialSize)
    {
        pooledObjects = new Stack<T>(initialSize);
        loadedObject = Resources.Load<T>(path);

        for (int i = 0; i < initialSize; i++)
        {
            T obj = InstantiateObject();
            pooledObjects.Push(obj);
        }
    }

    public T Rent()
    {
        T obj;

        if (!pooledObjects.TryPop(out obj))
        {
            obj = InstantiateObject();
        }

        return obj;
    }

    public void TurnBack(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(null);
        pooledObjects.Push(obj);
    }

    private T InstantiateObject()
    {
        T obj = Object.Instantiate(loadedObject);
        obj.gameObject.SetActive(false);
        return obj;
    }
}
