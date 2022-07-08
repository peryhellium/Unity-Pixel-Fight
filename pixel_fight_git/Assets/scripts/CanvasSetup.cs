using UnityEngine;
using UnityEngine.UI;

public class CanvasSetup : MonoBehaviour
{
    public GameObject titleMenu;
    public GameObject mainMenu;
    public GameObject quitMenu;
    public GameObject joinMenu;
    public GameObject createMenu;

    void Start()
    {

        titleMenu.SetActive(true);
        mainMenu.SetActive(false);
        quitMenu.SetActive(false);
        joinMenu.SetActive(false);
        createMenu.SetActive(false);
    }


    void Update()
    {
        if (Input.anyKey)
            mainMenu.SetActive(true);

        if (Input.anyKey)
            titleMenu.SetActive(false);
    }
}
