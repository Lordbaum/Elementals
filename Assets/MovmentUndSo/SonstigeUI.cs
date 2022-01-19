using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SonstigeUI : MonoBehaviour
{
    public Text hostClienttxt;

    public NetworkManager networkManager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (networkManager != null)
        {
            if (networkManager.IsHost)
            {
                hostClienttxt.text = "Host";
            }
            else if (networkManager.IsClient) hostClienttxt.text = "Client";
        }
    }
}