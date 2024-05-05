using UnityEngine;

public class Deadly : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        damageable?.TakeDamage(666);
    }
}
