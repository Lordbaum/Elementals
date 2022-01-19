using Generall;
using UnityEngine;

namespace Elements.Earth.Prime
{
    public class EarthPrime : MonoBehaviour
    {
        public float level;
        public Vector3 startPos;
        private Health health;
        private string safedName;

        // Start is called before the first frame update
        void Start()
        {
            //gets the start position
            startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            //Das ganze segemnt kontrolliert die bewegung des Angriffes
            // es sinkt ab wenn seine position nicht größer ist als seine Startposition +4
            if (!(transform.position.y > startPos.y + 4))
            {
                transform.Translate(0, -0.2f, 0);
            }
            //wen sie größer ist als startposition - 4.5 wird es zerstört
            else if (transform.position.y > startPos.y - 4.5f)
            {
                Destroy(gameObject, 0.25f);
                //sonst bewegt er sich nach oben
            }
            else transform.Translate(0, 0.1f, 0);
        }

        private void OnCollisionEnter(Collision other)
        {
            //kontrolliert den Schaden den er bei einer Collision verursacht
            if (other.gameObject.TryGetComponent(out health))
            {
                health.TakeDamage(10 * level, 0);
            }

//sorgt für bewegung bei getroffenen Objekten
            if (other.gameObject.TryGetComponent(out Rigidbody rigid))
            {
                rigid.velocity = other.relativeVelocity;
                rigid.constraints = RigidbodyConstraints.None;
                rigid.useGravity = true;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            //nochmal das gleiche nur wenn man schon in der Collision drin steht
            //namens sicherung um mehrfachschaden zu verhindern
            if (other.gameObject.name == safedName) return;
            safedName = other.gameObject.name;
            if (other.gameObject.TryGetComponent(out health))
            {
                health.TakeDamage(10 * level, 0);
            }

            if (other.gameObject.TryGetComponent(out Rigidbody rigid))
            {
                rigid.velocity = other.relativeVelocity;
                rigid.constraints = RigidbodyConstraints.None;
                rigid.useGravity = true;
            }
        }
    }
}