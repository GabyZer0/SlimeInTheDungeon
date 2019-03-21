using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /**
     * Pas d'effet d'impression de petite taille => a la troisieme personne, peu d'interet 
     **/

    public GameObject target;
    public Vector3 offset;
    public int speed;


    // Start is called before the first frame update
    void Start()
    {
        offset = target.transform.position - transform.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.transform.position - offset;

        float x, y;
        //un des deux axes doit faire rotationner le personnage
        if ((x = Input.GetAxis("Mouse X")) != 0)
        {
            transform.RotateAround(target.transform.position, Vector3.up, Time.deltaTime * speed * x);
        }
        if ((y = Input.GetAxis("Mouse Y")) != 0)
        {
            transform.RotateAround(target.transform.position, Vector3.right, -Time.deltaTime * speed * y);
        }
        transform.LookAt(target.transform);
        offset = target.transform.position - transform.position;

    }
}
