using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringTower : MonoBehaviour
{

    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject firingtower;
    [SerializeField] private Vector2 direction;

    [SerializeField] private bool shootAtplayer; // for shooting at the player
    [SerializeField] private float speedSAP; // speed for shooting at player
    [SerializeField] private bool shootMulPrj; // for shooting multiple, smaller projectiles
    [SerializeField] private float speedSMP; // speed of the projectiles 
    [SerializeField] private bool shootBigPrj; // for shooting one bigger projectile
    [SerializeField] private float speedSBP; // speed for shooting bigger projectiles

    void Start()
    {
        if(shootAtplayer)
        {
            InvokeRepeating("ShootAtPlayer", 0f, 3.5f);
        }
        else if(shootMulPrj)
        {
            InvokeRepeating("ShootMulPrj", 0f, 5f);
        }
        else if(shootBigPrj)
        {
            InvokeRepeating("ShootBigPrj", 0f, 2.5f);
        }
    }

    private void ShootAtPlayer()
    {
        PlayerController[] objects = FindObjectsOfType(typeof(PlayerController)) as PlayerController[]; // Get all player objects
        System.Array.Sort(objects, (a, b) => // Sort based on distance to the firing tower
        {
            float distanceToA = Vector3.Distance(firingtower.transform.position, a.transform.position);
            float distanceToB = Vector3.Distance(firingtower.transform.position, b.transform.position);
            return distanceToA.CompareTo(distanceToB);
        });
        PlayerController closestobj = objects[0]; // Get the closest player object

        Vector3 newpos = firingtower.transform.position + new Vector3(0f,1f,0f); // Determine position for projectile
        GameObject newobj = Instantiate(projectile, newpos, Quaternion.identity); // Instantiate projectile
        Rigidbody2D newobjrb = newobj.GetComponent<Rigidbody2D>(); // Get rigidbody
        Vector2 direction = new Vector2(closestobj.transform.position.x - newpos.x, closestobj.transform.position.y - newpos.y); // Determine direction by subtracting transform pos from player pos
        newobjrb.velocity = direction * speedSAP; // determine direction and velocity of projectile
    }

    private void ShootMulPrj()
    {
        StartCoroutine(RapidFire(0.3f));
    }
    IEnumerator RapidFire(float sec)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector3 newpos = firingtower.transform.position + new Vector3(0f, 1f, 0f); // Determine position for projectile
            GameObject newobj = Instantiate(projectile, newpos, Quaternion.identity); // Instantiate projectile
            newobj.transform.localScale = newobj.transform.localScale*0.5f; // Scale new object
            Rigidbody2D newobjrb = newobj.GetComponent<Rigidbody2D>(); // Get rigidbody
            newobjrb.velocity = new Vector2(direction.x, direction.y) * speedSMP; // determine direction and velocity of projectile
            yield return new WaitForSeconds(sec);
        }
    }

    private void ShootBigPrj()
    {
        Vector3 newpos = firingtower.transform.position + new Vector3(0f, 1f, 0f); // Determine position for projectile
        GameObject newobj = Instantiate(projectile, newpos, Quaternion.identity); // Instantiate projectile
        newobj.transform.localScale = newobj.transform.localScale * 1.5f; // Scale new object
        Rigidbody2D newobjrb = newobj.GetComponent<Rigidbody2D>(); // Get rigidbody
        newobjrb.velocity = new Vector2(direction.x, direction.y) * speedSBP; // determine direction and velocity of projectile
    }
}
