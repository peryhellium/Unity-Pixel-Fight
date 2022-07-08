using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{

    public GameObject playerPrefab;
    private GameObject player;
    public GameObject deathEffect;
    private bool isDead = false;
    public float respawnTime = 2f;
    public float staticRespawnTime = 2f;
    public bool collided = false;


    public IEnumerator OnTriggerEnter(Collider collider)
    {
        //collided = true;
        overheated.instance.deathScreen.SetActive(true);
        overheated.instance.deathText.text = "You were killed by yourself";
        isDead = true;
        PhotonNetwork.Destroy(player);

        MatchManager.instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);

        yield return new WaitForSeconds(respawnTime);


            Destroy(collider.gameObject);

            Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();

            player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);

            overheated.instance.deathScreen.SetActive(false);
            isDead = false;

            respawnTime = staticRespawnTime;

        //collided = false;

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
