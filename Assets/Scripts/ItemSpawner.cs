using UnityEngine;
using Unity.Netcode;

public class ItemSpawner : NetworkBehaviour
{
    public GameObject itemPrefab;   // Your pickup item prefab
    public float spawnHeight = 20f; // Height from which items will fall (above the visible area)
    public Vector2 spawnRange = new Vector2(-5f, 5f); // X-axis range for random spawn positions
    public float spawnInterval = 5f;  // Time interval between item spawns
    public int maxItems = 7;          // Maximum number of items allowed in the scene
    private int currentItemCount = 0; // Tracks the number of items currently in the scene
    private int fastSpawnCount = 7;   // Number of items to spawn quickly
    private float fastSpawnInterval; // Shortened interval for the initial spawns

    private void OnEnable()
    {
        // Subscribe to the OnItemDespawned event
        PickupItem.OnItemDespawned += OnItemDespawn;
        // Subscribe to the NetworkManagerUI event
        NetworkManagerUI.OnServerOrHostStarted += OnServerOrHostStartedHandler;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnItemDespawned event
        PickupItem.OnItemDespawned -= OnItemDespawn;
        // Unsubscribe from the NetworkManagerUI event
        NetworkManagerUI.OnServerOrHostStarted -= OnServerOrHostStartedHandler;
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsHost)
        {
            Debug.Log("This is a client, not spawning items.");
        }
    }

    private void OnServerOrHostStartedHandler()
    {
        if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Server or Host started, starting item spawn logic...");
            fastSpawnInterval = spawnInterval / 10f; // Set the quick spawn interval
            // Start spawning items with the shorter interval for the first few spawns
            InvokeRepeating(nameof(SpawnItemQuickly), fastSpawnInterval, fastSpawnInterval);
        }
    }

    private void SpawnItemQuickly()
    {
        Debug.Log("Attempting to spawn item quickly...");
        SpawnItem();
        if (currentItemCount >= fastSpawnCount)
        {
            Debug.Log("Fast spawn count reached, switching to normal spawn interval...");
            CancelInvoke(nameof(SpawnItemQuickly));
            InvokeRepeating(nameof(SpawnItem), spawnInterval, spawnInterval);
        }
    }

    private void SpawnItem()
    {
        Debug.Log("Spawning item...");
        if (currentItemCount >= maxItems)
        {
            Debug.Log("Max items reached, not spawning more.");
            return;
        }
        float spawnX = Random.Range(spawnRange.x, spawnRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnHeight, 0);

        if (IsServer)
        {
            GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            NetworkObject networkObject = item.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn(true); // Force spawn on all clients
                Debug.Log("Item spawned on the network!");
            }
            else
            {
                Debug.LogError("NetworkObject component missing on prefab!");
            }
            currentItemCount++;
        }
        else
        {
            Debug.LogWarning("Attempted to spawn item on a non-server instance.");
        }
    }

    private void OnItemDespawn()
    {
        // Decrement the item count when an item is despawned
        currentItemCount--;
        Debug.Log("Item despawned, current item count: " + currentItemCount);
    }
}