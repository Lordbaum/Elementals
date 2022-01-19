using Unity.Netcode;
using UnityEngine;

namespace MovmentUndSo
{
    public class Camara : MonoBehaviour
    {
        // grundwerte
        public float mouseSensitivity = 300f;
        public float downClamp = 90f;
        public float upClamp = -90f;
        public Transform cameraHolder;
        public bool isfreezed;
        public float mouseX;
        public Transform playerBody;

        private float mouseY;

        // macht es möglich einen Transform anzuhängen
        float xRotation = 0f;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!GetComponentInParent<NetworkObject>().IsLocalPlayer || isfreezed) return;
            // position der Maus wird mal die Sensivität mal die in game zeit genommen damit es nicht framerate unabhängig ist

            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            // sorgt fürs hoch und runter schauen
            xRotation -= mouseY;
            //sorgt für einen maxmum und minimum winkel
            xRotation = Mathf.Clamp(xRotation, upClamp, downClamp);
            //fügt es zu Camara hinzu
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            //blockiert Mouse lock
            playerBody.Rotate(0, mouseX, 0);
        }
    }
}