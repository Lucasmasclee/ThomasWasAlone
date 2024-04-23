using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FiringTower : MonoBehaviour
{
    private enum ShootMethod
    {
        TargeteningShoot,
        MultipleShot,
        SingleShot
    }

    private Pool<Projectile> pool;
    private const int INITIAL_POOL_SIZE = 10;

    [SerializeField] private ShootMethod method;
    [SerializeField] private Transform firingOrigin;
    [SerializeField] private float firingForce;
    [SerializeField] private float firingRate;

    private void Awake()
    {
        pool = new Pool<Projectile>(Constants.PROJECTILE_PREFAB, INITIAL_POOL_SIZE);
    }

    void Start()
    {
        InvokeRepeating(method.ToString(), 0, firingRate);
    }

    private void TargeteningShoot()
    {
        PlayerController[] playerControllers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
        if (playerControllers.Length <= 0) { return; }
        float minDistance = Vector2.Distance(playerControllers[0].transform.position, firingOrigin.position);
        Vector2 target = playerControllers[0].transform.position;
        playerControllers.TryRemoveElementsInRange(0, 1, out _);
        foreach (PlayerController playerController in playerControllers)
        {
            float distance = Vector2.Distance(playerController.transform.position, firingOrigin.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = playerController.transform.position;
            }
        }
        Vector2 targetDirection = target - (Vector2)firingOrigin.position;
        Projectile projectile = pool.Rent();
        projectile.transform.SetPositionAndRotation(firingOrigin.position, Quaternion.identity);
        projectile.Shoot(firingForce * targetDirection.normalized, ProjectileCallBack);

    }

    private void ProjectileCallBack(Projectile projectile)
    {
        pool.TurnBack(projectile);
    }

    private void MultipleShot()
    {
        StartCoroutine(RapidFire(0.3f));
    }

    IEnumerator RapidFire(float sec)
    {
        for (int i = 0; i < 5; i++)
        {
            Projectile projectile = pool.Rent();
            projectile.Shoot(firingForce * firingOrigin.transform.forward, ProjectileCallBack);
            yield return new WaitForSeconds(sec);
        }
    }

    private void SingleShot()
    {
        Projectile projectile = pool.Rent();
        projectile.Shoot(firingForce * firingOrigin.transform.forward, ProjectileCallBack);
    }
}
