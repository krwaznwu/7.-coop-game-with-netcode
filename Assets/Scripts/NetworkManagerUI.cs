using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    public static event System.Action OnServerOrHostStarted;

    private void Awake()
    {
        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server started.");
            OnServerOrHostStarted?.Invoke();
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started.");
            OnServerOrHostStarted?.Invoke();
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client is running.");
        });
    }
}