using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    [SerializeField] private GameObject enemyPrefab;
    private GameObject _enemy;

  

    void Start()
    {
        
    }

    void Update()
    {
        if (_enemy == null)
        {
            _enemy = Instantiate(enemyPrefab) as GameObject;
            float position = Random.Range(-40, 40);
            _enemy.transform.position = new Vector3(position, 1, position);
            float angle = Random.Range(0, 360);
            _enemy.transform.Rotate(0, angle, 0);
        }

      

    }
}
