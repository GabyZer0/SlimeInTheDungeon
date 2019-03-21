using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody rb;
    Transform cam;

    public int speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = /*GetComponent<CameraController>().transform;*/ Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float v, h;

        if ((v = Input.GetAxis("Vertical")) != 0)
        {
            rb.AddForce(cam.forward * v * speed);
        }

        if ((h = Input.GetAxis("Horizontal")) != 0)
        {
            rb.AddForce(cam.right * h * speed);
        }

    }
}