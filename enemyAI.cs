using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundMask, playerMask;
    public Vector3 walkPoint;
    public float walkPointRange;
    public float attackDelay;
    public float attackRange, sightRange;

	private bool walkPointSet;
	private bool attacked;
    private bool playerInSight, playerInAttackRange;

	void Start()
	{
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
	}


	void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        if (playerInAttackRange)
        {
            Attack();
        }
        else if (playerInSight)
        {
            Chase();
        }
        else
        {
            Patrol();
        }
	}

	private void Patrol()
    {
        if (!walkPointSet) { SearchWalkPoint(); }

        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
			Vector3 distanceToWalkPoint = transform.position - walkPoint;
			if (distanceToWalkPoint.magnitude < 1)
			{
				walkPointSet = false;
			}
		}
	}

    private void SearchWalkPoint()
    {
        float randZ = Random.Range(walkPointRange, -walkPointRange);
        float randX = Random.Range(walkPointRange, -walkPointRange);

        walkPoint = new Vector3(transform.position.x + randX, transform.position.y, transform.position.z + randZ);

        if (Physics.Raycast(walkPoint, Vector3.down, 2f, groundMask)) {
            walkPointSet = true;
        }
	}

	private void Attack()
    {
        agent.SetDestination(transform.position);
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }
}
