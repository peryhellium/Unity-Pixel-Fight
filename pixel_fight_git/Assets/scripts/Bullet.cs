using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bullet;

    void Start()
    {
        bullet.SetActive(false);
    }

    public void OnMouseOver()
    {
        bullet.SetActive(true);
    }

    public void OnMouseExit()
    {
        bullet.SetActive(false);
    }


}