using Unity.Netcode;
using UnityEngine;

public class ListnerDestroyer : MonoBehaviour
{
    public Camera cam;

    public AudioListener audio;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<NetworkObject>().IsLocalPlayer)
        {
            Destroy(cam);
            Destroy(audio);
        }
    }
}