using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public TextMeshPro hostClienttxt;

    public NetworkManager networkManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (networkManager.IsHost)
        {
            hostClienttxt.text = "Host";
        }
        else if (networkManager.IsClient) hostClienttxt.text = "Client";
    }
}