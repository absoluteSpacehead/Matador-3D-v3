using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class LookAtCamera : MonoBehaviour
{

    GameObject cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cam.transform.position - transform.position;
        pos.x = 0;
        pos.z = 0;
        transform.LookAt(cam.transform.position - pos);
        transform.localRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 180, 0);
    }
}
