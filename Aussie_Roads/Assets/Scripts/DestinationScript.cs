using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationScript : MonoBehaviour
{
    // The variables
    public GameObject outroProps;
    public Transform pos;
    public Transform objective;

    void Awake()
    {
        // Hides the outro props at start
        if (outroProps)
            outroProps.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Makes the UI marker always face the main camera
        objective.parent.LookAt(Camera.main.transform.position);
        // Resizes the objective so that the player can see it from any distance
        Vector3 rightScale = new Vector3(1, 1, 1);
        float minSize = 1;
        float maxSize = 15;
        rightScale *= Vector3.Distance(objective.transform.position, Camera.main.transform.position) / 200;
        rightScale = new Vector3(Mathf.Clamp(rightScale.x, minSize, maxSize), Mathf.Clamp(rightScale.y, minSize, maxSize), Mathf.Clamp(rightScale.z, minSize, maxSize));
        objective.parent.localScale = Vector3.Lerp(objective.parent.localScale, rightScale, 0.1f);
    }

    // Trigers the outro if the player is moving slowly enough inside this object
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponentInParent<VehicleControl>() & !GameManager.me.over)
        {
            if(other.gameObject.GetComponentInParent<VehicleControl>().GetComponent<Rigidbody>().velocity.sqrMagnitude < 6)
            {
                // Shows the outro props
                if (outroProps)
                {
                    outroProps.gameObject.SetActive(true);
                    GameManager.me.mainCamera.gameObject.SetActive(false);
                    outroProps.GetComponentInChildren<Camera>().enabled = true;
                    outroProps.GetComponentInChildren<Camera>().GetComponent<Animator>().CrossFadeInFixedTime("Outro", 0.1f);
                    foreach (var coll in Physics.OverlapSphere(pos.position, 8))
                        if (coll)
                            if (coll.transform.GetComponentInParent<VehicleDamage>())
                                if (coll.transform.GetComponentInParent<VehicleDamage>().gameObject != other.gameObject.GetComponentInParent<VehicleDamage>().gameObject)
                                    Destroy(coll.transform.GetComponentInParent<VehicleDamage>().gameObject);
                    Destroy(other.gameObject.GetComponentInParent<VehicleDamage>());
                    other.gameObject.GetComponentInParent<VehicleControl>().transform.position = pos.position;
                    other.gameObject.GetComponentInParent<VehicleControl>().transform.eulerAngles = pos.eulerAngles;
                }

                // Completes the game
                GameManager.me.Succeed();
                Destroy(gameObject);
            }
        }
    }
}
