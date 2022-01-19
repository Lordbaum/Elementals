using System;
using System.Collections;
using System.Collections.Generic;
using Elements.Earth.Prime;
using Generall;
using UnityEngine;

namespace Elements.Earth
{
    public class Earth_Basic : MonoBehaviour
    {
        public GameObject primeAttackBlueprint;
        public Transform groundAttack;
        public GameObject createdObject;
        public float primeAttackDelay = 0.5f;
        public float shildDelay = 3f;
        public float raylength = 10f;
        public Transform camaratrans;
        public GameObject wall1;
        public GameObject wall2;
        public float wallHealth;

        private Collider colider;
        private MeshCollider collider;
        private GameObject createdWallL;
        private GameObject createdWallM;
        private GameObject createdWallR;
        private float delayCounterPrime = 0.5f;
        private float delayCounterShield = 10f;
        private Elementselect elementselect;
        private List<Transform> groundAttacks = new List<Transform>();
        private Health health;

        private int i = 1;

        // Start is called before the first frame update
        void Start()
        {
            elementselect = GetComponent<Elementselect>();


            while (i <= groundAttack.childCount)
            {
                groundAttacks.Add(groundAttack.GetChild(i++ - 1));
            }

            i = 1;

            raylength *= elementselect.earth;
        }

        // Update is called once per frame
        void Update()
        {
            delayCounterPrime += Time.deltaTime;
            delayCounterShield += Time.deltaTime;
//Blockiert wenn das Elment Earth nicht ausgrüßtet ist
            if (!elementselect.IsElementActive("Earth"))
            {
                return;
            }

//Schaut ob die Tastegedrückt wird, ob der Delay abgelaufen ist und ob gerade ein Objekt geahlten oder gedropt wird
            if (!GetComponent<Camara_using_drag>().holding && Input.GetKeyDown(KeyCode.Mouse0) &&
                !GetComponent<Camara_using_drag>().isdroping && delayCounterPrime >= primeAttackDelay)
                EarthAttack();
            //schaut nach der Taste E und nach dem Delay
            if (Input.GetKeyDown(KeyCode.E) && delayCounterShield >= shildDelay) EarthShield();
        }

        private void EarthAttack()
        {
            delayCounterPrime = 0f;
            //kriert für jedes Level eine Attake
            while (i <= elementselect.GetElementLevel("Earth"))
            {
                try
                {
                    //fürt methoden aus i reprenstiert das level
                    StartCoroutine(CreateAttack(i, groundAttacks[i - 1]));

                    //SichertExceptions ab
                }
                catch (IndexOutOfRangeException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }

                i++;
            }

            i = 1;
            groundAttack.rotation = transform.rotation;
        }

