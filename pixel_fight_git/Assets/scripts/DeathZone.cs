using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class DeathZone : MonoBehaviour
{

    public static DeathZone instance;
    public void Awake()
    {
        instance = this;
    }

    public GameObject playerPrefab;
    private GameObject player;
    public GameObject deathEffect;
    private bool isDead = false;
    public float respawnTime = 2f;
    public float staticRespawnTime = 2f;
    public bool collided = false;

    public IEnumerator OnTriggerEnter(Collider other)
    {
        PhotonView phView = other.gameObject.GetComponent<PhotonView>();

        player = other.gameObject; //other means the object that collides with DeathZone
        if (!phView.IsMine)
        {
            // Not our local player - ignore
            yield break;
        }

        if (phView.IsMine)
        {
            overheated.instance.deathScreen.SetActive(true);
            overheated.instance.deathText.text = "You were killed by yourself";
            isDead = true;
            MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
            PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(player);


            yield return new WaitForSeconds(respawnTime);
            {
                overheated.instance.deathScreen.SetActive(false);
                isDead = false; 
                SpawnPlayer();
                
            }
        }
        
    }
    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        respawnTime = staticRespawnTime;

    }
    void Update()
    {
        if (isDead == true)
        {
            respawnTime -= Time.deltaTime;
            string seconds = Mathf.FloorToInt(respawnTime % 60 + 1).ToString("0");
            overheated.instance.timerText.text = seconds;
        }
    }
}
