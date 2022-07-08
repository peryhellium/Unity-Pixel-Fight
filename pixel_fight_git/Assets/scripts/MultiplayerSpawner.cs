using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class MultiplayerSpawner : MonoBehaviour
{

    public static MultiplayerSpawner instance;

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

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

        respawnTime = staticRespawnTime;
    }

    public void Die(string damager)
    {

        overheated.instance.deathText.text = "You were killed by " + damager;

        MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);

        isDead = true;

        if (player != null)
        {
            StartCoroutine(DieCo());
        }
    }

    public IEnumerator DieCo()
    {
        PhotonNetwork.Instantiate(deathEffect.name, player.transform.position, Quaternion.identity);

        PhotonNetwork.Destroy(player);

        overheated.instance.deathScreen.SetActive(true);

        yield return new WaitForSeconds(respawnTime);

        overheated.instance.deathScreen.SetActive(false);

        SpawnPlayer();

        isDead = false;

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
