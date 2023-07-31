using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;

    public Wave[] waves;
    public Enemy enemy;
    public RangedEnemy rangedEnemy;

    LivingEntity playerEntity;
    Transform playerT;

    Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    MapGenerator map;

    bool isDisabled;

    public event System.Action<int> OnNewWave;

    void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        map.GenerateMap();
        NextWave();
    }

    void Update()
    {
        if (!isDisabled)
        {
            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("SpawnEnemy");
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0)
        {
            StartCoroutine(StartNextWaveAfterDelay(4f));
        }
    }

    IEnumerator StartNextWaveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NextWave();
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void NextWave()
    {
        if (currentWaveNumber >= waves.Length)
        {
            Debug.Log("All waves completed!");
            isDisabled = true;
        }
        else
        {
            currentWaveNumber++;
            print("Wave: " + currentWaveNumber);
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount + currentWave.rangedEnemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        Transform spawnTile = map.GetRandomOpenTile();
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(
                initialColor,
                flashColor,
                Mathf.PingPong(spawnTimer * tileFlashSpeed, 1)
            );
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        if (
            currentWave.rangedEnemyCount > 0
            && Random.Range(0, enemiesRemainingToSpawn) < currentWave.rangedEnemyCount
        )
        {
            currentWave.rangedEnemyCount--;
            RangedEnemy spawnedEnemy =
                Instantiate(rangedEnemy, spawnTile.position + Vector3.up, Quaternion.identity)
                as RangedEnemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
            spawnedEnemy.SetCharacteristicsRangedEnemy(
                currentWave.moveSpeed,
                currentWave.hitsToKillPlayer,
                currentWave.rangedEnemyHealth,
                currentWave.projectileSpeed,
                currentWave.attackRange
            );
        }
        else
        {
            currentWave.enemyCount--;
            Enemy spawnedEnemy =
                Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
            spawnedEnemy.SetCharacteristics(
                currentWave.moveSpeed,
                currentWave.hitsToKillPlayer,
                currentWave.enemyHealth
            );
        }
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public int rangedEnemyCount;
        public float timeBetweenSpawns;
        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public float rangedEnemyHealth;
        public float projectileSpeed;
        public float attackRange;
    }
}
