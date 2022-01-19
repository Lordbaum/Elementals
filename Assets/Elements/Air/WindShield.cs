using UnityEngine;

public class WindShield : MonoBehaviour
{
    public float lifetime;
    public float level;

    private float timer;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        // sorgt dafür das es nach abgelaufener Zeit zerstört wird
        if (timer >= lifetime)
        {
            GetComponent<BoxCollider>().enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //bei einer collision wird gecheckt ob das object ein Rigidbody besitzt wenn ja wird seine x und z geschwindigkeit auf 0 gesetzt
        if (other.gameObject.TryGetComponent(out Rigidbody rigid))
        {
            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
        }
    }
}