using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoSingleton<EnemyManager>
{
    public GameObject[] enemyPrefabs;

    int minEnemyAmount = 10;
    int maxEnemyAmount = 100;

    [SerializeField]
    int waveNumver = 1;
    [SerializeField]
    int currentEnemyAmount;

    private Coroutine AutoGenerateEnemy;
    WaitForSeconds minSpawnInterval;

    private Vector3 playerPos;
    private Vector3 enemyGeneratePos;

    private int totalEnemyDefeat;

    // Pool Enemy
    public ObjectPool<Enemy> enemyPool_1;
    public ObjectPool<Enemy> enemyPool_2;
    public ObjectPool<Enemy> enemyPool_3;
    public ObjectPool<Enemy> enemyPool_4;
    public ObjectPool<Enemy> enemyPool_5;

    protected override void OnStart()
    {
        // Pool
        enemyPool_1 = new ObjectPool<Enemy>(CreateEnemyWeak, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 80, 100);
        enemyPool_2 = new ObjectPool<Enemy>(CreateEnemyNormal, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 80, 100);
        enemyPool_3 = new ObjectPool<Enemy>(CreateEnemyHard, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 80, 100);
        enemyPool_4 = new ObjectPool<Enemy>(CreateEnemyElite, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 80, 100);
        enemyPool_5 = new ObjectPool<Enemy>(CreateEnemyRange, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 80, 100);

        // Becareful, Int/Int usually cause 0 and BOOM!
        minSpawnInterval = new WaitForSeconds(5f / waveNumver);
        AutoGenerateEnemy = StartCoroutine(nameof(RandomSpawnEnemy));
    }

    IEnumerator RandomSpawnEnemy()
    {
        
        while(currentEnemyAmount < maxEnemyAmount)
         {
            playerPos = GameManager.Instance.Player.transform.position;

            int enemyRandom = Random.Range(0, 4);
            switch (enemyRandom){
                case 0:
                    enemyGeneratePos = playerPos + new Vector3(Random.Range(-5,5), Camera.main.ViewportToWorldPoint(new Vector2(0,0)).y - 3, 0);
                    break;
                case 1:
                    enemyGeneratePos = playerPos + new Vector3(Random.Range(-5, 5), Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y + 3, 0);
                    break;
                case 2:
                    enemyGeneratePos = playerPos + new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(0, 0)).x - 3, Random.Range(-5, 5), 0);
                    break;
                case 3:
                    enemyGeneratePos = playerPos + new Vector3(Camera.main.ViewportToWorldPoint(new Vector2(1, 0)).x + 3, Random.Range(-5, 5), 0);
                    break;
            }

            if (waveNumver < 5)
            {
                enemyPool_1.Get();
            }else if(waveNumver < 10)
            {
                switch (Random.Range(0, 2)){
                    case 0: enemyPool_1.Get(); break;
                    case 1: enemyPool_2.Get(); break;
                    default: enemyPool_1.Get(); break;
                }
            }else if (waveNumver < 15)
            {
                switch (Random.Range(0, 3))
                {
                    case 0: enemyPool_1.Get(); break;
                    case 1: enemyPool_2.Get(); break;
                    case 2: enemyPool_3.Get(); break;
                    default: enemyPool_2.Get(); break;
                }
            }else if (waveNumver < 20)
            {
                switch (Random.Range(0, 3))
                {
                    case 0: enemyPool_2.Get(); break;
                    case 1: enemyPool_3.Get(); break;
                    case 2: enemyPool_5.Get(); break;
                    default: enemyPool_3.Get(); break;
                }
            }else if (waveNumver < 25)
            {
                switch (Random.Range(0, 3))
                {
                    case 0: enemyPool_3.Get(); break;
                    case 1: enemyPool_4.Get(); break;
                    case 2: enemyPool_5.Get(); break;
                    default: enemyPool_4.Get(); break;
                }
            }
            else
            {
                this.gameObject.SetActive(false);
                //TODO: Game Win Logic
            }
            yield return minSpawnInterval;
         }
        // Enemy Too much
        // TODO: Fail Logic
    }

    private void DifficultyUp()
    {
        waveNumver++;
        minSpawnInterval = new WaitForSeconds(5f / waveNumver);
    }

    #region Pool
    private Enemy CreateEnemyWeak()
    {
        GameObject go = Instantiate(enemyPrefabs[0], enemyGeneratePos, Quaternion.identity, this.transform);
        Enemy enemy = go.GetComponent<Enemy>();

        enemy.SetPool(enemyPool_1);

        return enemy;
    }

    private Enemy CreateEnemyNormal()
    {
        GameObject go = Instantiate(enemyPrefabs[1], enemyGeneratePos, Quaternion.identity, this.transform);
        Enemy enemy = go.GetComponent<Enemy>();

        enemy.SetPool(enemyPool_2);

        return enemy;
    }

    private Enemy CreateEnemyHard()
    {
        GameObject go = Instantiate(enemyPrefabs[2], enemyGeneratePos, Quaternion.identity, this.transform);
        Enemy enemy = go.GetComponent<Enemy>();

        enemy.SetPool(enemyPool_3);

        return enemy;
    }

    private Enemy CreateEnemyElite()
    {
        GameObject go = Instantiate(enemyPrefabs[3], enemyGeneratePos, Quaternion.identity, this.transform);
        Enemy enemy = go.GetComponent<Enemy>();

        enemy.SetPool(enemyPool_4);

        return enemy;
    }

    private Enemy CreateEnemyRange()
    {
        GameObject go = Instantiate(enemyPrefabs[4], enemyGeneratePos, Quaternion.identity, this.transform);
        Enemy enemy = go.GetComponent<Enemy>();

        enemy.SetPool(enemyPool_5);

        return enemy;
    }

    private void OnTakeEnemyFromPool(Enemy enemy)
    {
        currentEnemyAmount++;

        // set enemy
        enemy.InitEnemy(waveNumver);
        enemy.transform.position = enemyGeneratePos;

        //set active
        enemy.gameObject.SetActive(true);
    }

    private void OnReturnEnemyToPool(Enemy enemy)
    {
        currentEnemyAmount--;
        totalEnemyDefeat++;

        enemy.gameObject.SetActive(false);
        if (totalEnemyDefeat > 15) 
        { 
            DifficultyUp();
            totalEnemyDefeat = 0;
        }
    }

    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    #endregion
}
