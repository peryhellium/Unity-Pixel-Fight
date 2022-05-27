using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
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
    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;
    //public float timeBetweenShots = .1f;
    private float shotCounter;
    private float cooldown = 0f;
    public float muzzleDisplayTime;
    private float muzzleCounter;

    public float maxHeat = 12f, /*heatPerShot = 1f,*/ coolRate = 2f, overHeatCoolrate = 4f;
    private float heatCounter;
    private bool overHeated;

    public Guns[] allGuns;
    private int selectedGun;

    public Animator anim;
    public GameObject playerModel;

    public GameObject playerHitImpact;

    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        overheated.instance.TempSlider.maxValue = maxHeat;

        SwitchGun();

        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;

        currentHealth = maxHealth;

        overheated.instance.healthSlider.maxValue = maxHealth;
        overheated.instance.healthSlider.value = currentHealth;

    }


    void Update()
    {

        if (photonView.IsMine) { 
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

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

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;

        charCon.Move(movement * Time.deltaTime);

        //overheating

        if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;

            if (muzzleCounter <= 0)
            {
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
        }

        //anim.SetBool("grounded", isGrounded);
        //anim.SetFloat("speed", moveDir.magnitude);


        if (!overHeated) {

            if (Input.GetMouseButtonDown(0) && cooldown <= 0)
            {
                Shoot();
            }

            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }

              if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
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

                overheated.instance.crosshair.gameObject.SetActive(true);
            }
        }


        overheated.instance.TempSlider.value = heatCounter;

        //switch guns
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if(selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            SwitchGun(); 
        } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;

            if(selectedGun < 0)
            {
                selectedGun = allGuns.Length -1;
            }
            SwitchGun();
        }

        for(int i = 0; i <allGuns.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString())) {
                selectedGun = i;
                SwitchGun();
            }
        }


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
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            
           

            if(hit.collider.gameObject.tag == "Player")
            {
                Debug.Log("Hit " + hit.collider.gameObject.GetPhotonView().Owner.NickName);

                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);

                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage);


            } else {

                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));

                Destroy(bulletImpactObject, 10f);
            }

            GameObject hitObject = hit.transform.gameObject;
            AnotherAI target = hitObject.GetComponent<AnotherAI>();
            if (target != null)
            {
                target.ReactToHit();
            }
        }

        shotCounter = allGuns[selectedGun].timeBetweenShots;

        cooldown = allGuns[selectedGun].timeBetweenShots;

        heatCounter += allGuns[selectedGun].heatPerShot;

        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;

            overHeated = true;

            overheated.instance.overheatedMessage.gameObject.SetActive(true);

            //overheated.instance.crosshair.gameObject.SetActive(false);
        }

        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;

        allGuns[selectedGun].shotSound.Stop();
        allGuns[selectedGun].shotSound.Play();
    }

    [PunRPC]
    public void DealDamage(string damager, int damageAmount)
    {
        TakeDamage(damager, damageAmount);
    }

    public void TakeDamage(string damager, int damageAmount)
    {

        if (photonView.IsMine) {
            //Debug.Log(photonView.Owner.NickName + "has been hit by: " + damager);

            currentHealth -= damageAmount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                MultiplayerSpawner.instance.Die(damager);
            }

            overheated.instance.healthSlider.value = currentHealth;


        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine) { 
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
        }
    }

    void SwitchGun()
    {
        foreach(Guns gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);

        allGuns[selectedGun].muzzleFlash.SetActive(false);
    }
}

