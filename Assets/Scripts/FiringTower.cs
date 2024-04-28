using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FiringTower : MonoBehaviour
{
    private enum ShootMethod
    {
        TargeteningShoot,
        SingleShot,
        MultipleShot,
    }

    private Pool<Projectile> pool;
    private const int INITIAL_POOL_SIZE = 10;
    private const int PROJECTILES_AMOUNT = 4;

    [SerializeField] private ShootMethod method;
    [SerializeField] private Transform firingOrigin;
    [SerializeField] private float firingForce;
    [SerializeField] private float firingRate;
    [SerializeField] Vector2 firingDirection;
    private float delayBetweenBursts;
    private float currentTime = 0;

    private void Awake()
    {
        pool = new Pool<Projectile>(Constants.PROJECTILE_PREFAB, INITIAL_POOL_SIZE);
    }

    private void OnValidate()
    {
        delayBetweenBursts = 3;
        if (method != ShootMethod.MultipleShot)
        {
            delayBetweenBursts = 0;
        }
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > firingRate)
        {
            currentTime = 0 - delayBetweenBursts;
            Invoke(method.ToString(), firingRate);
        }
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
        RentAndShoot(firingForce * targetDirection.normalized);

    }

    private void ProjectileCallBack(Projectile projectile)
    {
        pool.TurnBack(projectile);
    }

    private void MultipleShot()
    {
        StopCoroutine("RapidFire");
        StartCoroutine(RapidFire(firingRate / PROJECTILES_AMOUNT));
    }

    IEnumerator RapidFire(float sec)
    {
        for (int i = 0; i < PROJECTILES_AMOUNT; i++)
        {
            RentAndShoot(firingForce * firingDirection);
            yield return new WaitForSeconds(sec);
        }
    }

    private void SingleShot()
    {
        RentAndShoot(firingForce * firingDirection);
    }

    private void RentAndShoot(Vector2 force)
    {
        Projectile projectile = pool.Rent();
        projectile.transform.SetPositionAndRotation(firingOrigin.position, Quaternion.identity);
        projectile.Shoot(force, ProjectileCallBack);
    }
}
