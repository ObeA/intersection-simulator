using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnZone : MonoBehaviour
{
    public float spawnRate;
    public uint Count { get; private set; }
    public bool IsClear => Count == 0;

    private List<VehicleSpawner> _children;

    private void Awake()
    {
        _children = GetComponentsInChildren<VehicleSpawner>().ToList();
    }

    private void Start()
    {
        StartCoroutine(SpawnAtRate());
    }

    private IEnumerator SpawnAtRate()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void Spawn()
    {
        if (!IsClear)
        {
            return;
        }

        var tries = 10;
        while (tries-- >= 0 && !_children[Random.Range(0, _children.Count)].Spawn())
            ;
    }

    private void OnTriggerEnter(Collider other)
    {
        Count++;
        Debug.Log($"[Zone {name}] [ENTER] {Count}");
    }

    private void OnTriggerExit(Collider other)
    {
        Count--;
        Debug.Log($"[Zone {name}] [EXIT] {Count}");
    }
}
