using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
        if (!Camera.main)
            return;

        float target = Camera.main.transform.position.y - transform.position.y;

        transform.LookAt(Camera.main.transform.position - new Vector3(0.0f, target, 0.0f));
        transform.localEulerAngles = new Vector3(0, transform.rotation.eulerAngles.y - 180, 0);
    }
}
