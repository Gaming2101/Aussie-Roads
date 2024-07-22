using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraScript : MonoBehaviour
{
    public static IntroCameraScript me;

    // Ignore
    [HideInInspector]
    public Animator ani;

    void Awake()
    {
        // Sets up variables
        me = this;
        GetComponent<Camera>().enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Sets up the ani variable
        ani = GetComponent<Animator>();
    }
}
