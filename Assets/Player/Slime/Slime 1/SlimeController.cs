using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

public class SlimeController : MonoBehaviour
{
    private FlexClothActor fca;

    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        fca = GetComponent<FlexClothActor>();
    }

    // Update is called once per frame
    void Update()
    {
        float v, h;

        if ((v = Input.GetAxis("Vertical"))!=0)
        {
            Debug.Log("Vertical : " + v);
            //rbInner.velocity += new Vector3(0, 0, v);
            fca.ApplyImpulse(new Vector3(0, 0, v*speed));
            
        }

        if ((h = Input.GetAxis("Horizontal"))!=0)
        {
            Debug.Log("Horizontal : " + h);
            //rbInner.velocity += new Vector3(h, 0, 0);
            fca.ApplyImpulse(new Vector3(h*speed, 0, 0));
        }
    }
}
