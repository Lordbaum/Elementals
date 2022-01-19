using Unity.Netcode;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Player " + GetComponent<NetworkObject>().OwnerClientId;
    }
}