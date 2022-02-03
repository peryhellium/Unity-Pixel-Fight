using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnotherAI : MonoBehaviour
{

    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public Guns Gun;

    public float muzzleDisplayTime;
    private float muzzleCounter;

    //private float cooldown = 0f;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States 
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private bool _alive;

    public Animator anim;
    private bool isGrounded;
    public LayerMask groundLayers;
    public Transform groundCheckPoint;

    private void Awake()
    {
        
        player = GameObject.Find("Player").transform;
        if (_alive) {

            agent = GetComponent<NavMeshAgent>();
        }
    }


    void Start()
    {
        _alive = true;
    }


    void Update()
    {
        if (_alive)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInAttackRange && !playerInSightRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }

        
    }

    private void Patroling()
    {
        if (_alive) { 
            if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        if (_alive)
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

            anim.SetBool("grounded", isGrounded);
            anim.SetFloat("speed", walkPoint.magnitude);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            {
                walkPointSet = true;
            }
        }
    }

    private void ChasePlayer()
    {
        if (_alive) { 
            agent.SetDestination(player.position);
            transform.LookAt(player);

           

           isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

           anim.SetBool("grounded", isGrounded);
           anim.SetFloat("speed", 1f);
            
            


        }
    }
    private void AttackPlayer()
    {
        if (_alive)
        {
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            anim.SetBool("grounded", isGrounded);
            anim.SetFloat("speed", 0f);


            if (!alreadyAttacked)
            {
                Gun.muzzleFlash.SetActive(true);
                Gun.muzzleFlash.SetActive(false);
                Gun.muzzleFlash.SetActive(true);


                /*muzzleCounter = muzzleDisplayTime;

                if (Gun.muzzleFlash.activeInHierarchy)
                {
                    muzzleCounter -= Time.deltaTime;

                    if (muzzleCounter <= 0)
                    {
                        Gun.muzzleFlash.SetActive(false);
                    }
                }*/
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);

                //Debug.Log("shot!");

            }
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false; 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void ReactToHit()
    {
        _alive = false;
        StartCoroutine(Die());

    }

    private IEnumerator Die()
    {
        /*this.transform.Rotate(-90, 0, 0);

        this.transform.Translate(0, 0, 0);*/

        yield return new WaitForSeconds(0.5f);

        Destroy(this.gameObject);


    }

}
