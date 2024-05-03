using System;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int projectileDamage;
    [SerializeField] private float duration;
    private Action<Projectile> callBack;
    private Rigidbody2D myRigidbody2D;
    private bool isShooting = false;
    private float currentDuration;

    private void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isShooting) return;

        currentDuration += Time.deltaTime;

        if (currentDuration >= duration)
        {
            isShooting = false;
            callBack?.Invoke(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        int otherLayer = collision.gameObject.layer;
        if (this.gameObject != null && otherLayer != LayerMask.NameToLayer("Firing_tower"))
        {
            Destroy(this.gameObject);
        }
        if (damageable == null)
        {
            return;
        }
        damageable.TakeDamage(projectileDamage);
    }

    public void Shoot(Vector2 force, Action<Projectile> callBack)
    {
        myRigidbody2D.velocity = Vector2.zero;
        gameObject.SetActive(true);
        this.callBack = callBack;
        myRigidbody2D.AddForce(force, ForceMode2D.Impulse);
        currentDuration = 0;
        isShooting = true;
    }
}
