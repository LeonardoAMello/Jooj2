using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public EnemyController enemy;
    public PlayerController player;
    public GameObject bossPrefab;
    public Animator musicTransition;
    public AudioClip bossMusic;

    public float spawnInterval = .2f;
    private float lastSpawn;

    public static int enemyCount = 0;

    public bool activeSpawn = true;

    private void Update()
    {
        if (activeSpawn)
        {
            EnemyController obj;
            if (lastSpawn <= Time.time - spawnInterval && enemyCount <= 50)
            {
                obj = Instantiate(enemy, new Vector3(Random.Range(-50, 50), 1.5f, Random.Range(-50, 50)), transform.rotation);

                obj.target = player;
                obj.GetComponent<NavMeshAgent>().speed = Random.Range(2.2f, 3.5f);

                enemyCount++;

                if (enemyCount > 25)
                {
                    float step1 = enemyCount - 25;
                    float step2 = step1 / 25;
                    float step3 = step2 * 4;
                    spawnInterval = .1f + step3;
                }
                else
                    spawnInterval = .1f;

                lastSpawn = Time.time;
            }
        }
    }

    public void SpawnBoss()
    {
        GameObject boss = Instantiate(bossPrefab);
        boss.GetComponent<Boss>().target = player;
        boss.GetComponent<Boss>().spawner = this;
        if (musicTransition)
        {
            musicTransition.SetTrigger("MakeTransition");
            Invoke("SwitchMusic", 1f);
        }
    }

    private void SwitchMusic()
    {
        AudioSource source = musicTransition.GetComponent<AudioSource>();

        if (source)
        {
            source.clip = bossMusic;
            source.Play();
        }
    }
}
