using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    public GameObject Enemy;
    public int xPos;
    public int yPos;
    public int zPos;
    public int enemyCount;
    private GameObject _enemy;


    void Start()
    {
        StartCoroutine(EnemySpawn());
    }

    IEnumerator EnemySpawn()
    {
        while (enemyCount < 10)
        {
            xPos = Random.Range(20, 65);
            yPos = Random.Range(2, 36);
            zPos = Random.Range(-15, -60);
            Instantiate(Enemy, new Vector3(xPos, yPos, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1;

        }
    }

    private void Update()
    {
        if (_enemy == null)
        {
            _enemy = Instantiate(Enemy) as GameObject;
        }
    }
}
