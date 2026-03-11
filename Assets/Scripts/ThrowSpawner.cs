using UnityEngine;

public class ThrowSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;
    public GameObject[] prefabs;
    public GameObject targetPrefab;

    [Header("Spawn")]
    public float spawnRadius = 4.0f;
    public float spawnHeightMin = -0.3f;
    public float spawnHeightMax = 1.2f;

    [Header("Throw")]
    public float throwSpeedMin = 3.0f;
    public float throwSpeedMax = 7.0f;
    public float aimJitterDegrees = 6f;
    public float spinMin = 0f;
    public float spinMax = 10f;

    [Header("Timing")]
    public float spawnInterval = 1.0f;

    [Header("Cleanup")]
    public float ballOutOfRangeDistance = 12f;
    public float catchDistance = 0.6f;
    public float targetSpawnDistance = 5f;
    public float targetHeightOffset = 0.2f;

    float _t;
    GameObject _activeBall;
    GameObject _activeTarget;
    bool _ballCaught;

    void Update()
    {
        if (playerHead == null || prefabs == null || prefabs.Length == 0) return;

        if (_activeBall == null)
        {
            if (_activeTarget != null)
            {
                Destroy(_activeTarget);
                _activeTarget = null;
            }

            _ballCaught = false;

            _t += Time.deltaTime;
            if (_t >= spawnInterval)
            {
                _t = 0f;
                SpawnAndThrow();
            }

            return;
        }

        HandleActiveBall();
    }

    void SpawnAndThrow()
    {
        Vector3 dir = Random.onUnitSphere;
        dir.y = 0f;
        dir = dir.normalized;

        float yOffset = Random.Range(spawnHeightMin, spawnHeightMax);
        Vector3 spawnPos = playerHead.position + dir * spawnRadius;
        spawnPos.y = playerHead.position.y + yOffset;

        if (_activeTarget != null)
        {
            Destroy(_activeTarget);
            _activeTarget = null;
        }

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, spawnPos, Random.rotation);
        _activeBall = obj;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (!rb) rb = obj.AddComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Vector3 toPlayer = (playerHead.position - spawnPos).normalized;
        Quaternion jitter = Quaternion.AngleAxis(Random.Range(-aimJitterDegrees, aimJitterDegrees), Random.onUnitSphere);
        Vector3 aimDir = (jitter * toPlayer).normalized;

        float speed = Random.Range(throwSpeedMin, throwSpeedMax);
        rb.velocity = aimDir * speed;
        rb.angularVelocity = Random.onUnitSphere * Random.Range(spinMin, spinMax);
    }

    void HandleActiveBall()
    {
        if (_activeBall == null) return;

        if (_ballCaught && _activeTarget != null)
        {
            float distanceToTarget = Vector3.Distance(_activeBall.transform.position, _activeTarget.transform.position);
            if (distanceToTarget <= 0.6f)
            {
                HandleBallHitTarget(_activeBall, _activeTarget);
                return;
            }
        }

        float distanceToHead = Vector3.Distance(_activeBall.transform.position, playerHead.position);

        if (!_ballCaught && distanceToHead <= catchDistance)
        {
            _ballCaught = true;
            SpawnTarget();
        }

        float distanceFromPlayer = Vector3.Distance(_activeBall.transform.position, playerHead.position);
        if (distanceFromPlayer > ballOutOfRangeDistance)
        {
            Destroy(_activeBall);
            _activeBall = null;

            if (_activeTarget != null)
            {
                Destroy(_activeTarget);
                _activeTarget = null;
            }
        }
    }

    public void HandleBallHitTarget(GameObject ball, GameObject target)
    {
        if (ball != null) Destroy(ball);
        if (target != null) Destroy(target);

        if (_activeBall == ball) _activeBall = null;
        if (_activeTarget == target) _activeTarget = null;
        _ballCaught = false;
        _t = 0f;
    }

    void SpawnTarget()
    {
        if (targetPrefab == null || _activeTarget != null) return;

        Vector3 forward = playerHead.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f)
        {
            forward = Vector3.forward;
        }
        forward.Normalize();

        Vector3 targetPos = playerHead.position + forward * targetSpawnDistance;
        targetPos.y = playerHead.position.y + targetHeightOffset;

        _activeTarget = Instantiate(targetPrefab, targetPos, Quaternion.identity);
    }
}