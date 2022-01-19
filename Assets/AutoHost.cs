using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoHost : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneLoadAndInformation.ConectionMode == "Host")
        {
            NetworkManager.Singleton.StartHost();
        }
        else if (SceneLoadAndInformation.ConectionMode == "Client")
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    public void Update()
    {
        try
        {
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost &&
                !NetworkManager.Singleton.IsServer &&
                SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
            {
                SceneManager.LoadScene(0);
            }
        }
        catch (Exception)
        {
        }
    }
}