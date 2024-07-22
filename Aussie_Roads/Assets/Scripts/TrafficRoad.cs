using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class TrafficRoad : MonoBehaviour
{
    // The data for the road
    public List<TrafficAI> traffic = new List<TrafficAI>();
    public float range = 100;
    public float despawnRange = 300;
    public int maxTraffic = 15;

    // Ignore
    [HideInInspector] public SplineContainer spline;
    [HideInInspector] public List<TrafficAI> spawnTraffic = new List<TrafficAI>();
    [HideInInspector] public bool close;
    [HideInInspector] public Transform pos;

    // Start is called before the first frame update
    void Start()
    {
        // Set up the spline
        spline = GetComponent<SplineContainer>();
        pos = new GameObject().transform;
        pos.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        // Detects if the player is close the road
        pos.transform.localPosition = spline.Spline[0].Position;
        if (Vector3.Distance(pos.transform.position, Camera.main.transform.position) <= range + GameManager.me.additionTrafficRange)
        {
            if (!close)
            {
                // Spawns as much traffic as the Max Traffic variable says
                for (int i = 0; i < maxTraffic; i++)
                    SpawnTraffic();
                close = true;
            }
        }
        else if (close)
            close = false;
    }

    // Spawns the traffic
    public void SpawnTraffic()
    {
        // Picks a random vehicle
        TrafficAI trafficVehicle = Instantiate(traffic[Random.Range(0, traffic.Count - 1)]);
        trafficVehicle.transform.SetParent(transform);
        trafficVehicle.road = this;
        trafficVehicle.name = "Traffic";
        // Picks a random point along the spline to spawn
        float point = Random.Range(0f, 1f);
        spline.Spline.Evaluate(point, out var pos, out var dir, out var up);
        trafficVehicle.transform.SetLocalPositionAndRotation(pos, Quaternion.LookRotation(dir));
        // Detects if there is anything close to the vehicle, and deletes it if so
        if (Vector3.Distance(trafficVehicle.transform.position, GameManager.me.mainCamera.transform.position) <= 60)
            Destroy(trafficVehicle.gameObject);
        if (Vector3.Distance(trafficVehicle.transform.position, GameManager.me.startPos) <= 25)
            Destroy(trafficVehicle.gameObject);
        foreach (TrafficAI ai in FindObjectsOfType<TrafficAI>())
            if (Vector3.Distance(trafficVehicle.transform.position, ai.transform.position) <= 5 & ai != trafficVehicle)
                Destroy(trafficVehicle.gameObject);
        // Adds the vehicle to the list
        if (trafficVehicle.gameObject)
            spawnTraffic.Add(trafficVehicle);
    }
}
