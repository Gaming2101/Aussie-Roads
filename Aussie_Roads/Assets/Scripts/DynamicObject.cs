using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObject : MonoBehaviour
{
    // The force required to move the object
    public float force = 200;

    // Ignore
    Rigidbody rigi;
    bool done;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up rigidbody
        rigi = GetComponent<Rigidbody>();
        rigi.isKinematic = true;
    }

    // Moves the object once hit hard enough
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude > force)
        {
            done = true;
            rigi.isKinematic = false;
            rigi.AddForce(collision.relativeVelocity * 24000);
        }
    }
}
