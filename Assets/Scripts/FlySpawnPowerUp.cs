using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySpawnPowerUp : MonoBehaviour
{
    [SerializeField] private GameObject flyPrefab;
    [SerializeField] private int spawnRadius;



    private Vector3 offset;
    
    private int spawnAmount;

    private void Start()
    {
        offset = new Vector3(transform.position.x, transform.position.y + spawnRadius, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().StartAudio(AudioManager.Sound.flycage, transform.position, .5f);
            SpawnFlies();

            gameObject.SetActive(false);
            Destroy(gameObject, 0.1f);
        }
    }

    void SpawnFlies()
    {
        spawnAmount = Random.Range(3, 9);
        

        //spawns random num of flies from 3-8
        for(int i = 0; i < spawnAmount; i++)
        {
            //spawns fly in a random pos inside a sphere above the ground
            Vector3 spawnPos = Random.insideUnitSphere * spawnRadius;
            Instantiate(flyPrefab, spawnPos + offset, Quaternion.identity);
        }
    }

}
