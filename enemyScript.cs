using UnityEditor.UI;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    public Rigidbody body;
    public MeshRenderer enemyRenderer;
    public Material hitMaterial;
    public Material normalMaterial;
	public int health = 100;
    public float hitCoolDown = 0.1f;
    public float upwardKnockbackBonus = 2f;

    private float timer = 0;
    private bool isHit = false;

    void Start()
    {
        
    }

    void Update()
    {
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

    public void attack(int damage, Vector3 forward, float knockback)
    {
        if (isHit) { return; }
        isHit = true;
        timer = 0;

        health -= damage;
        body.linearVelocity = forward * knockback + Vector3.up * knockback * upwardKnockbackBonus;
        enemyRenderer.material = hitMaterial;

        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
