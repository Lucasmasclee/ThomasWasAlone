using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 startpos; // This should be to the left of, or above endpos
    [SerializeField] private Vector2 endpos; // This should be to the right of, or under endpos
    private bool moveToEndPos = true;

    private void Start()
    {
        transform.position = startpos;
    }

    private void Update()
    {
        if (moveToEndPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, startpos, moveSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, endpos, moveSpeed * Time.deltaTime);
        }
        if(transform.position.x <= startpos.x || transform.position.x >= endpos.x)
        {
            moveToEndPos ^= true; // Toggles the variable
        }
    }
    private void OnCollisionEnter2D()
    {
        moveToEndPos ^= true;
    }
}
