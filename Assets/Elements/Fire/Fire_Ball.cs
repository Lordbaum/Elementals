using Generall;
using UnityEngine;

namespace Elements.Fire
{
    public class Fire_Ball : MonoBehaviour
    {
        public GameObject prefab;
        public int musk;
        private Objectdata data;

        private void OnCollisionEnter(Collision other)
        {
            //wenn data vorhanden ist
            if (other.gameObject.TryGetComponent(out data))
            {
                //wenn das Object aus Pflanzenbesteht aber nicht brennbar ist oder aus Wasser besteht
//wenn der Boden das Elment Pflanze hat aber nicht das Elment waser und nicht brennbar ist
                if (data.HasElement("Plant") && !data.isFlamable && !data.HasElement("Water") &&
                    other.gameObject.layer == musk)
                {
                    //dann wird ein Object Instanziert und seine Position entsprechend gesetzt
                    GameObject cratedObject = Instantiate(prefab);
                    cratedObject.transform.position = other.contacts[0].point;
                }
            }
        }
    }
}