using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WandererAI : MonoBehaviour
{
    public float speed = 3.0f;
    public float obstacleRange = 5.0f;
    private bool _alive;
    public Transform Player;
    int maxDist = 12;
    int stopDist = 6;

    private Vector3 movement;
    
    //public float gravityMod = 4f;


    void Start()
    {
        _alive = true;
    }
    
    void Update()
    {
        movement.y += Physics.gravity.y;
        if (_alive) {

            if (Vector3.Distance(transform.position, Player.position) <= maxDist && (Vector3.Distance(transform.position, Player.position) > stopDist)) { 
                transform.Translate(0, 0, speed * Time.deltaTime);
                transform.LookAt(Player);
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.SphereCast(ray, 0.75f, out hit))
                {

                    if (hit.distance < obstacleRange)
                    {
                        float angle = Random.Range(-110, 110);
                        transform.Rotate(0, angle, 0);
                    }
                }
            }  else if (Vector3.Distance(transform.position, Player.position) <= stopDist)
            {
               
                transform.LookAt(Player);
            } else
            {
                transform.Translate(0, 0, speed * Time.deltaTime);
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.SphereCast(ray, 0.75f, out hit))
                {

                    if (hit.distance < obstacleRange)
                    {
                        float angle = Random.Range(-110, 110);
                        transform.Rotate(0, angle, 0);
                    }
                }

            }

            //float yVel = movement.y;

            //movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

            //movement.y = yVel;


        }
                
    }

    public void SetAlive (bool alive)
    {
        _alive = alive;
    }
}
