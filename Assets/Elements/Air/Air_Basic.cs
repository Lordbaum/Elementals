using Generall;
using MovmentUndSo;
using UnityEngine;

namespace Elements.Air
{
    public class Air_Basic : MonoBehaviour
    {
        public Transform cameratrans;

        public float windCutDelay = 0.1f;
        public float windShieldDelay = 10f;
        public float windPushDelay = 2f;
        public ParticleSystem windCutParticle;
        public ParticleSystem windPushParticle;
        public float pushForce;
        public float shieldlifetime;

        public GameObject windShield;
        private float level;
        private float windCutDelayCounter;
        private float windPushDelayCounter;
        private float windShieldDelayCounter;

        // Start is called before the first frame update
        void Start()
        {
            //checkt das level damit die Methode nicht andauernd ausgeführt wird
            level = GetComponent<Elementselect>().air;
            //setzt die Delays so das man anfangs kein delay hat
            windCutDelayCounter = windCutDelay;
            windPushDelayCounter = windPushDelay;
            windShieldDelayCounter = windShieldDelay;
        }

        // Update is called once per frame
        void Update()
        {
            //lässt die delay hochzählen
            windCutDelayCounter += Time.deltaTime;
            windPushDelayCounter += Time.deltaTime;
            windShieldDelayCounter += Time.deltaTime;
            //wenn Air nicht gewählt ist endet die Funktion hier
            if (!GetComponent<Elementselect>().IsElementActive("Air")) return;
            //wenn man die Linke Maustaste drückt und der Delay abglaufen ist wird die Methode/Attacke WindCut ausgeführt
            if (Input.GetKeyDown(KeyCode.Mouse0) && windCutDelayCounter >= windCutDelay)
            {
                WindCut();
            }

            //wenn man die Rechte Maustaste gedrückthält und der Delay abglaufen ist wird die Methode/Attacke WindPush ausgeführt
            if (Input.GetKey(KeyCode.Mouse1) && windPushDelayCounter >= windPushDelay)
            {
                WindPush();
            } //wenn man die Taste löslässt
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                //die Camera kann sich wieder bewegen
                cameratrans.GetComponent<Camara>().isfreezed = false;
                //Delay wird neu gestartet
                windPushDelayCounter = 0;
                //der Spieler kann sich wieder bewegen
                GetComponent<RigidbodyMovment>().isFreezed = false;
                //Die Partikel werden gestopt
                windPushParticle.Stop();
            }

//wenn man die E drückt und der Delay abglaufen ist wird die Methode/Attacke WindShield ausgeführt
            if (Input.GetKeyDown(KeyCode.E) && windShieldDelayCounter >= windShieldDelay)
            {
                WindShield();
            }
        }

        private void WindCut()
        {
            //Der Delay wird zurück gestzt
            windCutDelayCounter = 0;
            //partikel werden abgespielt
            windCutParticle.Play();
            //in einem bereich mit einer länge von 1 und einem Radius von 3 vor der Camera wird nach einem Object/collision geschaut
            //wenn dort eine istwird geschaut ob es das HealthScript besitzt wenn ja wird die funktion TakeDamage ausgeführt
            if (Physics.SphereCast(cameratrans.position, 1, cameratrans.forward, out RaycastHit hit, 1)
                && hit.transform.TryGetComponent(out Health health))
            {
                health.TakeDamage(5 * level, 0);
            }
        }

        private void WindPush()
        {
            //Die bewegungsfreiheit von camera und spieler wird blockiert

            cameratrans.GetComponent<Camara>().isfreezed = true;
            GetComponent<RigidbodyMovment>().isFreezed = true;
            // es wird in einem bereich einer länge von 7.5*level und einem Radius von dem wer des levels vor der Camera wird nach einem Object/collision gesucht
            if (Physics.SphereCast(cameratrans.position, level, cameratrans.forward, out RaycastHit hit, 7.5f * level))
            {
                //schaut ob das Objekt bendigbar ist
                if (hit.transform.TryGetComponent(out Objectdata data) && data.bendigbar)
                {
                    //nimmt das objectdata Script normalisert sein rigidbody und fügt diesem eine kraft in richtung des blickfeldes der Camera
                    data.NormalRigidbody().AddForce(cameratrans.forward * (pushForce * level));
                }
                //schaut nach einem rigidbody
                else if (hit.transform.TryGetComponent(out Rigidbody rigid))
                {
                    //fügt diesem eine kraft in richtung des Blickfeldes der Camera
                    rigid.AddForce(cameratrans.forward * (pushForce * level));
                    rigid.constraints = RigidbodyConstraints.None;
                }
                //schaut nach einem Playercontroller in richtung des Blickfeldes der Camera
                else if (hit.transform.TryGetComponent(out CharacterController controller))
                {
                    //und bewegt ihn
                    controller.Move(cameratrans.forward * (pushForce * level));
                }
            }

            //beim Drücken der rechten Maustaste
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //past die form und menge des Partikelsystems an die Attacke an
                var shapeModule = windPushParticle.shape;
                shapeModule.length = 7.5f * level;
                shapeModule.radius = level;
                var emmisionModule = windPushParticle.emission;
                emmisionModule.rateOverTimeMultiplier = 125 * level;
                //spielt es ab
                windPushParticle.Play();
            }
        }

        private void WindShield()
        {
            windShieldDelayCounter = 0;
            //kreiert das Windshild und setzt sein parent
            GameObject shield = Instantiate(windShield, transform);
            //setzt die position
            shield.transform.localPosition = new Vector3(0, 1, 2);
            //passt den colider des Schildes an das Level an
            shield.GetComponent<BoxCollider>().size = new Vector3(2.5f * level * 0.75f, 2.5f * level * 0.75f, 1);
            //übergibt daten an das script
            var script = shield.GetComponent<WindShield>();
            script.lifetime = shieldlifetime;
            script.level = level;
            //past das Particel system an
            ParticleSystem particel = shield.GetComponent<ParticleSystem>();
            ParticleSystem.ShapeModule particelShape = particel.shape;
            particelShape.radius = level * 0.75f;
            ParticleSystem.EmissionModule emmisionModule = particel.emission;
            emmisionModule.rateOverTimeMultiplier = 40 * level;
            //macht das Schild unabhängig vom Spieler
            shield.transform.parent = null;
        }
    }
}