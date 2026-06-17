using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float spawnRadius = 1f;

    private int _activeEnemies;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (_activeEnemies < maxEnemies)
                SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null) return;

        Vector2 offset = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0f);
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        _activeEnemies++;
        Health health = enemy.GetComponent<Health>();
        if (health != null)
            health.OnDeath += () => _activeEnemies--;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
