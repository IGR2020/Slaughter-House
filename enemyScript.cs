using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;

public class enemyScript : MonoBehaviour
{
    public MeshRenderer enemyRenderer;
    public NavMeshAgent agent;
    public Material hitMaterial;
    public Material normalMaterial;
	public int health = 100;
    public int damage = 1;
    public float hitCoolDown = 0.1f;
    public float knockbackAmplifier = 0.9f;
    public float speed = 4;
    public float debuffSpeed = 1;

    private Vector3 knockback = Vector3.zero;
    private float timer = 0;
    private bool isHit = false;

    void Start()
    {
        
    }

    void Update()
    {
        knockback *= knockbackAmplifier;
        if (knockback.magnitude < 0.2)
        {
            knockback = Vector3.zero;
            agent.speed = speed;
        }

        transform.position += knockback * Time.deltaTime;

        if (isHit)
        {
            timer += Time.deltaTime;
        }
        else { return; }
        if (timer > hitCoolDown) { 
            isHit = false;
            enemyRenderer.material = normalMaterial;
        }
    }

    public void Attack(int damage, Vector3 forward, float knockbackForce)
    {
        if (isHit) { return; }
        isHit = true;
        timer = 0;

        health -= damage;
        
        knockback = forward * knockbackForce;
        agent.speed = debuffSpeed;

        enemyRenderer.material = hitMaterial;

        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
