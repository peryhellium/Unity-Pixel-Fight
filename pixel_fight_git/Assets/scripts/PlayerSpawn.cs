using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    private GameObject _player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            _player = Instantiate(PlayerPrefab) as GameObject;
            float position = Random.Range(-40, 40);
            _player.transform.position = new Vector3(position, 1, position);
            float angle = Random.Range(0, 360);
            _player.transform.Rotate(0, angle, 0);
        }
    }
}
