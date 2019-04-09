using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    private float health = 100f;

    public Camera cam;
    private NavMeshAgent agent;
    private ThirdPersonCharacter character;
    public ParticleSystem flamethrowerPrefab;
    public Vector3 flamesOffset;
    public float flamesCost = 5f;
    public AudioSource flamethrower;

    private Renderer render;

    public bool manualSanity;

    public static bool hasActiveFlamethrower = false;

    [Range(0, 0.8f)]
    public float sanity;

    public bool directionByCamera;

    public float startHealth = 100f;

    public RectTransform bossTolerance;
    public PauseControl pc;
    public bool died = false;
    private bool startDissolve = false;
    float dissolveProgress;

    public bool bossKilled = false;

    public float Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health > 100)
                health = 100;
            if (health <= 20 && !died)
            {
                died = true;
                GetComponent<Animator>().SetTrigger("DeathTrigger");
                Invoke("Death", 3f);
            }
        }
    }

    private void Start()
    {
        Health = startHealth;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        render = GetComponentInChildren<Renderer>();
        character = GetComponent<ThirdPersonCharacter>();
        character.M_GroundCheckDistance = 100;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Update()
    {
        if(bossKilled)
        {
            GetComponent<Animator>().SetTrigger("Win");

            if (agent != null)
                agent.isStopped = true;

            Invoke("ReturnToMenu", 4f);
        }

        if (!died && !bossKilled)
        {
            Health -= Time.deltaTime;

            flamethrowerPrefab.transform.position = transform.position + flamesOffset;

            if (agent != null) // Movement
            {
                Vector2 movement = new Vector2();

                if (Input.GetKey("w"))
                    movement.x += 10;
                if (Input.GetKey("s"))
                    movement.x -= 10;
                if (Input.GetKey("d"))
                    movement.y -= 10;
                if (Input.GetKey("a"))
                    movement.y += 10;

                agent.SetDestination(transform.position + cam.transform.forward * movement.x - cam.transform.right * movement.y);


                if (agent.remainingDistance > agent.stoppingDistance)
                    character.Move(agent.desiredVelocity, false, false);
                else
                    character.Move(Vector3.zero, false, false);
            }

            if (hasActiveFlamethrower && !flamethrower.isPlaying)
                flamethrower.Play();

            if (!hasActiveFlamethrower && flamethrower.isPlaying)
                flamethrower.Stop();

            //Attack
            if (Input.GetMouseButtonDown(0) && health < 100 && !hasActiveFlamethrower)
            {
                hasActiveFlamethrower = true;


                RaycastHit hit;
                if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
                    Attack(hit.point);
                else
                    Attack(transform.position + transform.forward * 5);
            }

            if (Input.GetMouseButtonUp(0))
            {
                hasActiveFlamethrower = false;
            }

            if (hasActiveFlamethrower)
                Health += flamesCost * Time.deltaTime;


            if (render != null)
                if (manualSanity)
                    render.material.SetFloat("Vector1_5E74D552", sanity);
                else
                    render.material.SetFloat("Vector1_5E74D552", CalculateSanity(Health));
        }
        else if(died)
        {
            if (agent != null)
                agent.isStopped = true;

            if (startDissolve)
                render.material.SetFloat("Vector1_9ADE6237", Time.time - dissolveProgress);
        }

    }

    private void Attack(Vector3 targetPosition)
    {
        ParticleSystem obj = Instantiate(flamethrowerPrefab);
        obj.transform.LookAt(targetPosition);
        obj.GetComponent<Flamethrower>().player = this;
    }

    public float CalculateSanity(float _health)
    {
        return (100 - _health) / 100f;
    }

    private void Death()
    {
        dissolveProgress = Time.time;
        startDissolve = true;

        Invoke("ReturnToMenu", 1f);
    }

    private void ReturnToMenu()
    {
        pc.Menu();
    }

    public void DecreaseBossTolerance(float toleranceLost)
    {
        if (bossTolerance != null)
        {
            Vector2 addOffset = bossTolerance.offsetMax;
            addOffset.x += toleranceLost;
            bossTolerance.offsetMax = addOffset;

            Gradient gradient = new Gradient();
            GradientColorKey[] colors = new GradientColorKey[3];

            colors[0].color = Color.green;
            colors[0].time = 0f;
            colors[1].color = Color.yellow;
            colors[1].time = .5f;
            colors[2].color = Color.red;
            colors[2].time = 1f;

            gradient.colorKeys = colors;

            bossTolerance.GetComponent<Image>().color = gradient.Evaluate((800 - Mathf.Abs(bossTolerance.offsetMax.x)) / 800);

            if (bossTolerance.offsetMax.x >= 0)
            {
                Destroy(bossTolerance.GetComponentInParent<InputField>().gameObject);
                FindObjectOfType<Spawner>().SpawnBoss();
            }
        }
    }
}
