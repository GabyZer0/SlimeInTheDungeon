using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject player;

   
    Vector3 true_size;
    Vector3 map_size;


    // Start is called before the first frame update
    void Start()
    {
        Vector3 res = new Vector3(0, 0, 0);
        int nb = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                if (transform.GetChild(i).GetChild(j).name.Contains("Floor"))
                {
                    res += transform.GetChild(i).GetChild(j).GetComponent<Renderer>().bounds.size;
                    nb++;
                }
            }
        }

        float div = Mathf.Sqrt(nb);

        true_size = res / div;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 tmp = transform.InverseTransformPoint(player.transform.position);

        Vector3 res = new Vector3(tmp.x * (100f / true_size.x), 0, tmp.z*(100f / true_size.z)); // le calcul marche (ici pour une map 100x100) reste plus qu'à la dessiner

        //Debug.Log(res);
    }
}
