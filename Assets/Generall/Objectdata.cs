using System.Linq;
using UnityEngine;

namespace Generall
{
    public class Objectdata : MonoBehaviour
    {
        public bool bendigbar = true;
        public bool casting;
        public string[] element;
        public bool destroyOnCollision;
        public float damageMultiplier = 1;

        public bool isBurning;
        public bool isFlamable;
        public float fuelTime = 5f;
        public GameObject flamePrefab;
        public Vector3 flameSize;
        private float averageVelocity;

        private float damageDealt;
        private float extraDamage;
        private float fireProtection = 1f;
        private GameObject flame;
        private Renderer flameRenderer;
        private bool holding;
        private int i;
        private Rigidbody rigid;
        private float timeBurned;


        // Start is called before the first frame update
        void Start()
        {
            if (!TryGetComponent(out rigid)) return;
            rigid.useGravity = false;
            rigid.constraints = RigidbodyConstraints.FreezeAll;
            rigid.isKinematic = false;
        }

// Update is called once per frame

        void Update()
        {
            if (isBurning)
            {
                Burning();
            }

//sorgt fürs Ausbrennen
            if (timeBurned >= fuelTime)
            {
                Destroy(gameObject);
            }

//zählt die fireProtection runter
            if (fireProtection > 0 && !isBurning)
            {
                fireProtection -= Time.deltaTime;
            }
        }

        public void OnCollisionEnter(Collision other)
        {
            //sorgt für die Zerstörung bei Berührung wenn sie enabeld ist und das Object nicht gehalten wird.
            if (destroyOnCollision && !holding)
            {
                Destroy(gameObject, 0.2f);
            }

//überprüft ob das Objekt überhaupt ein rigidbody besitzt
            if (rigid != null)
            {
//berechnet die durschnitliche Geschwindigkeit des Objektes
                var rigidVelo = rigid.velocity;
                averageVelocity = Mathf.Abs(rigidVelo.x) + Mathf.Abs(rigidVelo.y) + Mathf.Abs(rigidVelo.z) / 3;
//schaut ob das Objekt gegenüber leben hat und Ob die eigen Geschwindigkeit größer als 1 ist damit der Spielr kein Schaden nimmt wenn er einfach nur dagegen läuft
                if (other.gameObject.TryGetComponent(out Health health) && averageVelocity >= 1)
                {
                    var realtiveVelo = other.relativeVelocity;
                    //berechnet den Schaden aufgrund dem durschnitt der relativen Geschwindigkeit
                    damageDealt = Mathf.Abs(realtiveVelo.x) + Mathf.Abs(realtiveVelo.y) + Mathf.Abs(realtiveVelo.z) / 3;
                    //veringert den Damage wenn man ein Object hält
                    if (holding) damageDealt /= 4;
                    //sorgt für extra damage
                    while (damageDealt > 10 && i < damageDealt && !holding)
                    {
                        extraDamage += 5;
                        i += 10;
                    }

//fügt den Damage zu
                    health.TakeDamage((damageDealt + extraDamage) * damageMultiplier, 1f, true, true);
                }
            }

//schaut nach der ObjectData des Colidirendem Object
            if (other.gameObject.TryGetComponent(out Objectdata data))
            {
                //wenn Das andere Object brennt und this.Object brennbar ist nicht selbst brennt und die fireprotection auf null ist fängt es an zu brennen
                if (data.isBurning && isFlamable && !isBurning && fireProtection <= 0)
                {
                    StartBurning();
                }
                //wenn das andere Object das Element Wasser besitzt wird this.Object gelöscht
                else if (data.HasElement("Water"))
                {
                    StopBurning();
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Objectdata data))
            {
                //wenn Das andere Object brennt und this.Object brennbar ist nicht selbst brennt und die fireprotection auf null ist fängt es an zu brennen
                if (data.isBurning && isFlamable && !isBurning && fireProtection <= 0)
                {
                    StartBurning();
                }
                //wenn das andere Object das Element Wasser besitzt wird this.Object gelöscht
                else if (data.HasElement("Water"))
                {
                    StopBurning();
                }
            }
        }


        public void IsHolding(bool holding)
        {
            this.holding = holding;
            NormalRigidbody();
        }

        public void SetData(string[] element, bool destroyOnCollision)
        {
            this.element = element;
            this.destroyOnCollision = destroyOnCollision;
        }

        public Rigidbody NormalRigidbody()
        {
            //geht sicher ob rigid nicht null ist
            if (rigid == null) rigid = GetComponent<Rigidbody>();
//sorgt dafür das sich der/die/das Rigidbody frei bewegen kann
            rigid.useGravity = true;
            rigid.constraints = RigidbodyConstraints.None;
            rigid.isKinematic = false;

            return rigid;
        }

        public bool HasElement(string element)
        {
            //schaut nach ob das geforderte element vorhanden ist
            foreach (string s in this.element.Where(s => s == element))
            {
                return true;
            }

            return false;
        }

        private void Burning()
        {
            timeBurned += Time.deltaTime;
        }


        public void StartBurning()
        {
            isBurning = true;
            //erzeugt das Feuer
            flame = Instantiate(flamePrefab, gameObject.transform);
//bearbeitet das Feuer
            flame.transform.localScale = flameSize;
            flame.transform.position = transform.GetComponent<Renderer>().bounds.center;
            //lässt es spielen und stopen bei einer Exception
            try
            {
                flame.GetComponent<ParticleSystem>().Play();
            }
            catch (System.Exception)
            {
                flame.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }

        public void StopBurning()
        {
            //wenn es brennt und die flamme nicht null ist
            if (isBurning && flame != null)
            {
                // es brennt nicht mehr und die Fireprotection wird auf eine Skunde gestzt
                isBurning = false;
                fireProtection = 1f;
                try
                {
                    //stoppt den Partikeleffect
                    flame.GetComponent<ParticleSystem>().Stop();
                }
                catch (System.Exception)
                {
                    //bei einer Exception wird geschaut ob das Particelsystem in den Childern ist
                    flame.GetComponentInChildren<ParticleSystem>().Stop();
                }
            }
        }
    }
}