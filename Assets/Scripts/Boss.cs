using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public GameObject[] flames;
    private float lastFlame;
    public float flamesFrequency;

    static int cryCount;
    public PlayerController target;
    public float attackInterval = 2f, damage = 40f;

    private NavMeshAgent agent;
    private AudioSource cry;

    private Animator anim;

    private float nextCry;
    private float nextAttack = 0f;

    public float distanceOfAttack = 6f;

    public Spawner spawner;

    public RectTransform lifeIndicator;

    private bool dead;

    private float health = 2000f;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health <= 0)
                Death();
        }
    }

    private void Start()
    {
        CameraController.AdjustParticles();
        agent = GetComponent<NavMeshAgent>();
        cry = GetComponent<AudioSource>();
        nextCry = Time.time + Random.Range(2f, 8f);
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (target != null)
        {
            if (Time.time > lastFlame + flamesFrequency && flames.Length > 0 && QualitySettings.GetQualityLevel() > 0)
            {
                Vector3 flamePosition = transform.position;
                flamePosition.y = 0;

                GameObject newFlame = Instantiate(flames[Random.Range(0, flames.Length)], flamePosition, transform.rotation);
                lastFlame = Time.time;

                if (QualitySettings.GetQualityLevel() == 1)
                    Destroy(newFlame, 5f);
            }

            if (agent != null && !target.died)
                agent.SetDestination(target.transform.position);
            else if (target.died)
                agent.isStopped = true;

            if (agent.remainingDistance > agent.stoppingDistance)
                anim.SetFloat("speedv", 1f);
            else if (agent.remainingDistance != 0 && !target.died)
                attack();
        }

        if (Time.time > nextCry)
        {
            cryCount++;
            Invoke("finishCry", 1.5f);
            cry.pitch = Random.Range(.7f, 2f);
            cry.Play();
            nextCry = Time.time + Random.Range(3f, 8f);
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
            if (Vector3.Distance(target.transform.position, transform.position) < distanceOfAttack)
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

        Vector2 newOffset = lifeIndicator.offsetMax;
        newOffset.x = -(2000 - health) / 2.547f;
        lifeIndicator.offsetMax = newOffset;
    }

    private void Death()
    {
        spawner.activeSpawn = false;

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].Death();
        }

        if (!dead)
        {
            dead = true;
            FindObjectOfType<PlayerController>().bossKilled = true;
            
            anim.SetTrigger("Fall1");
            Destroy(gameObject, 1.5f);
        }
    }
}
