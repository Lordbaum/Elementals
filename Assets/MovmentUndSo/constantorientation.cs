using UnityEngine;

public class constantorientation : MonoBehaviour
{
    public bool x, y, z;
    public Vector3 rotation;


    // Update is called once per frame
    void Update()
    {
        if (rotation == Vector3.zero)
        {
            Vector3 euler = transform.parent.rotation.eulerAngles;
            if (x) transform.rotation = Quaternion.Euler(euler.x, 0, 0);
            if (y) transform.rotation = Quaternion.Euler(0, euler.y, 0);
            if (z) transform.rotation = Quaternion.Euler(0, 0, euler.z);
            if (z && x) transform.rotation = Quaternion.Euler(euler.x, 0, euler.z);
            if (z && y) transform.rotation = Quaternion.Euler(0, euler.y, euler.z);
            if (x && y) transform.rotation = Quaternion.Euler(euler.x, euler.y, 0);
            if (!x && !y && !z) transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}