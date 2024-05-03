using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Transform spawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController == null) return;

        Debug.Log($"FOUND CHECKPOINT {index}, SPAWN POSITION: {spawnPosition.position}");
        playerController.SetNewCheckpoint(spawnPosition.position, index);
    }
}
