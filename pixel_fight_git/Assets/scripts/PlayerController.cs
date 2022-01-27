using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public float moveSpeed = 4f, runSpeed = 8f;
    private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;

    private Camera cam;

    public float jumpForce = 10f, gravityMod = 2f;

    public GameObject bulletImpact;
    public float timeBetweenShots = .1f;
    private float shotCounter;

    public float maxHeat = 12f, heatPerShot = 1f, coolRate = 2f, overHeatCoolrate = 4f;
    private float heatCounter;
    private bool overHeated;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        overheated.instance.TempSlider.maxValue = maxHeat;
    }


    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -45f, 45f);

        viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }

        float yVel = movement.y;

        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;

        movement.y = yVel;

        if (charCon.isGrounded)
        {
            movement.y = 0f;
        }

        if (Input.GetButtonDown("Jump") && charCon.isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

        charCon.Move(movement * Time.deltaTime);

        if (!overHeated) { 

            if (Input.GetMouseButtonDown(0))
            {
            Shoot();
             }

              if (Input.GetMouseButton(0))
             {
                  shotCounter -= Time.deltaTime;

                  if(shotCounter <= 0)
                 {
                Shoot();
                  }
             }
              if(heatCounter > 0) { 
            heatCounter -= coolRate * Time.deltaTime;
            }
        } else
        {
            heatCounter -= overHeatCoolrate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                overHeated = false;

                overheated.instance.overheatedMessage.gameObject.SetActive(false);
            }
        }

        overheated.instance.TempSlider.value = heatCounter;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("We hit " + hit.collider.gameObject.name);

            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(bulletImpactObject, 10f);
        }

        shotCounter = timeBetweenShots;

        heatCounter += heatPerShot;
        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;

            overHeated = true;

            overheated.instance.overheatedMessage.gameObject.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }
}

