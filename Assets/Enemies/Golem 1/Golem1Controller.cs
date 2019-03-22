using UnityEngine;

public class Golem1Controller : EnemyController
{
    Animator animator;
    StateMachine sm;

    Vector3[] patrol;
    int nbPatrol;


    float time_iddleaction;

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

        nbPatrol = 0;

        animator.SetBool("Moving", true);
        //GoToNextPatrolPoint();
    }

    public void StartTheGame()
    {
        this.enabled = true;
    }

    public void SetPatrol(Vector3[] patrol)
    {
        this.patrol = patrol;
    }

    private void GoToNextPatrolPoint()
    {
        nvagent.destination = patrol[nbPatrol];

        nbPatrol = (nbPatrol + 1) % patrol.Length;
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