        IEnumerator CreateAttack(float level, Transform attackPosition)
        {
            //wenn "attackPosition" den boden bürhrt der Erde ausgrüßtethat
            if (attackPosition.GetComponent<EarthCollisionCheck>().earthCheck)
            {
//kreiert das Objekt
                createdObject = Instantiate(primeAttackBlueprint
                    , new Vector3(attackPosition.position.x, attackPosition.position.y - (3f + (5 - level) * 0.1f), attackPosition.position.z),
                    new Quaternion(-170, 0, 0, 0), groundAttack);
//verändert die größen
                createdObject.transform.localScale = new Vector3(createdObject.transform.localScale.x,
                    createdObject.transform.localScale.y + 4 + level, createdObject.transform.localScale.z);
                //fügt Componeten hinzu
                createdObject.AddComponent<EarthPrime>().level = level;
                createdObject.AddComponent<MeshCollider>().convex = true;
                createdObject.transform.parent = null;
                createdObject = null;
                //Partikel
                attackPosition.GetComponent<ParticleSystem>().Play();
                //Delay
                yield return new WaitForSecondsRealtime(3f);
            }

            yield return new WaitForSecondsRealtime(0f);
        }

//kreiert das ErdSchild
        private void EarthShield()
        {
            //Schaut ob der Spieler auf ein Objekt mit dme Element "Earth" schaut und ob der Delay vergangen ist.
            if (Physics.Raycast(camaratrans.position, camaratrans.forward, out RaycastHit hit, raylength) &&
                hit.transform.TryGetComponent(out Objectdata data) &&
                data.HasElement("Earth") && delayCounterShield >= shildDelay)
            {
                //setz den Delay zurück
                delayCounterShield = 0f;
                //Sucht für das Entsprechende Level eine aktion raus
                switch (elementselect.GetElementLevel("Earth"))
                {
                    //generiert eine Level 1 Mauer
                    case 1:
                        createdObject = Instantiate(wall1, hit.point, wall1.transform.rotation * transform.rotation,
                            transform);
                        createdObject.transform.parent = null;
                        break;
                    case 2:
                        //genereiert eine Level 2 Mauer
                        createdObject = Instantiate(wall2, hit.point, wall2.transform.rotation * transform.rotation,
                            transform);
                        createdObject.transform.parent = null;
                        break;
                    // generiert 3 Level 1 Mauern
                    case 3:
                        /*generiert eine art Zentrum als neus Gameobject und fügt ihm position mit hilfe des Raycast hits
                 und eine rotation anhand der Rotation des Spielers*/
                        createdObject = new GameObject("creater");
                        createdObject.transform.position = hit.point;
                        createdObject.transform.rotation = transform.rotation;
                        //generiert die MittelMauer und gibt ihm seine Position
                        createdWallM = Instantiate(wall1, createdObject.transform);
                        createdWallM.transform.localPosition = new Vector3(0, -1, 3);
                        //generiert die linke Mauer und gibt ihm seine Position und rotation
                        createdWallL = Instantiate(wall1, createdObject.transform);
                        createdWallL.transform.localRotation = new Quaternion(0, -20, 0, 5);
                        createdWallL.transform.localPosition = new Vector3(-5.63f, -1, -0.5f);
                        //generiert die rechte Mauer und gibt ihm seine Position und rotation
                        createdWallR = Instantiate(wall1, createdObject.transform);
                        createdWallR.transform.localRotation = new Quaternion(0, -20, 0, -5);
                        createdWallR.transform.localPosition = new Vector3(5.63f, -1, -0.5f);
                        //spielt die partikel ab
                        foreach (ParticleSystem p in createdObject.GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Play();
                        }

                        //Deaktivert die parents und löscht das zentrum
                        createdWallM.transform.parent = null;
                        createdWallL.transform.parent = null;
                        createdWallR.transform.parent = null;
                        Destroy(createdObject);
                        break;
                    //kreiert eine level2 mittelmauer und 2 level1 mittelmauer
                    case 4:
                        createdObject = new GameObject("creater");
                        createdObject.transform.position = hit.point;
                        createdObject.transform.rotation = transform.rotation;
                        createdWallM = Instantiate(wall2, createdObject.transform);
                        createdWallM.transform.localPosition = new Vector3(0, -1, 3);
                        createdWallL = Instantiate(wall1, createdObject.transform);
                        createdWallL.transform.localRotation = new Quaternion(0, -20, 0, 5);
                        createdWallL.transform.localPosition = new Vector3(-5.63f, -1, -0.5f);
                        createdWallR = Instantiate(wall1, createdObject.transform);
                        createdWallR.transform.localRotation = new Quaternion(0, -20, 0, -5);
                        createdWallR.transform.localPosition = new Vector3(5.63f, -1, -0.5f);
                        foreach (ParticleSystem p in createdObject.GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Play();
                        }

                        createdWallM.transform.parent = null;
                        createdWallL.transform.parent = null;
                        createdWallR.transform.parent = null;
                        Destroy(createdObject);
                        break;
                    // kreiert 3 level2 Mauern
                    case 5:
                        createdObject = new GameObject("creater");
                        createdObject.transform.position = hit.point;
                        createdObject.transform.rotation = transform.rotation;
                        createdWallM = Instantiate(wall2, createdObject.transform);
                        createdWallM.transform.localPosition = new Vector3(0, -1, 3);
                        createdWallL = Instantiate(wall2, createdObject.transform);
                        createdWallL.transform.localRotation = new Quaternion(0, -20, 0, 5);
                        createdWallL.transform.localPosition = new Vector3(-5.63f, -1, -0.5f);
                        createdWallR = Instantiate(wall2, createdObject.transform);
                        createdWallR.transform.localRotation = new Quaternion(0, -20, 0, -5);
                        createdWallR.transform.localPosition = new Vector3(5.63f, -1, -0.5f);
                        foreach (ParticleSystem p in createdObject.GetComponentsInChildren<ParticleSystem>())
                        {
                            p.Play();
                        }

                        createdWallM.transform.parent = null;
                        createdWallL.transform.parent = null;
                        createdWallR.transform.parent = null;
                        Destroy(createdObject);
                        break;
                }

//setzt die Methode zurück
                createdObject = null;
                health = null;
            }
        }
    }
}