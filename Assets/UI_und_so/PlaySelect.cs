using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySelect : MonoBehaviour
{
    public GameObject playSelect;

    public GameObject mainMenu;

    //startet die map als Singleplayer
    public void Singleplayer()
    {
        SceneLoadAndInformation.ConectionMode = "Single";
        //wechselt zu nächsten scence
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //startet die map als Host
    public void Host()
    {
        SceneLoadAndInformation.ConectionMode = "Host";
        //wechselt zu nächsten scence
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //startet die map als Client
    public void Connect()
    {
        SceneLoadAndInformation.ConectionMode = "Client";
        //wechselt zu nächsten scence
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

//gos back to the titel Screen
    public void Back()
    {
        playSelect.active = false;
        mainMenu.active = true;
    }
}