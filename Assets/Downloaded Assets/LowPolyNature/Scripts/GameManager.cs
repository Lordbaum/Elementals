using UnityEngine;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        public PlayerController Player;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            // Dont destroy on reloading the scene
            DontDestroyOnLoad(gameObject);
        }
    }
}