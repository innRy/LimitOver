using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("up"))
        {
            transform.position += transform.forward * 0.1f;
        }
        if (Input.GetKey("down"))
        {
            transform.position -= transform.forward * 0.1f;
        }
        if (Input.GetKey("right"))
        {
            transform.Rotate(0f, 3.0f, 0f);
        }
        if (Input.GetKey("left"))
        {
            transform.Rotate(0f, -3.0f, 0f);
        }
    }
}
