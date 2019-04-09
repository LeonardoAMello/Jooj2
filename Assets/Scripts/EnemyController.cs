using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    static float lastCry;
    static int cryCount;
    public PlayerController target;
    public float attackInterval = 2f, damage = 10f;

    private NavMeshAgent agent;
    private AudioSource cry;

    private Animator anim;

    private float nextCry;
    private float nextAttack = 0f;

    public float bossTolerance = 25f;

    private float health = 100f;

    private bool dead = false;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health > 100)
                health = 100;
            if (health <= 0)
                Death();
        }
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        cry = GetComponent<AudioSource>();
        nextCry = Time.time + Random.Range(0f, 30);
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (target != null)
        {
            if (agent != null && !target.died)
                agent.SetDestination(target.transform.position);
            else if (target.died)
                agent.isStopped = true;

            if (Time.time > nextCry && Time.time > lastCry + 1 && Vector3.Distance(transform.position, target.transform.position) > 15 && cryCount <= 5)
            {
                cryCount++;
                Invoke("finishCry", 1.5f);
                cry.clip = Resources.Load("grito" + Random.Range(1, 7)) as AudioClip;
                cry.Play();
                nextCry = Time.time + Random.Range(5f, 30);
                lastCry = Time.time;
            }

            if (agent.remainingDistance > agent.stoppingDistance)
                anim.SetFloat("speedv", 1f);
            else if (agent.remainingDistance != 0 && !target.died)
                attack();
        }
        else if (Time.time > nextCry && Time.time > lastCry + 1 && cryCount <= 5)
        {
            cryCount++;
            Invoke("finishCry", 1.5f);
            cry.clip = Resources.Load("grito" + Random.Range(1, 7)) as AudioClip;
            cry.Play();
            nextCry = Time.time + Random.Range(5f, 30);
            lastCry = Time.time;
        }
    }

    private void attack()
    {
        if (Time.time > nextAttack)
        {
            anim.SetTrigger("Attack1h1");
            Invoke("damageTarget", 1.35f);
            nextAttack = Time.time + attackInterval;
        }
    }

    private void damageTarget()
    {
        if (target != null)
            if (Vector3.Distance(target.transform.position, transform.position) < 3f)
                target.Health -= damage;
    }

    private void finishCry()
    {
        cryCount--;
    }

    public void TakeDamage(float damage)
    {
        anim.SetTrigger("Hit1");
        Health -= damage;
    }

    public void Death()
    {
        if (!dead)
        {
            dead = true;
            target.DecreaseBossTolerance(800 / bossTolerance);

            Spawner.enemyCount--;

            anim.SetTrigger("Fall1");
            Destroy(gameObject, 1.5f);
        }
    }
}
