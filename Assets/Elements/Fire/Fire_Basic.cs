using Generall;
using UnityEngine;

namespace Elements.Fire
{
    public class Fire_Basic : MonoBehaviour
    {
        public Transform cameratrans;
        public float raylength;
        public GameObject firePrefab;
        public GameObject fireArea;
        public int groundLayer = 6;

        public float delayWall = 10f, delayCounterWall = 10f;
        private Elementselect elementselect;
        private Vector3 firePositionL, firePositionR;
        private GameObject fireRayHolderL, fireRayHolderR;
        private GameObject fireWallHolder;

        // Start is called before the first frame update
        void Start()
        {
            elementselect = GetComponent<Elementselect>();
            raylength = 10 * elementselect.fire;
        }

        // Update is called once per frame
        void Update()
        {
            delayCounterWall += Time.deltaTime;
            if (elementselect.IsElementActive("Fire"))
            {
                //wen e gedrücktwird und der delay conter größer als der Delay ist wird die fähigkeit ausgelöst
                if (Input.GetKeyDown(KeyCode.E) && delayCounterWall >= delayWall)
                {
                    FireWall();
                }
            }
        }

        private void FireWall()
        {
            //delay wird zurück gesetzt
            delayCounterWall = 0f;
            //raycast zu schauen wo der spiler hinzielt
            if (Physics.Raycast(cameratrans.position, cameratrans.forward, out RaycastHit hit, raylength))
            {
//erschafft ein holder Object für die Feuer Raycastsobjecte
                fireWallHolder = new GameObject("Firewall Placer");
                //setzt die Position des Holders
                fireWallHolder.transform.position = hit.point;
                fireWallHolder.transform.rotation = cameratrans.parent.rotation;
                //plaziert unter sich ein Feuer
                FireWallRaycast(fireWallHolder);

                //wiederholt den Prozess für jedes Level eine flamme rechts und Links
                for (var i = 0; i <= elementselect.fire; i++)
                {
                    //creiert und plaziert den Linken FeuerRaycast holder
                    fireRayHolderL = new GameObject("fireL");
                    fireRayHolderL.transform.parent = fireWallHolder.transform;
                    fireRayHolderL.transform.localPosition = new Vector3(-i * 3, +5, 0);
                    //plaziert dessen Flamme
                    FireWallRaycast(fireRayHolderL);
                    //creiert und plaziert den rechten FeuerRaycast holder
                    fireRayHolderR = new GameObject("fireR");
                    fireRayHolderR.transform.parent = fireWallHolder.transform;
                    fireRayHolderR.transform.localPosition = new Vector3(i * 3, +5, 0);
                    //plaziert dessen Flamme
                    FireWallRaycast(fireRayHolderR);
                }

//zerstört den holder und setzt alles sicherheithalber auf null
                Destroy(fireWallHolder);
                fireWallHolder = null;
                fireRayHolderL = null;
                fireRayHolderR = null;
            }
        }

        private void FireWallRaycast(GameObject fireRayHolder)
        {
//check was unter den holdern ist in einer diestanz von 7.5
            if (Physics.Raycast(fireRayHolder.transform.position, Vector3.down, out RaycastHit hit, 7.5f))
            {
                //schaut ob es Leben hat und diese entflammen kann wenn ja zündet er es an
                if (hit.transform.TryGetComponent(out Health health) && health.isFlamable)
                {
                    health.StartBurning();
                }
                //schaut ob es ein Objectdata-Script hat und dieses entflammen kann wenn ja zündet er es an
                else if (hit.transform.TryGetComponent(out Objectdata data) && data.isFlamable)
                {
                    data.StartBurning();
                }
//wenn der Boden das Elment Pflanze hat aber nicht das Elment waser und nicht brennbar ist
                else if (data.HasElement("Plant") && !data.isFlamable && !data.HasElement("Water") &&
                         hit.transform.gameObject.layer == groundLayer)
                {
                    //dann wird ein Object Instanziert und seine Position entsprechend gesetzt
                    GameObject cratedObject = Instantiate(fireArea);
                    cratedObject.transform.position = hit.point;
                }
            }
        }
    }
}