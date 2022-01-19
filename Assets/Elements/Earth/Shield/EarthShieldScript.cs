using UnityEngine;

namespace Elements.Earth.Shield
{
    public class EarthShieldScript : MonoBehaviour
    {
        private Vector3 startPos;

        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position;
        }


        void Update()
        {
            //just moves the wallupwards
            if (!(transform.position.y > startPos.y + 2.5f))
            {
                transform.Translate(0, 0.065f, 0);
            }
        }
    }
}