using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int projectileDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();

        if (damageable == null)
        {
            return;
        }
        damageable.TakeDamage(projectileDamage);
    }
}
