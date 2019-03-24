using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /**
     * Pas d'effet d'impression de petite taille => a la troisieme personne, peu d'interet 
     **/

    public GameObject o_target;
    public Vector3 offset;
    public int speed;


    // Start is called before the first frame update
    void Start()
    {
        offset = o_target.transform.position - transform.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = o_target.transform.position - offset;


        float x, y;
        //un des deux axes doit faire rotationner le personnage
        if ((x = Input.GetAxis("Mouse X")) != 0)
        {
            transform.RotateAround(o_target.transform.position, Vector3.up, Time.deltaTime * speed * x);
            offset = o_target.transform.position - transform.position;
        }
        if ((y = Input.GetAxis("Mouse Y")) != 0)
        {
            transform.RotateAround(o_target.transform.position, Vector3.right, -Time.deltaTime * speed * y);
            offset = o_target.transform.position - transform.position;
        }


        if (Physics.Linecast(o_target.transform.position, transform.position, out RaycastHit hit) && hit.collider.tag == "Untagged" )
        {
            Vector3 hitPoint = new Vector3(hit.point.x + hit.normal.x * 0.5f, hit.point.y + hit.normal.y * 0.5f, hit.point.z + hit.normal.z * 0.5f);
            transform.position = new Vector3(hitPoint.x, hitPoint.y, hitPoint.z);
            Debug.Log(hit.collider.name);
        }

        transform.LookAt(o_target.transform);

    }
}
