using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject activeobject;
    [SerializeField] private GameObject gameobject;
    private List<GameObject> gameobjects = new List<GameObject>();

    [SerializeField] private int debugactiveobject;
    private float surface;

    void Start()
    {
        instance = this;
        gameobjects.Add(gameobject);
        surface = transform.localScale.x * transform.localScale.x;
    }

    void Update()
    {
        debugactiveobject = gameobjects.IndexOf(activeobject);

        if (Input.GetKeyDown(KeyCode.RightArrow) && gameobjects.IndexOf(activeobject) != gameobjects.Count - 1)
        {
            activeobject = gameobjects[gameobjects.IndexOf(activeobject) + 1];
            Debug.Log("Active object: " + gameobjects.IndexOf(activeobject));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && gameobjects.IndexOf(activeobject) > 0)
        {
            activeobject = gameobjects[gameobjects.IndexOf(activeobject) - 1];
            Debug.Log("Active object: " + gameobjects.IndexOf(activeobject));
        }
    }

    public void SplitObject()
    {
        Vector3 spawnpos = new Vector3(transform.position.x, transform.position.y, 0f); // DEtermine spawn pos of new object
        GameObject newobject = Instantiate(gameobject, spawnpos, Quaternion.identity); // Instantiate new object
        gameobjects.Add(newobject); // Add the new object to the list of gameobjects

        Transform objtransform = newobject.transform; // Get transform of new object
        float newscale = Mathf.Sqrt((activeobject.transform.localScale.x*activeobject.transform.localScale.x)/2); // Determine scale of new object
        objtransform.localScale = new Vector3(newscale, newscale); // Set scale of new object
        activeobject.transform.localScale = new Vector3(newscale, newscale); // Set scale of new object
        objtransform.position = new Vector3(objtransform.position.x + 2f, 0, 0); // Set the position of new object
        activeobject.transform.position = new Vector3(gameobject.transform.position.x - 2f, 0, 0); // Set the position of original object

        activeobject = newobject;
        // Set the new and original object to the appropriate layer.
    }

    public void MergeObjects(Vector3 pos1, Vector3 pos2, float scale)
    {
        Vector3 spawnpos = (pos1 + pos2) / 2f; // Spawn pos will be in the middle of the two objects
        GameObject newobj = Instantiate(gameobject, spawnpos, Quaternion.identity); // Instantiate the object
        newobj.transform.localScale = new Vector3(scale, scale, 0f);
    }

    public GameObject GetActiveObject()
    {
        return activeobject;
    }
}
