using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    
    
    public GameObject Enemy;
    private GameObject _enemy;
    public int xPos;
    public int yPos;
    public int zPos;
    //public int enemyCount;



    void Start()
    {
        
    }

    void Update()
    {
        /*if (enemyCount < 1)
        {
            xPos = Random.Range(20, 65);
            yPos = Random.Range(2, 36);
            zPos = Random.Range(-15, -60);
            Instantiate(Enemy, new Vector3(xPos, yPos, zPos), Quaternion.identity);
            enemyCount += 1;
        }*/

        if (_enemy == null)
        {
            _enemy = Instantiate(Enemy) as GameObject;
            _enemy.transform.position = new Vector3(0, 1, 0);
            Destroy(_enemy);
            xPos = Random.Range(20, 65);
            yPos = Random.Range(2, 36);
            zPos = Random.Range(-15, -60);
            _enemy = Instantiate(Enemy, new Vector3(xPos, yPos, zPos), Quaternion.identity);
            
            
            
        }


    }

}
