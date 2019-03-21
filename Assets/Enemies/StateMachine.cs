using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    /** actuellemnt on utilise pas DEAD **/
    public enum MachineState { NORMAL, ATTACK, DEAD};

    public MachineState State
    {
         get; private set; 
    }
    // Start is called before the first frame update
    void Start()
    {
        State = MachineState.NORMAL;
    }

    public void PlayerFound(GameObject player)
    {
        State = MachineState.ATTACK;
    }

    public void StopAttack()
    {
        State = MachineState.NORMAL;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
