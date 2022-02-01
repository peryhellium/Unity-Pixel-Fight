using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    public void ReactToHit()
    {
        WandererAI behavior = GetComponent<WandererAI>();
        if (behavior != null) {
            {
                behavior.SetAlive(false);
            } 
        }
        StartCoroutine(Die());

    }

    private IEnumerator Die()
    {
        this.transform.Rotate(-90, 0, 0);

        this.transform.Translate(0, 0, -0.5f);

        yield return new WaitForSeconds(1.5f);

        Destroy(this.gameObject);

        
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
