using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float destroyAfter = 5f;

    private void Start()
    {
        Destroy(gameObject, destroyAfter);
    }
}
