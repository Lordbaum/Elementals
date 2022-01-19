using UnityEngine;

namespace Old
{
    public class drag : MonoBehaviour
    {
        public const float raylength = 10;
        public Rigidbody rigid;
        public Transform trans;
        public Transform camaratrans;
        public Transform playertrans;
        public bool uselockeddistance;
        public float lockeddistance = 10;
        public Transform raytrans;
        private bool first_drag = true;
        private bool first_press1 = true;
        private bool first_press2;
        private Vector3 firstpos;
        private float gamma;
        private RaycastHit hit;
        private Vector3 mOffset;
        private float mZCoord;
        private Vector3 oldcamtransforwardpos;
        private Vector3 oldcamtranspos;
        private Vector3 oldtranspos;
        private float oldx;
        private float oldy;
        private float oldz;
        private float phie;
        private float r;
        private float relativex;
        private float relativey;
        private float relativez;
        private Vector3 startpos;


        // Start is called before the first frame update
        void Start()
        {
            startpos = trans.position;
            rigid.useGravity = false;
            print("started");
            rigid.freezeRotation = true;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (first_drag) trans.position = startpos;
            Debug.DrawRay(camaratrans.position, camaratrans.forward, Color.red);
            if (Physics.Raycast(camaratrans.position, camaratrans.forward, out hit, raylength) &&
                hit.rigidbody == rigid ||
                first_press2)
            {
                raytrans = hit.transform;
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
                    mOffset = gameObject.transform.position - GetMouseWorldPos();

                    if (first_press1)
                    {
                        rigid.gameObject.layer = 2;
                        if (first_drag)
                        {
                            first_drag = false;
                            rigid.freezeRotation = false;
                        }

                        first_press1 = false;
                        first_press2 = true;
                        rigid.useGravity = false;


                        if (uselockeddistance)
                        {
                            r = lockeddistance;
                        }
                        else
                        {
                            firstpos = trans.position;
                            r = Vector3.Distance(firstpos,
                                new Vector3(camaratrans.position.x - oldcamtranspos.x,
                                    camaratrans.position.y - oldcamtranspos.y,
                                    camaratrans.position.z - oldcamtranspos.z));
                        }

                        if (Vector3.Distance(camaratrans.position, hit.transform.position) < r)
                            r = Vector3.Distance(camaratrans.position, hit.transform.position);
                    }
                    else
                    {
                        first_press1 = true;
                        first_press2 = false;
                        rigid.useGravity = true;
                        rigid.velocity = new Vector3((trans.position.x - oldx) / Time.deltaTime,
                            (trans.position.y - oldy) / Time.deltaTime, (trans.position.z - oldz) / Time.deltaTime);
                        rigid.gameObject.layer = 0;
                    }
                }
            }

            if (first_press2)
            {
                OnMouseOver();
                /*phie = camaratrans.rotation.x* r / (r / 2);
            gamma = playertrans.rotation.y* r / (r / 2);
            /*trans.position
            rigid.MovePosition(new Vector3(
                r * Mathf.Sin(gamma) * Mathf.Cos(phie) + camaratrans.position.x,
                r * Mathf.Sin(phie) * -1 + camaratrans.position.y
                , r * Mathf.Cos(phie) * Mathf.Cos(gamma) + camaratrans.position.z
            )); */
                oldtranspos = trans.position;
                oldx = trans.position.x;
                oldy = trans.position.y;
                oldz = trans.position.z;
            }

            oldcamtranspos = camaratrans.position;
            oldcamtransforwardpos = camaratrans.forward;
        }

        private void OnMouseOver()
        {
            transform.position = GetMouseWorldPos() + mOffset;
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousepoint = Input.mousePosition;
            mousepoint.z = mZCoord;
            return Camera.main.ScreenToWorldPoint(mousepoint);
        }
    }
}