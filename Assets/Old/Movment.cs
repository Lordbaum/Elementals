using Elements;
using UnityEngine;

namespace MovmentUndSo
{
    public class Movment : MonoBehaviour
    {
        public CharacterController controller;
        public float normalspeed = 12f;
        public float dashmultiplier = 3f;
        public float gravity = -8.81f;
        public float jumphight = 3f;
        public Transform groundcheck;
        public float groundDistance;
        public LayerMask groundMask;
        public float airresitend = 1f;
        public bool isfreezed;
        public ParticleSystem airJump;
        public float climbingSpeed = 3f;
        private bool dashing;
        private Elementselect elementselect;
        private int extraJumps;
        bool isGrounded;
        private Vector3 move;
        private float speed;
        float sprintmultiplier = 1.25f;
        Vector3 velocity;

        void Start()
        {
            elementselect = GetComponent<Elementselect>();
            extraJumps = elementselect.air;
        }

        // Update is called once per frame
        private void Update()
        {
            isGrounded = Physics.CheckSphere(groundcheck.position, groundDistance, groundMask);
            // checks if it is grounded
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            //Cordinaten der Maus
            if (Input.GetButtonDown("Jump") && !isfreezed &&
                (isGrounded || elementselect.IsElementActive("Air") && extraJumps > 0))
            {
                if (!isGrounded)
                {
                    airJump.Play();
                    extraJumps--;
                }

                velocity.y = Mathf.Sqrt(jumphight * -2 * gravity);
                //berechnet die geschwindigkeit des Sprungs
            }

            move = transform.right * x + transform.forward * z + transform.up * 0;
            if (isGrounded)
            {
//wenn man auf dem Boden ist
                speed = normalspeed;
                extraJumps = elementselect.GetElementLevel("Air");
                if (Input.GetKey(KeyCode.LeftShift)) speed = sprintmultiplier * normalspeed;
// normales sprinten
                if (Input.GetKeyDown(KeyCode.LeftControl) || dashing)
                {
                    speed = dashmultiplier * normalspeed;
                }

//dashen
                if (isfreezed)
                {
                    speed = 0;
                } //for freezing the position

                controller.Move(move * speed * Time.deltaTime);
            }
            else
            {
                if (speed >= 0) speed -= airresitend;
                else speed = 0f;
                controller.Move(move * speed * Time.deltaTime);
                //movment in der Luft
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
            //Bewegung
            if (Input.GetKeyDown(KeyCode.P))
            {
                transform.position = new Vector3(3, 4, 34);
            }
        }

        private void OnCollisionStay(Collision other)
        {
            //        if (elementselect.IsElementActive("Plant")&&other.gameObject.TryGetComponent(out Objectdata data)
            //        )if(data.element == "Plant"&&Input.GetKey(KeyCode.Space))
            //        {
//
//
            //            controller.Move(transform.up * (climbingSpeed * elementselect.plant) * Time.deltaTime);
            //        }
            //    }
        }
    }
}