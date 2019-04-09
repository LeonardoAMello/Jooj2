using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ParticleSystem))]
public class Flamethrower : MonoBehaviour
{
    public float damage = 1f;

    private ParticleSystem part;

    public PlayerController player;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + player.flamesOffset;

            if (!part.isPlaying)
                Destroy(gameObject);

            if (player.Health >= 100)
            {
                PlayerController.hasActiveFlamethrower = false;
                part.Stop();
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(player.cam.ScreenPointToRay(Input.mousePosition), out hit))
                    transform.LookAt(hit.point);
            }

            if (Input.GetMouseButtonUp(0))
                part.Stop();

            if (part.isStopped)
                Destroy(gameObject);
        }
    }

    void OnParticleCollision(GameObject other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        Boss boss = other.GetComponent<Boss>();

        if (enemy)
            enemy.TakeDamage(damage);
        if (boss)
            boss.TakeDamage(damage);
    }
}