using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFlies : MonoBehaviour
{
    [SerializeField] private GameObject flyPrefab;
    [SerializeField] private float spawningTime;
    [SerializeField] private int maxFlies = 25;

    GameObject[] fliesInScene;

    private BoxCollider col;

    private Vector3 spawnPoint;

    private int fliesAmount;

    private float timer;


    private void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //if number of flies is less than the allowed amount, spawn flies under timer
        if (FindFlies())
        {
            timer += Time.deltaTime;
            if(timer >= spawningTime)
            {
                SpawnFly();
                timer = 0f;
            }
        }
    }

    //find random point inside the bounds of the collider trigger
    private Vector3 SpawnLocation(Bounds colBounds)
    {
        spawnPoint = new Vector3(Random.Range(colBounds.min.x, colBounds.max.x), Random.Range(colBounds.min.y, colBounds.max.y), Random.Range(colBounds.min.z, colBounds.max.z));

        return spawnPoint;
    }

    private void SpawnFly()
    {
        //instantiate fly at given location
        Instantiate(flyPrefab, SpawnLocation(col.bounds), Quaternion.identity);
    }

    private bool FindFlies()
    {
        //adds all flies in the scene to an array for optimisation
        fliesInScene = GameObject.FindGameObjectsWithTag("Fly");

        if(fliesInScene.Length > maxFlies)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
}
