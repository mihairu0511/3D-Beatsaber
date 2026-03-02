using UnityEngine;

public class ThrowSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform playerHead;
    public GameObject[] prefabs;

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

    float _t;

    void Update()
    {
        if (playerHead == null || prefabs == null || prefabs.Length == 0) return;

        _t += Time.deltaTime;
        if (_t >= spawnInterval)
        {
            _t = 0f;
            SpawnAndThrow();
        }
    }

    void SpawnAndThrow()
    {
        Vector3 dir = Random.onUnitSphere;
        dir.y = 0f;
        dir = dir.normalized;

        float yOffset = Random.Range(spawnHeightMin, spawnHeightMax);
        Vector3 spawnPos = playerHead.position + dir * spawnRadius;
        spawnPos.y = playerHead.position.y + yOffset;

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, spawnPos, Random.rotation);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (!rb) rb = obj.AddComponent<Rigidbody>();

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        Vector3 toPlayer = (playerHead.position - spawnPos).normalized;
        Quaternion jitter = Quaternion.AngleAxis(Random.Range(-aimJitterDegrees, aimJitterDegrees), Random.onUnitSphere);
        Vector3 aimDir = (jitter * toPlayer).normalized;

        float speed = Random.Range(throwSpeedMin, throwSpeedMax);
        rb.linearVelocity = aimDir * speed;

        rb.angularVelocity = Random.onUnitSphere * Random.Range(spinMin, spinMax);
    }
}