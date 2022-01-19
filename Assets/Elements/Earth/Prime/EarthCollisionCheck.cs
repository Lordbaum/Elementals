using Generall;
using UnityEngine;

namespace Elements.Earth.Prime
{
    public class EarthCollisionCheck : MonoBehaviour
    {
        public LayerMask layer;

        public bool earthCheck;

        private void FixedUpdate()
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.blue);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit hit, 4)
                && hit.transform.TryGetComponent(out Objectdata data) && data.HasElement("Earth"))
            {
                earthCheck = true;
            }
            else earthCheck = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Objectdata data) && data.HasElement("Earth"))
                earthCheck = true;
            else earthCheck = false;
        }

        private void OnTriggerExit(Collider other)
        {
            earthCheck = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Objectdata data) && data.HasElement("Earth"))
            {
                earthCheck = true;
            }
            else earthCheck = false;
        }
    }
}