using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_wall : MonoBehaviour
{
    Transform myTransform;
    Vector3 position_start;
    Vector3 position_now;

    void Start()
    {
        myTransform = this.transform;
        position_start = myTransform.position;
        position_now = position_start;

        StartCoroutine(Corou1());
    }

    IEnumerator Corou1()
    {
        while (true)
        {
            // ëOÇ…êiÇﬁ
            while (position_start.z - position_now.z <= 5)
            {
                position_now.z -= 0.05f;
                myTransform.position = position_now;
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);

            while (position_now.z < position_start.z)
            {
                position_now.z += 0.05f;
                myTransform.position = position_now;
                yield return null;
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}