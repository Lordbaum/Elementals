using System;
using Generall;
using UnityEngine;

namespace Elements
{
    public class Camara_using_drag : MonoBehaviour
    {
        //länge des Raycast
        public float raylength = 15;

        [Tooltip("the Tranform of the Object which gets holded")]
        public Transform trans;

        public Transform camaratrans;

// ob ein Objekt gehalten wird
        public bool holding;
        public float throwForce = 500f;
        public Transform holdParent;
        public float moveForce = 250;

        [Tooltip("a test rigid for error preventing")]
        public Rigidbody errofixRigid;

        public GameObject hitSafe;
        public GameObject stoneCast, waterCast, fireCast;

        public Elementselect elementselect;

        public bool isdroping;
        private bool canBeeDroped;
        private GameObject castedObject;
        private Collider coli;

        private Objectdata data;
        private RaycastHit hit;

        //rigidbody und Tranform des Objekts
        private Rigidbody rigid;
        private int selctedLevel;

        private Vector3 velocityOldpos;
        // Update is called once per frame

        private void Start()
        {
            // Error beseitigung

            rigid = errofixRigid;
            trans = rigid.transform;
        }


        public void Update()
        {
            isdroping = false;
            //passt atribute an level an
            selctedLevel = elementselect.GetElementLevel();
            if (selctedLevel > 1)
            {
                raylength *= selctedLevel;
                throwForce *= selctedLevel * 0.75f;
                moveForce *= selctedLevel * 0.75f;
            }

            Debug.DrawRay(camaratrans.position, camaratrans.forward, Color.red);
            // kreiert einen Strahl der checkt ob es mit etwas colidiert
            if (Physics.Raycast(camaratrans.position, camaratrans.forward, out hit, raylength))
            {
                //speichert den Tranform des hits ab um fehler zu verhindern
                try
                {
                    if (hit.transform != null)
                        hitSafe = hit.transform.gameObject;
                }
                catch (NullReferenceException)
                {
                    print("NullReferenceException caught");
                }

//Schaut ob das Objekt bendigbar ist und ob das Elment mit dem ausgewählten Element übereinstimt
                if (!holding && hitSafe.TryGetComponent(out Objectdata objectdata))
                {
                    System.Diagnostics.Debug.Assert(trans != null, nameof(trans) + " != null");
                    if ((objectdata.bendigbar || trans.GetComponent<Objectdata>().bendigbar) &&
                        elementselect.IsElementActive(objectdata.element))
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse1))
                        {
//speichert wichtige daten ab
                            rigid = hit.rigidbody;
                            trans = hitSafe.transform;
                            data = trans.gameObject.GetComponent<Objectdata>();
                            coli = trans.GetComponent<Collider>();
                            PickUp();
                            canBeeDroped = false;
                        }
                    }

                    //Schaut ob das Objekt castbar ist und ob das Elment mit dem ausgewählten Element übereinstimt
                    if (Input.GetKeyDown(KeyCode.Mouse1) && !holding)
                    {
                        if (hitSafe.GetComponent<Objectdata>().casting &&
                            elementselect.IsElementActive(hitSafe.GetComponent<Objectdata>().element[0]))
                        {
                            Casting(hitSafe.GetComponent<Objectdata>().element[0]);
                        }
                        else if (hitSafe.GetComponent<Objectdata>().isBurning && elementselect.IsElementActive("Fire"))
                        {
                            Casting("Fire");
                        }
                    }
                }
            }

            //wenn das Obejkt gehalten wird
            if (holding)
            {
                holdParent.rotation = camaratrans.parent.rotation;


                if (Input.GetKeyDown(KeyCode.Mouse0))
                    Throw();
                else if (Input.GetKeyDown(KeyCode.Mouse1) && canBeeDroped)
                {
                    //berechnet die Geschwindigkeit des Objektes in der Hand
                    var position = trans.position;
                    rigid.velocity = new Vector3(
                        Mathf.Clamp((position.x - velocityOldpos.x) / Time.deltaTime,
                            -20f * elementselect.GetElementLevel(), 20f * elementselect.GetElementLevel()),
                        Mathf.Clamp((position.y - velocityOldpos.y) / Time.deltaTime,
                            -20f * elementselect.GetElementLevel(), 20f * elementselect.GetElementLevel()),
                        Mathf.Clamp((position.z - velocityOldpos.z) / Time.deltaTime,
                            -20f * elementselect.GetElementLevel(), 20f * elementselect.GetElementLevel()));
                    Drop();
                }


                velocityOldpos = trans.position;
            }


            canBeeDroped = true;
            if (!holding)
            {
                hitSafe = null;
            }

            try
            {
                if (trans == null || rigid == null)
                {
                }
            }
            catch (NullReferenceException)
            {
                trans = errofixRigid.transform;
                rigid = errofixRigid;
            }


            if (selctedLevel > 1)
            {
                raylength /= elementselect.GetElementLevel();
                throwForce = throwForce / (selctedLevel * 0.75f);
                moveForce = moveForce / (selctedLevel * 0.75f);
            }
        }

        void FixedUpdate()
        {
            try
            {
                if (!holding) return;
                if (Vector3.Distance(trans.position, holdParent.position) > 0.25f)
                {
                    // fügt dem Objekt eine Kraft zum holdParnet hinzu
                    Vector3 moveDirection = (holdParent.position - trans.position);
                    rigid.AddForce(moveDirection * moveForce);
                }
                else
                {
                    rigid.velocity = new Vector3(0, 0, 0);
                }
            }
            catch (MissingReferenceException e)
            {
                rigid = errofixRigid;
                trans = errofixRigid.transform;
                Console.WriteLine(e);
                throw;
            }
        }

        void Drop()
        {
            isdroping = true;
            holding = false;
            rigid.drag = 0f;
            rigid.freezeRotation = false;
            rigid.useGravity = true;
            rigid.gameObject.layer = 0;
            rigid.isKinematic = false;
            data.IsHolding(holding);
            data = null;
            coli = null;
            rigid.transform.parent = null;
            trans = errofixRigid.transform;
            rigid = errofixRigid;
        }

        void PickUp()
        {
            rigid.gameObject.layer = 2;
            holding = true;
            data.IsHolding(holding);
            rigid.useGravity = false;
            rigid.transform.parent = holdParent;
            rigid.drag = 15f;
        }

        void Throw()
        {
            rigid.AddForce(holdParent.forward * throwForce);
            Drop();
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }

        private void Casting(String element)
        {
            canBeeDroped = false;
            switch (element)
            {
                case "Earth":
                    castedObject = Instantiate(stoneCast, hit.point, holdParent.rotation, holdParent);
                    break;
                case "Water":

                    castedObject = Instantiate(waterCast, hit.point, holdParent.rotation, holdParent);
                    break;
                case "Fire":
                    castedObject = Instantiate(fireCast, hit.point, holdParent.rotation, holdParent);
                    hitSafe.gameObject.GetComponent<Objectdata>().StopBurning();
                    break;
            }

            data = castedObject.GetComponent<Objectdata>();
            data.SetData(new[] {element}, true);
            rigid = castedObject.GetComponent<Rigidbody>();
            trans = castedObject.transform;
            PickUp();
            castedObject = null;
        }
    }
}