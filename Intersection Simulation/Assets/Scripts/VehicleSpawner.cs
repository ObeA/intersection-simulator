using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathCreation;
using UnityEngine;
using PathFollower = Intersection.PathFollower;
using Random = UnityEngine.Random;

public class VehicleSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public SpawnZone spawnZone;
    public SpawnerSettings[] spawners;

    private List<SpawnerSettings> _spawners;
    private float _lastSpawn;
    private PathCreator _path => GetComponent<PathCreator>() ?? throw new Exception("Should have a path");

    private void Awake()
    {
        _spawners = spawners.OrderBy(s => s.chance).ToList();
    }

    void Start()
    {
        if (GetComponentInParent<SpawnZone>() == null)
        {
            StartCoroutine(SpawnAtRate());
        }
    }

    private IEnumerator SpawnAtRate()
    {
        Spawn();
        yield return new WaitForSeconds(timeBetweenSpawns);
    }

    public bool Spawn()
    {
        var totalRandom = _spawners.Sum(s => s.chance);
        var rand = Random.Range(0, totalRandom);

        SpawnerSettings setting = null;
        for (var i = 0; i < _spawners.Count; i++)
        {
            if ((i == 0 || rand >= _spawners.Take(i - 1).Sum(c => c.chance)) 
                && rand < _spawners.Take(i).Sum(c => c.chance) + _spawners[i].chance)
            {
                setting = spawners[i];
                break;
            }
        }

        if (setting == null)
        {
            Debug.Log($"This should never happen {rand}");
            return false;
        }

        if (setting.vehicle == null)
        {
            return true;
        }

        if ((spawnZone != null && !spawnZone.IsClear) || HasCloseSibling(setting))
        {
            Debug.Log($"[{name}] Aborted spawning. There is currently a vehicle waiting in the spawn zone");
            return false;
        }
        
        var vehicle = Instantiate(setting.vehicle, transform);
        vehicle.SetActive(false);
        var follower = vehicle.AddComponent<PathFollower>();
        follower.pathCreator = _path;
        follower.topSpeed = Random.Range(setting.minSpeed, setting.maxSpeed);
        vehicle.SetActive(true);

        return true;
    }

    private bool HasCloseSibling(SpawnerSettings setting)
    {
        var bounds = setting.vehicle.GetComponent<Collider>().bounds;
        var extents = bounds.extents;
        var collides = Physics.CheckBox(_path.bezierPath[0] + Vector3.up, new Vector3(0.05f, 1, 0.25f),
            _path.path.GetRotationAtDistance(0, EndOfPathInstruction.Stop));

        return collides || GetComponentsInChildren<PathFollower>()
                   .Any(p => p.DistanceTravelled < extents.magnitude * 2);
    }

    [Serializable]
    public class SpawnerSettings
    {
        public GameObject vehicle;
        public float minSpeed;
        public float maxSpeed;
        public float chance;
    }
}
