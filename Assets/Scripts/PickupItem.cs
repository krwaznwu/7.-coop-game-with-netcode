using UnityEngine;
using Unity.Netcode;

public class PickupItem : NetworkBehaviour
{
    public int scoreValue = 1; // Example: Points the item is worth

    // Delegate and event for notifying spawners when this item despawns
    public delegate void ItemDespawned();
    public static event ItemDespawned OnItemDespawned;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only the server handles the pickup logic

        if (other.CompareTag("Player")) // Ensure the colliding object is a player
        {
            // Notify the server to handle the pickup
            HandlePickupServerRpc(other.GetComponent<NetworkObject>().OwnerClientId);

            // Notify listeners (e.g., ItemSpawner) that this item is despawning
            OnItemDespawned?.Invoke();

            // Despawn the item across the network
            GetComponent<NetworkObject>().Despawn();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HandlePickupServerRpc(ulong clientId)
    {
        // Update the player's score or other logic here
        Debug.Log($"Player {clientId} picked up {gameObject.name}");
    }
}
