// camerarotation.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerarotation : MonoBehaviour
{
    public GameObject player;
    private float sensitivity = 3.0f;

    void Update()
    {
        float my = Input.GetAxis("Mouse Y");

        if (Mathf.Abs(my) > 0.001f)
        {
            transform.RotateAround(player.transform.position, player.transform.right, -my * sensitivity);
        }
    }
}