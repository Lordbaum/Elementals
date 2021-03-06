using UnityEngine;
using UnityEngine.AI;

namespace Downloaded_Assets.LowPolyNature.Scripts
{
    public class Sheep : MonoBehaviour
    {
        public GameObject Player;

        public float EnemyDistanceRun = 4.0f;

        public GameObject[] ItemsDeadState = null;
        private NavMeshAgent mAgent;

        private Animator mAnimator;

        private bool mIsDead = false;

        private bool IsNavMeshMoving
        {
            get { return mAgent.velocity.magnitude > 0.1f; }
        }

        // Use this for initialization
        void Start()
        {
            mAgent = GetComponent<NavMeshAgent>();

            mAnimator = GetComponent<Animator>();
        }


        // Update is called once per frame
        void Update()
        {
            if (mIsDead)
                return;

            // Only runaway if player is armed
            bool isPlayerArmed = Player.GetComponent<PlayerController>().IsArmed;

            // Performance optimization: Thx to kyl3r123 :-)
            float squaredDist = (transform.position - Player.transform.position).sqrMagnitude;
            float EnemyDistanceRunSqrt = EnemyDistanceRun * EnemyDistanceRun;

            // Run away from player
            if (squaredDist < EnemyDistanceRunSqrt && isPlayerArmed)
            {
                // Vector player to me
                Vector3 dirToPlayer = transform.position - Player.transform.position;

                Vector3 newPos = transform.position + dirToPlayer;

                mAgent.SetDestination(newPos);
            }

            mAnimator.SetBool("walk", IsNavMeshMoving);
        }

        void OnCollisionEnter(Collision collision)
        {
            InteractableItemBase item = collision.collider.gameObject.GetComponent<InteractableItemBase>();
            if (item != null)
            {
                // Hit by a weapon
                if (item.ItemType == EItemType.Weapon)
                {
                    if (Player.GetComponent<PlayerController>().IsAttacking)
                    {
                        mIsDead = true;
                        mAgent.enabled = false;
                        mAnimator.SetTrigger("die");
                        Destroy(GetComponent<Rigidbody>());

                        Invoke("ShowItemsDeadState", 1.2f);
                    }
                }
            }
        }

        void ShowItemsDeadState()
        {
            // Activate the items
            foreach (var item in ItemsDeadState)
            {
                item.SetActive(true);
            }

            Destroy(GetComponent<CapsuleCollider>());

            // Hide the sheep mesh
            transform.Find("sheep_mesh").GetComponent<SkinnedMeshRenderer>().enabled = false;
        }
    }
}