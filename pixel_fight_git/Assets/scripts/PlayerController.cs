using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;
    //public Transform rigthArm;

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
    public Transform modelGunPoint, gunHolder;
    public GameObject playerHitImpact;
    public int maxHealth = 100;
    private int currentHealth;

    private int blinkTime = 3;

    public Material[] allSkins;

    public static PlayerController instance;

    public AudioSource hitted;
    public TMP_Text nicknameLabel;

    public Image crosshair;
    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        crosshair.color = new Color(1, 1, 1, 0.75f);
        hitted.Stop();

        gunHolder.parent = modelGunPoint;
        gunHolder.localPosition = Vector3.zero;
        gunHolder.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;

        overheated.instance.TempSlider.maxValue = maxHeat;
        //SwitchGun();
        photonView.RPC("SetGun", RpcTarget.All, selectedGun);
        //Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newTrans.position;
        //transform.rotation = newTrans.rotation;
        currentHealth = maxHealth;
        if (photonView.IsMine)
        {
            overheated.instance.healthNumber.text = currentHealth.ToString();
        } else
        {
            gunHolder.parent = modelGunPoint;
            gunHolder.localPosition = Vector3.zero;
            gunHolder.localRotation = Quaternion.identity;
        }
        playerModel.GetComponent<Renderer>().material = allSkins[photonView.Owner.ActorNumber % allSkins.Length];
    }
    void Update()
    {
        if (photonView.IsMine/* && !overheated.instance.settingsScreen.activeInHierarchy*/) 
        {
        if (!overheated.instance.settingsScreen.activeInHierarchy)
            {
                mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
                verticalRotStore += mouseInput.y;
                verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);
                viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);

            }
          
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
            //muzzle 
            if (allGuns[selectedGun].muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                if (muzzleCounter <= 0)
                {
                    allGuns[selectedGun].muzzleFlash.SetActive(false);
                }
            }
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
                //SwitchGun(); 
                photonView.RPC("SetGun", RpcTarget.All, selectedGun);
            } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if(selectedGun < 0)
            {
                selectedGun = allGuns.Length -1;
            }
                //SwitchGun();
                photonView.RPC("SetGun", RpcTarget.All, selectedGun);
            }
        for(int i = 0; i <allGuns.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString())) {
                selectedGun = i;
                    //SwitchGun();
                    photonView.RPC("SetGun", RpcTarget.All, selectedGun);
                }
        }
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0) && !overheated.instance.settingsScreen.activeInHierarchy)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }*/



        }

        if (nicknameLabel != null)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.tag == "Player") { 
                crosshair.color = new Color(1, 0, 0, 0.75f);
                overheated.instance.crosshair.color = new Color(1, 0, 0, 0.75f);
                Debug.Log("See: " + hit.collider.gameObject.name);
                nicknameLabel.text = photonView.Owner.NickName;
                nicknameLabel.color = Color.red;
                nicknameLabel.transform.LookAt(Camera.main.transform.position);
                nicknameLabel.transform.Rotate(0, 180, 0);
                } else
                {
                    nicknameLabel.text = "";
                    crosshair.color = new Color(1, 1, 1, 0.75f);
                    overheated.instance.crosshair.color = new Color(1, 1, 1, 0.75f);
                }
            }
        }

        anim.SetBool("grounded", isGrounded);
        anim.SetFloat("speed", moveDir.magnitude);
     
    }
    void Shoot()
    {
        
        //check if not in Settings Menu
        if (!overheated.instance.settingsScreen.activeInHierarchy) { 
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                //Debug.Log("Hit " + hit.collider.gameObject.GetPhotonView().Owner.NickName);
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
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
        photonView.RPC("ShootingSound", RpcTarget.All);
        photonView.RPC("MuzzleFlashing", RpcTarget.All);
        }
    }
    [PunRPC]
    public void ShootingSound()
    {
        allGuns[selectedGun].shotSound.Stop();
        allGuns[selectedGun].shotSound.Play();
    }
    [PunRPC]
    public void MuzzleFlashing()
    {
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        //muzzleCounter = muzzleDisplayTime;
        StartCoroutine(Muzzle());
    }
    private IEnumerator Muzzle()
    {
        yield return new WaitForSeconds(0.5f);
        allGuns[selectedGun].muzzleFlash.SetActive(false);
    }
    [PunRPC]
    public void DealDamage(string damager, int damageAmount, int actor)
    {
        TakeDamage(damager, damageAmount, actor);
    }
    public void TakeDamage(string damager, int damageAmount, int actor)
    {
        if (photonView.IsMine) {

            hitted.Stop();
            hitted.Play();

            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                MultiplayerSpawner.instance.Die(damager);
                MatchManager.instance.UpdateStatsSend(actor, 0, 1);
                overheated.instance.healthNumber.color = Color.white;
                overheated.instance.healthNumber.fontStyle = TMPro.FontStyles.Normal;
            }

            overheated.instance.healthNumber.text = currentHealth.ToString();
            StartCoroutine(BlinkText());
        }
    }
    IEnumerator BlinkText()
    {
        for(int i = 0; i <= blinkTime; i++)
        {
            overheated.instance.healthNumber.color = Color.red;
            overheated.instance.healthNumber.fontStyle = TMPro.FontStyles.Bold;
            yield return new WaitForSeconds(.3f);
            overheated.instance.healthNumber.fontStyle = TMPro.FontStyles.Normal;
            overheated.instance.healthNumber.color = Color.white;
            yield return new WaitForSeconds(.3f);
        }
    }
    private void LateUpdate()
    {
        if (photonView.IsMine) { 
            if(MatchManager.instance.state == MatchManager.GameState.Playing)
            { 
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
            } else
            {
                cam.transform.position = MatchManager.instance.mapCamPoint.position;
                cam.transform.rotation = MatchManager.instance.mapCamPoint.rotation;
            }
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
    [PunRPC]
    public void SetGun(int gunToSwitchTo)
    {
        if(gunToSwitchTo < allGuns.Length)
        {
            selectedGun = gunToSwitchTo;
            SwitchGun();
        }
    } 
}
