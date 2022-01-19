using Generall;
using UnityEngine;

namespace Elements.Fire
{
    public class FireArea : MonoBehaviour
    {
        public float burningTime;
        private float burnCounter;


        // Update is called once per frame
        void Update()
        {
            //sorgt dafür das Das gebiet nach eine gewissen zeit Ausbrennt
            if (burnCounter >= burningTime)
            {
                //Deaktivert den Colider
                GetComponent<Collider>().enabled = false;
                //zerstört es
                Destroy(gameObject, 1);
            }

//erhöht den burnCounter um die Vergangene Zeit
            burnCounter += Time.deltaTime;
        }

        void OnTriggerEnter(Collider collider)
        {
            //Schaut nach dem Health oder dem Objectdata script und setzt es in Brant
            if (collider.TryGetComponent(out Health health))
            {
                health.StartBurning();
                //etwas extra damage
                health.TakeDamage(0.5f * Time.deltaTime, 0, false, false);
            }
            else if (collider.TryGetComponent(out Objectdata data) && data.isFlamable)
            {
                data.StartBurning();
            }
        }
    }
}