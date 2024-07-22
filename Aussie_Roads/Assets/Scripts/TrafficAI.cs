using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class TrafficAI : MonoBehaviour
{
    [Header("Variables")]
    public float speed = 15;
    public float additionalRange;

    [Header("Ignore")]
    public TrafficRoad road;
    public Transform frontObject;
    Rigidbody rigi;
    public Transform p;
    public Transform r;
    public AudioSource source;
    public AudioClip idleEngine;
    public AudioClip movingEngine;
    public float playDistance = 200;
    public Vector3 prevRot;
    float lastDrivingTime;
    float lastPosition;
    float lastSwitchT;
    float lastTurnT;
    float lastTick;
    float randomTick;
    float spawnT;

    // Start is called before the first frame update
    void Start()
    {
        // Sets up everything
        rigi = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        lastDrivingTime = -10;
        spawnT = Time.timeSinceLevelLoad;
        // Spawns in objects which help with navigation and rotation of the vehicle
        p = new GameObject().transform;
        r = new GameObject().transform;
        p.name = "Pos";
        r.name = "Rot";
        p.SetParent(transform.parent);
        r.SetParent(transform.parent);
        randomTick = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        // Only ticks every certain amount of time for performance
        if (Time.unscaledTime >= lastTick + randomTick)
        {
            lastTick = Time.unscaledTime;
            randomTick = UnityEngine.Random.Range(0.01f, 0.03f);
            // Controls the engine sound
            source.enabled = Vector3.Distance(transform.position, Camera.main.transform.position) < source.maxDistance;

            // Checks if main camera is close enough to the vehicle for performance
            if (Vector3.Distance(transform.position, Camera.main.transform.position) < playDistance + GameManager.me.additionTrafficRange || Time.time < lastSwitchT + 0.3f)
            {
                bool driving = Time.time > lastDrivingTime + 5 & Time.timeSinceLevelLoad >= spawnT + 0.5f;

                // Controls the engine intensity
                if (rigi.velocity.sqrMagnitude >= 1 & driving & !FrontCheck())
                {
                    source.clip = movingEngine;
                    source.volume = 0.432f;
                }
                else
                {
                    source.clip = idleEngine;
                    source.volume = 0.25f;
                }
                if (!source.isPlaying & source.enabled)
                    source.Play();

                // Drives the vehicle
                if (driving & !FrontCheck())
                {
                    if (road)
                    {
                        float currentPoint = 0;
                        SplineUtility.GetNearestPoint(road.spline.Spline, (float3)transform.localPosition, out float3 currentNearestPoint, out currentPoint);
                      // Checks if the vehicle has reached the end of the road/spline
                        if (currentPoint >= 0.999f)
                        {
                            bool clear = true;
                            road.spline.Spline.Evaluate(0, out var pos, out var dir, out var up);
                            Transform t = new GameObject().transform;
                            t.SetParent(transform.parent);
                            t.localPosition = pos;
                            t.SetParent(null);
                            
                            // Checks that there is nothing in the way at the start of the road 
                            foreach (var coll in Physics.OverlapSphere(t.localPosition, 3))
                                if (coll)
                                    if (coll.gameObject != gameObject)
                                    {
                                        if (coll.transform.GetComponentInParent<TrafficAI>())
                                            clear = false;
                                        if (coll.transform.GetComponentInParent<VehicleControl>())
                                            clear = false;
                                    }
                            Destroy(t.gameObject);
                            if (clear)
                            {
                                // Teleports the vehicle to the start of the road
                                transform.SetLocalPositionAndRotation(pos, Quaternion.LookRotation(dir));
                                lastSwitchT = Time.time;
                                rigi.velocity = new Vector3();
                            }
                        }
                        else
                        {
                            // Moves the vehicles rigibody along the closest point along road/spline
                            float point = 0;
                            SplineUtility.GetNearestPoint(road.spline.Spline, (float3)transform.localPosition, out float3 nearestPoint, out point);
                            road.spline.Spline.Evaluate(point, out var pos, out var dir, out var up);
                            p.localPosition = pos;
                            p.localRotation = Quaternion.LookRotation(dir);
                            p.transform.localPosition += p.transform.forward * 25;

                            // Controls the rotation of the vehicle
                            r.localPosition = transform.localPosition;
                            r.transform.LookAt(p.position);
                            transform.localRotation = Quaternion.Lerp(transform.localRotation, r.localRotation, 0.5f);

                            // Moves the vehicle forward
                            if (rigi.velocity.sqrMagnitude < 200)
                                rigi.AddForce(transform.forward * 10000 * speed);
                        }
                    }
                }
            }

            // Destroys vehicle if player is too far from it
            float despawnRange = 200;
            if (road)
                despawnRange = road.despawnRange;
            if (Vector3.Distance(transform.position, Camera.main.transform.position) > despawnRange + GameManager.me.additionTrafficRange)
                Destroy(gameObject);
        }

        // Slows the vehicle down if there is something in the way
        if (FrontCheck())
            rigi.velocity /= 2;
    }

    // Checks if there are any objects in the vehicles way
    public bool FrontCheck()
    {
        frontObject = null;
        Vector3 normalPos = transform.position + (transform.up * 2f);
        foreach (var coll in Physics.OverlapCapsule(normalPos, normalPos + (transform.forward * (5 + additionalRange)), 1.5f))
            if (coll)
                if (coll.gameObject != gameObject)
                {
                    if (coll.transform.GetComponentInParent<TrafficAI>() || coll.transform.GetComponentInParent<VehicleControl>())
                    {
                        // Checks what distance to use for the different types of objects
                        if (coll.transform.GetComponentInParent<TrafficAI>())
                        {
                            if (Vector3.Distance(coll.transform.position, transform.position) <= (10 + additionalRange))
                                frontObject = coll.transform;
                        }
                        if (coll.transform.GetComponentInParent<VehicleControl>())
                        {
                            if (Vector3.Distance(coll.transform.position, transform.position) <= (14 + additionalRange))
                                frontObject = coll.transform;
                        }
                    }
                    else
                        frontObject = coll.transform;
                }
        return frontObject;
    }

    // Stops the car for a duration when hit hard enough by something
    public void OnCollisionEnter(Collision collision)
    {
        if (Time.time < lastSwitchT + 1 || Time.timeSinceLevelLoad < spawnT + 1)
            return;
        if (collision.transform.tag == "Street")
            return;

        // Determines the severity of the force of the hit
        if (collision.relativeVelocity.sqrMagnitude > 200)
            lastDrivingTime = Time.time;
        if (collision.relativeVelocity.sqrMagnitude > 300)
        {
            lastDrivingTime = Mathf.Infinity;
            Destroy(gameObject, 15);
        }
    }

    // Removes all the associated objects upon being deleted
    public void OnDestroy()
    {
        if (p)
            Destroy(p.gameObject);
        if (r)
            Destroy(r.gameObject);
        if (road)
            if (road.spawnTraffic.Contains(this))
                road.spawnTraffic.Remove(this);
    }
}
