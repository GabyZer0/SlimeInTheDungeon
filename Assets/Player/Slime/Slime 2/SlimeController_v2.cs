using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

public class SlimeController_v2 : MonoBehaviour
{
    private FlexSoftActor fsa;
    private FlexContainer fc;

    Transform cam;

    /**
     * Il faut que la vitesse et la vitesse max soient cohérentes. A voir pour trouver une relation mathématiques entre les deux 
     **/
    public int speed;
    public float square_velocity;
    public float max_velocity;


    Vector3 last_pos;

    bool dashing;
    float clock_dash;

    public GameObject projectile;

 

    // Start is called before the first frame update
    void Start()
    {
        fsa = GetComponent<FlexSoftActor>();
        fc = fsa.container;
        cam = /*GetComponent<CameraController>().transform;*/ Camera.main.transform;
        last_pos = transform.position;
        dashing = false;
        clock_dash = 0;
    }

    void Turn(float f)
    {
        Debug.Log("Turning : " + f);
        //fsa.Teleport(transform.position, Quaternion.LookRotation(cam.forward, cam.up));

    }

    // doesnt care about Y magnitude
    float SqrtMagnitudeXZ(Vector3 v)
    {
        return v.x * v.x + v.z * v.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dashing) //et le other et un enemi
        {
            Debug.Log("Alors");
            other.gameObject.SendMessage("ReceiveDamage", 1f);
        }

        Debug.Log("Triggerred by "+other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided by " + collision.transform.name);
    }



    // Update is called once per frame
    void Update()
       {
        bool input = false;
        /**
         * Faudra probablement extraire tout ça
         **/
        float v, h;
        if (square_velocity < max_velocity && (v = Input.GetAxis("Vertical")) != 0)
        {
            //Debug.Log("Vertical : " + v);
            //rbInner.velocity += new Vector3(0, 0, v);

            fsa.ApplyImpulse(cam.forward * v * speed);

            
            input = true;

        }

        if (square_velocity < max_velocity && (h = Input.GetAxis("Horizontal")) != 0)
        {
            //rbInner.velocity += new Vector3(h, 0, 0);
            //fsa.ApplyImpulse(new Vector3(h * speed, 0, 0));
            fsa.ApplyImpulse(cam.right * h * speed);
            input = true;

        }

        if(Input.GetButtonDown("Fire1"))
        {
            Debug.Log("FIre");
            GameObject go = Instantiate(projectile);
            go.transform.position = transform.position;
            go.GetComponent<Rigidbody>().AddForce((cam.forward + cam.up * 0.5f) * 1000); //TODO: Detruite les proj
            Debug.Log("pos : " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        /* DEBUT DASH */
        if(Input.GetKeyDown(KeyCode.RightShift))
        {
            fsa.ApplyImpulse(cam.forward * 25000);
            dashing = true;
            clock_dash = 0;

        }
        if(dashing)
        {
            clock_dash += Time.deltaTime;
            if (clock_dash >= 0.2) //temps dash à mettre en constante
                dashing = false;

        }
        /* FIN DASH */

        if (!input && !dashing &&SqrtMagnitudeXZ(last_pos - transform.position) > 0.001)
        {
            //La téléporrtation supprime le mommentum
            //fsa.Teleport(transform.position, transform.rotation);
        }

        square_velocity = (transform.position - last_pos).sqrMagnitude; 
        last_pos = transform.position;


    }
}
