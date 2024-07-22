using UnityEngine;
using System.Collections;

public class VehicleDamage : MonoBehaviour
{
    [Header("Variables")]
    public float maxMoveDelta = 1.0f; // maximum distance one vertice moves per explosion (in meters)
    public float maxCollisionStrength = 50.0f;
    public float YforceDamp = 0.1f; // 0.0 - 1.0
    public float demolutionRange = 0.5f;
    public float impactDirManipulator = 0.0f;

    // The UI messages to display
    [Header("UI Messages")]
    public UIMessageScript crashHUD;
    public UIMessageScript crashHardHUD;

    [Header("The crash effects")]
    public AudioSource crashSound;
    public ParticleSystem crashEffect;

    [Header("Ignore")]
    public MeshFilter[] optionalMeshList;
    private MeshFilter[] meshfilters;
    private float sqrDemRange;
    public float spawnT;

    public void Start()
    {
        // Sets up the meshed
        if (optionalMeshList.Length > 0)
            meshfilters = optionalMeshList;
        else
            meshfilters = GetComponentsInChildren<MeshFilter>();
        sqrDemRange = demolutionRange * demolutionRange;
        spawnT = Time.timeSinceLevelLoad;
    }

    private Vector3 colPointToMe;
    private float colStrength;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Street")
            return;
        // Stops the process if the time is not right
        if (Time.timeSinceLevelLoad < spawnT + 0.5f)
            return;
        if (GetComponent<TrafficAI>() & !collision.transform.GetComponentInParent<VehicleControl>())
            return;

        // Determines if the crash is bad enough to inflict damage
        Vector3 colRelVel = collision.relativeVelocity;
        colRelVel.y *= YforceDamp;
        if (collision.contacts.Length > 0)
        {
            // Damages the mesh
            colPointToMe = transform.position - collision.contacts[0].point;
            colStrength = colRelVel.magnitude * Vector3.Dot(collision.contacts[0].normal, colPointToMe.normalized);
            OnMeshForce(collision.contacts[0].point, Mathf.Clamp01(colStrength / maxCollisionStrength));

            // Controls the hit effects and sounds
            if (colPointToMe.magnitude > 1.0f & Time.timeSinceLevelLoad > spawnT + 1.2f)
            {
                int points = 0;
                // Controls the damage penalties for the player
                if (GetComponent<VehicleControl>() & colStrength > 8)
                {
                    if (collision.transform.GetComponentInParent<TrafficAI>())
                    {
                        if (colStrength > 14)
                        {
                            if (colStrength > 17)
                                points = 16;
                            else
                                points = 13;
                        }
                        else
                            points = 5;
                    }
                    else
                    {
                        if (colStrength > 7)
                        {
                            if (colStrength > 12)
                                points = 13;
                            else
                                points = 9;
                        }
                        else
                            points = 3;
                    }
                    GameManager.me.Crash(points);
                    Instantiate(crashHardHUD);
                }

                // Controls the sound
                if (crashSound)
                    if (!crashSound.isPlaying)
                    {
                        crashSound.Play();
                        crashSound.volume = (colStrength / 100) + 0.2f;
                    }

                // Controls the particle effects
                if (crashEffect)
                {
                    ParticleSystem vfx = Instantiate(crashEffect);
                    vfx.transform.position = collision.contacts[0].point;
                    Destroy(vfx.gameObject, 5);
                }

                // Shows the crash HUD
                if (crashHUD & points == 0 & colStrength >= 7)
                    Instantiate(crashHUD);
            }
        }
    }

    // if called by SendMessage(), we only have 1 param
    public void OnMeshForce(Vector4 originPosAndForce)
    {
        OnMeshForce((Vector3)originPosAndForce, originPosAndForce.w);
    }

    public void OnMeshForce(Vector3 originPos, float force)
    {
        // force should be between 0.0 and 1.0
        force = Mathf.Clamp01(force);
        for (int j = 0; j < meshfilters.Length; ++j)
        {
            Vector3[] verts = meshfilters[j].mesh.vertices;

            for (int i = 0; i < verts.Length; ++i)
            {
                Vector3 scaledVert = Vector3.Scale(verts[i], transform.localScale);
                Vector3 vertWorldPos = meshfilters[j].transform.position + (meshfilters[j].transform.rotation * scaledVert);
                Vector3 originToMeDir = vertWorldPos - originPos;
                Vector3 flatVertToCenterDir = transform.position - vertWorldPos;
                flatVertToCenterDir.y = 0.0f;

                // 0.5 - 1 => 45° to 0°  / current vertice is nearer to exploPos than center of bounds
                if (originToMeDir.sqrMagnitude < sqrDemRange) //dot > 0.8f )
                {
                    float dist = Mathf.Clamp01(originToMeDir.sqrMagnitude / sqrDemRange);
                    float moveDelta = force * (1.0f - dist) * maxMoveDelta;

                    Vector3 moveDir = Vector3.Slerp(originToMeDir, flatVertToCenterDir, impactDirManipulator).normalized * moveDelta;
                    verts[i] += Quaternion.Inverse(transform.rotation) * moveDir;
                }
            }
            meshfilters[j].mesh.vertices = verts;
            meshfilters[j].mesh.RecalculateBounds();
        }
    }
}
