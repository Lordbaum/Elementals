using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Generall
{
    public class Health : MonoBehaviour
    {
        public float maxhealth = 100;
        public float health;

        public bool player;
        public bool isObject;

        public RectTransform hitmarker;
        public ParticleSystem blood;
        public bool bleed;

        public bool isBurning;

        public ParticleSystem flame;
        public bool isFlamable = true;
        public float burningTime;


        public GameObject ui;
        private float burntimer;
        private float damage;
        private ParticleSystem.EmissionModule emissionModule;

        private bool gotHit;
        private bool showHitmarker;
        private Vector3 startpos;

        // Start is called before the first frame update
        void Start()
        {
            health = maxhealth;
            startpos = hitmarker.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            //wenn das Leben unter oder gleich 0 fällt
            if (health <= 0f)
            {
                if (player) Die();
                else if (isObject) Destroying();
            }
            //setzt Leben zurück wenn über Max
            else if (health > maxhealth) health = maxhealth;

            if (gotHit || showHitmarker)
            {
                Hitmarker();
            }

            if (isBurning && burntimer <= burningTime)
            {
                TakeDamage(1f * Time.deltaTime, 0, false, false);
                burntimer += Time.deltaTime;
            }
            else
            {
                StopBurning();
                burntimer = 0;
            }

//schaut ob eine UI exsistiert
            if (ui != null)
            {
                UI();
            }
        }

        public void OnCollisionEnter(Collision other)
        {
            //wenn das andere Object brennt und man selbst entzündlich ist dan fängt es an zu brennen
            if (other.transform.TryGetComponent(out Objectdata data) && data.isBurning && isFlamable)
            {
                StartBurning();
                //aber wenn es Wasser Berührt dann geht die flamme aus
            }
            else if (other.transform.TryGetComponent(out Objectdata data2) && data2.HasElement("Water"))
            {
                StopBurning();
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            //wenn das andere Object brennt und man selbst entzündlich ist dan fängt es an zu brennen
            if (other.transform.TryGetComponent(out Objectdata data) && data.isBurning && isFlamable)
            {
                StartBurning();
                //aber wenn es Wasser Berührt dann geht die flamme aus
            }
            else if (other.transform.TryGetComponent(out Objectdata data2) && data2.HasElement("Water"))
            {
                StopBurning();
            }
        }

        private void Die()
        {
            if (bleed) blood.Play();
        }

        private void Destroying()
        {
            if (bleed) blood.Play();


            Destroy(gameObject);
        }

//heilt
        public void Heal(float healAmount, bool objectHealer, bool playerHealer)
        {
            //sichert den Heiltypen
            if (isObject && !objectHealer)
            {
                return;
            }

            if (player && !playerHealer)
            {
                return;
            }

            //heilt
            health += healAmount;
        }

        //sorgt für den Schaden mit Partikel anfrage
        public void TakeDamage(float damageAmount, float damagemin, bool useHitMarker, bool playParticel)
        {
            //sorgt dafür das nicht mininimal schaden mit einbezogen wird (z.B. wenn Steine wackeln)
            if (damageAmount >= damagemin)
            {
                //reguliert werte im Leben und Hitmarker
                health -= damageAmount;
                damage += damageAmount;
                gotHit = useHitMarker;

                //wenn die Partikel spilen soll und er Bluten kann
                if (playParticel && bleed)
                {
                    //regelt das die Particel in gewisser anzahl kommen je nach schaden
                    var em = blood.emission.rateOverTime;
                    em.constant = Mathf.Clamp(damageAmount * 5, 1, 2500);
                    emissionModule = blood.emission;
                    emissionModule.rateOverTime = em.constant;
//spielt den Partikeleffect ab
                    blood.Play();
                }
            }
        }

        //sorgt für den Schaden ohne Partikel abfrage
        public void TakeDamage(float damageAmount, float damagemin)
        {
            //sorgt dafür
            if (damageAmount >= damagemin)
            {
                health -= damageAmount;
                damage += damageAmount;
                gotHit = true;

                //wenn er bluten kann
                if (bleed)
                {
                    //regelt das die Particel in gewisser anzahl kommen je nach schaden
                    var em = blood.emission.rateOverTime;
                    em.constant = Mathf.Clamp(damageAmount * 5, 1, 2500);
                    emissionModule = blood.emission;
                    emissionModule.rateOverTime = em.constant;
//spielt den Partikeleffect ab
                    blood.Play();
                }
            }
        }

        public void StartBurning()
        {
            isBurning = true;
            flame.Play();
        }

        public void StopBurning()
        {
            if (isBurning)
            {
                isBurning = false;
                flame.Stop();
            }
        }

//sorgt für den Verlauf des Hitmarkers
        private void Hitmarker()
        {
            //beschriftet ihn
            hitmarker.GetComponent<TextMeshPro>().text = "-" + damage;
            //verändert seine position für den Verlauf
            Vector3 localPosition = hitmarker.localPosition;
            localPosition = new Vector3(localPosition.x, localPosition.y + 0.02f,
                localPosition.z);
            hitmarker.localPosition = localPosition;

            showHitmarker = true;
            //setzt ihn ab einer Gewissen höhe zurück
            if (hitmarker.localPosition.y >= startpos.y + 4f)
            {
                hitmarker.GetComponent<TextMeshPro>().text = null;
                damage = 0;
                gotHit = false;
                showHitmarker = false;
                hitmarker.localPosition = startpos;
            }
        }

        private void UI()
        {
            // nimmt den Text raus
            Text text = ui.GetComponent<Text>();
            //schreibt die Leben rein
            text.text = "Health:" + Mathf.Round(health) + "/" + maxhealth;
            //sorgt für die Farbe je nach schaden
            if (health > maxhealth * 50 / 100)
            {
                text.color = Color.green;
            }
            else if (health <= maxhealth * 50 / 100 && health > maxhealth * 25 / 100)
            {
                text.color = Color.yellow;
            }
            else if (health <= maxhealth * 25 / 100)
            {
                text.color = Color.yellow;
            }
        }
    }
}