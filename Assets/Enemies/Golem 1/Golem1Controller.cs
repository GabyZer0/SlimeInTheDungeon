using UnityEngine;

public class Golem1Controller : EnemyController
{
    Animator animator;
    StateMachine sm;

    Transform[] patrol;
    int nbPatrol;

    public GameObject test_obj_moving;


    float time_iddleaction;

    public GameObject patrouille;

    UnityEngine.AI.NavMeshAgent nvagent;

    GameObject attackTarget;
    float max_following_distance = 150;

    // Start is called before the first frame update
    void Start()
    {
        pv = 5f;
        attack = 1f;
        attack_distance = 15f;
        attack_cooldown = 1.7f;

        time_iddleaction = 30;
        animator = GetComponent<Animator>();
        sm = GetComponent<StateMachine>();

        nvagent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        patrol = new Transform[patrouille.transform.childCount];
        for (int i = 0; i < patrouille.transform.childCount; i++)
            patrol[i] = patrouille.transform.GetChild(i).transform;

        nbPatrol = 0;

        animator.SetBool("Moving", true);
        GoToNextPatrolPoint();
    }

    private void GoToNextPatrolPoint()
    {
        nvagent.destination = patrol[nbPatrol].position;

        nbPatrol = (nbPatrol + 1) % patrol.Length;
    }

    void TestMoving()
    {
        animator.SetBool("Moving", true);
        //transform.LookAt(test_obj_moving.transform);
        //test_is_moving = true;

        nvagent.destination = test_obj_moving.transform.position;
    }

    override protected void ReceiveDamage(float f)
    {
        base.ReceiveDamage(f);
        animator.SetTrigger("Damaged");
    }

    override protected void Death()
    {
        animator.SetTrigger("Dying");
        nvagent.isStopped = true;
    }

    public void PlayerFound(GameObject player)
    {
        attackTarget = player;
    }

    // Update is called once per frame
    void Update()
    {
        if (sm.State == StateMachine.MachineState.NORMAL)
        {
            if (!nvagent.pathPending && nvagent.remainingDistance < 0.5f)
                GoToNextPatrolPoint();

            if (time_iddleaction >= 30 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                Debug.Log("Wesh");
                if (Random.value > 0.9 || Input.GetKeyDown(KeyCode.O))
                {
                    animator.SetTrigger("IddleAction");
                    time_iddleaction = 0;
                }
            }
            else
            {
                time_iddleaction += Time.deltaTime;
            }
        }
        else if(sm.State == StateMachine.MachineState.ATTACK)
        {
            nvagent.destination = attackTarget.transform.position;

            float sDist = (attackTarget.transform.position - transform.position).sqrMagnitude;
            if (attack_clock <=0 &&  sDist <= attack_distance)
            {
                animator.SetTrigger("Attack");
                attackTarget.SendMessage("RecieveDamage", attack);
                attack_clock = attack_cooldown;
            }

            if(sDist >= max_following_distance)
            {
                SendMessage("StopAttack");
            }

            attack_clock -= Time.deltaTime;

        }
    }
}
