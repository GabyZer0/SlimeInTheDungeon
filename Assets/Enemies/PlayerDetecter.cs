using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetecter : MonoBehaviour
{

    public float max_distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray
        {
            direction = transform.forward,
            origin = transform.position + transform.forward * 0.6f + transform.up * 0.1f
        };


        Debug.DrawLine(ray.origin, ray.origin + ray.direction * max_distance, Color.green);



        if (Physics.Raycast(ray, out RaycastHit hit, max_distance, 0, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Sending PlayerFound");
                SendMessage("PlayerFound", hit.collider.gameObject);
            }

            Debug.Log("RayHit: " + hit.collider.gameObject.name);
        }
    }
}
