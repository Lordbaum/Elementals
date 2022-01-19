using UnityEngine;

namespace Old
{
    public class ThirdpersonCamara : MonoBehaviour
    {
        public float mouseSensitivity = 100f;
        public Transform playerBody;
        public float downangelclamp = 90f;
        public float upangelclamp = -90f;
        public float minposclamp = -2f;
        public float maxposclamp = 2f;
        float Camarayposition = 0f;

        float xRotation = 0f;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            minposclamp -= transform.localPosition.y;
            maxposclamp += transform.localPosition.y;
        }

        // Update is called once per frame
        void Update()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            playerBody.Rotate(Vector3.up * mouseX);
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, downangelclamp, upangelclamp);
            Camarayposition = Mathf.Clamp(mouseY, minposclamp, maxposclamp);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            //transform.localPosition = new Vector3(0.05f, 2.158f-Camarayposition, -1.987f);
            if (transform.localPosition.y <= maxposclamp && mouseY <= 0)
            {
                transform.localPosition += new Vector3(0f, -mouseY / 100, 0f);
            }
            else if (transform.localPosition.y >= minposclamp && mouseY >= 0)
            {
                {
                    transform.localPosition += new Vector3(0f, -mouseY / 100, 0f);
                }
            }
        }
    }
}