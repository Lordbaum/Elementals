using Elements;
using MovmentUndSo;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : NetworkBehaviour
{
    public bool inMenu;

    // Start is called before the first frame update
    void Start()
    {
        InMenu(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            //öffnet das Menü
            InMenu(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inMenu)
        {
            //schließt das Menü
            InMenu(false);
        }
    }

    public void InMenu(bool isInMenu)
    {
        inMenu = isInMenu;
        //schaltet die sichtbarkeit des Meüs und des Cursors um
        Cursor.visible = isInMenu;
        transform.GetComponent<Canvas>().enabled = isInMenu;
        //schaltet um ob der Cursr gelocked ist oder nicht
        if (!isInMenu)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

//schaltet die bewegungs freiheit um
        transform.parent.gameObject.GetComponent<RigidbodyMovment>().isFreezed = isInMenu;
        transform.parent.gameObject.GetComponent<Elementselect>().activated = !isInMenu;
        transform.parent.gameObject.GetComponentInChildren<Camara>().isfreezed = isInMenu;
    }

    public void BackToMainMenu()
    {
        //lädt das MainMenu
        SceneManager.LoadScene(0);
        //setzt den ConnectionMode zurück
        DisconectClientServerRpc(NetworkManager.Singleton.LocalClientId);
        Destroy(NetworkManager.Singleton);
    }

    [ServerRpc]
    private void DisconectClientServerRpc(ulong Clientid)
    {
        NetworkManager.Singleton.DisconnectClient(Clientid);
    }
}