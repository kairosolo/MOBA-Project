using UnityEngine;
using UnityEngine.AI;

public class Enemy : Champion
{
    private Color32 COLOR_RED = new Color32(231, 76, 60, 255);
    private NavMeshAgent agent;
    private MeshRenderer meshRenderer;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private bool stunned = false;

    [Space]
    [Header("Patrolling")]
    [SerializeField] private Vector3 walkPoint;

    [SerializeField] private bool walkPointSet;
    [SerializeField] private float walkPointRange;

    [SerializeField] private float currentStunDuration;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        meshRenderer.material = new Material(meshRenderer.material);
    }

    private void Update()
    {
        if (stunned)
        {
            currentStunDuration -= Time.deltaTime;
            if (currentStunDuration <= 0)
            {
                stunned = false;
                //meshRenderer.material.color = COLOR_RED;
                agent.isStopped = false;
            }
        }
        Patroling();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public void SetStun(bool boolean, float stunDuration)
    {
        currentStunDuration = stunDuration;
        stunned = boolean;
        agent.isStopped = stunned;
    }

    public void SetSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void SetSpeed()
    {
        agent.speed = speed;
    }

    public void SetColor(Color32 newColor)
    {
        meshRenderer.material.color = newColor;
    }
}