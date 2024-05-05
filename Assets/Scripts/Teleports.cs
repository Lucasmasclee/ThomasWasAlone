using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleports : MonoBehaviour
{
    // All child gameobjects
    [SerializeField] private GameObject portal1;
    [SerializeField] private GameObject portal2;
    [SerializeField] private GameObject portal1square; // The square of portal1, this turns green if teleporting is allowed and red if not
    [SerializeField] private GameObject portal1triangle; // The triangle of portal1, this is transparent if teleporting is not allowed and solid otherwise
    [SerializeField] private GameObject portal2square; // The square of portal2, this turns green if teleporting is allowed and red if not
    [SerializeField] private GameObject portal2triangle; // The triangle of portal2, this is transparent if teleporting is not allowed and solid otherwise

    // Everything for colliding and teleporting
    private BoxCollider2D boxTrigger; // Box that checks if anything is in front of 2nd teleport
    private BoxCollider2D t1box; // Boxcollider of portal1square
    private BoxCollider2D t2box; // Boxcollider of portal2square
    [SerializeField] private LayerMask layerPlayer; // Layer of the player
    [SerializeField] private LayerMask layerTeleport; // Layer teleport
    [SerializeField] private Vector2 spawnpos; // spawn pos of teleported object
    private bool teleportAllowed = true;

    // Everything else
    [SerializeField] private float delayTeleport;
    private float scalefactor;
    private SpriteRenderer spsquare1;
    private SpriteRenderer spsquare2;

    void Start()
    {
        boxTrigger = portal2.GetComponent<BoxCollider2D>();
        t1box = portal1square.GetComponent<BoxCollider2D>();
        t2box = portal2square.GetComponent<BoxCollider2D>();
        spsquare1 = portal1square.GetComponent<SpriteRenderer>();
        spsquare2 = portal2square.GetComponent<SpriteRenderer>();
        scalefactor = 1 + (delayTeleport / 100f);
    }

    void Update()
    {
        AllowTeleport(!boxTrigger.IsTouchingLayers(layerPlayer)); // True if nothing is in front of 2nd portal
        if (t1box.IsTouchingLayers(layerPlayer) && !boxTrigger.IsTouchingLayers(layerPlayer))
        {
            spawnpos = new Vector2(boxTrigger.transform.position.x, boxTrigger.transform.position.y - 0.5f);
            TeleportPlayer(); teleportAllowed = true;
        }
        else
        {
            teleportAllowed = false;
        }

    }

    private void AllowTeleport(bool enable)
    {
        if(enable)
        {
            spsquare1.color = Color.green; spsquare2.color = Color.green; // Change color of teleport squares
        }
        else
        {
            spsquare1.color = Color.red; spsquare2.color = Color.red; // Change color of teleport squares
        }
    }

    private void TeleportPlayer()
    {
        PlayerController[] objects = FindObjectsOfType(typeof(PlayerController)) as PlayerController[]; // Get all player objects
        foreach (PlayerController obj in objects)
        {
            BoxCollider2D objcollider = obj.GetComponent<BoxCollider2D>();
            if(objcollider != null && objcollider.IsTouchingLayers(layerTeleport) && teleportAllowed) // checking if teleporting is allowed, before delay
            {
                StartCoroutine(DelayedCode(delayTeleport, obj, objcollider)); // Execute delayed teleport
                obj.transform.localScale = obj.transform.localScale / scalefactor;
            }
        }
    }
    IEnumerator DelayedCode(float sec, PlayerController obj, BoxCollider2D objcollider)
    {
        yield return new WaitForSeconds(sec);
        obj.transform.localScale = obj.transform.localScale * scalefactor;
        if (objcollider != null && objcollider.IsTouchingLayers(layerTeleport) && teleportAllowed) // checking if teleporting is allowed, after delay
        {
            obj.transform.localPosition = spawnpos;
        }
    }
}
