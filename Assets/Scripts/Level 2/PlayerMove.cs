using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove instance;
    private float surface;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    private Rigidbody2D rb;
    //private GameObject myGameObject;


    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        surface = 3f*3f; // Hardcoded, dont know how else to do this
    }

    void FixedUpdate()
    {
        if(GameManager.instance.GetActiveObject() == gameObject)
        {
            if (Input.GetKey(KeyCode.A)) // Moving left
            {
                transform.position += Vector3.left * moveSpeed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.D)) // Moving right
            {
                transform.position += Vector3.right * moveSpeed * Time.fixedDeltaTime;
            }
            if (Input.GetKey(KeyCode.W)) // Jumping
            {
                rb.velocity = new Vector2(0f, jumpPower);
            }
            if (Input.GetKeyDown(KeyCode.Space) && Math.Pow(transform.localScale.x,2f)*15.9f > surface) // Splitting objects, if statement becomes false after object is splitted 3 times
            {
                Debug.Log("Splitting objects");
                GameManager.instance.SplitObject();
            }
        }
    }

    void OnCollisonEnter2D(Collision2D other)
    {
        Transform othertransform = other.transform;
        if(transform.localScale == othertransform.localScale) // ***Condition is incorrect, condition must be true if two objects of same size collide
        {
            Debug.Log("Merging objects");
            float scale = (Mathf.Sqrt(2f) * transform.localScale.x);
            GameManager.instance.MergeObjects(transform.position, other.transform.position, scale);
        }
    }
}
