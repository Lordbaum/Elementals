using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject playSelect;
    private bool fullScreen = true;

    public void Start()
    {
        //clears Information when come to the Titel Screen
        SceneLoadAndInformation.ClearStaticSceneInformation();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11) && fullScreen)
        {
            fullScreen = false;
            Screen.fullScreen = fullScreen;
        }
        else if (Input.GetKeyDown(KeyCode.F11) && !fullScreen)
        {
            fullScreen = true;
            Screen.fullScreen = fullScreen;
        }
    }

    public void Play()
    {
        mainMenu.active = false;
        playSelect.active = true;
    }


    public void QuitGame()
    {
        //beendet das Spiel
        Application.Quit();
    }
}