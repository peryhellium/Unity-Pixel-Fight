using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform viewPoint;
    public Transform rightArmPoint;
    public Transform hipsPoint;
    public Transform headPoint;

    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private float spine_verticalRotStore;
    private Vector2 mouseInput;
    public float moveSpeed = 4f, runSpeed = 8f;
    private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;
    private Camera cam;
    private Camera camGun;

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

    public float maxHeat = 12f, coolRate = 2f, overHeatCoolrate = 4f;
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
    public float switchDelay = 0.75f;
    private float nextSwitch;

    public float adsSpeed = 5f;

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
        camGun = GameObject.Find("gunCam").GetComponent<Camera>();

        UIcontroller.instance.TempSlider.maxValue = maxHeat;
        photonView.RPC("SetGun", RpcTarget.All, selectedGun);
        currentHealth = maxHealth;
        if (photonView.IsMine)
        {
            UIcontroller.instance.healthNumber.text = currentHealth.ToString();

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
        
        if (photonView.IsMine) 
        {
        if (!UIcontroller.instance.settingsScreen.activeInHierarchy)

            {
                mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
                verticalRotStore += mouseInput.y;
                verticalRotStore = Mathf.Clamp(verticalRotStore, -20f, 40f);
                spine_verticalRotStore = Mathf.Clamp(verticalRotStore, -20f, 40f);
                hipsPoint.rotation = Quaternion.Euler(-spine_verticalRotStore, hipsPoint.rotation.eulerAngles.y, hipsPoint.rotation.eulerAngles.z);

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
                UIcontroller.instance.overheatedMessage.gameObject.SetActive(false);
                UIcontroller.instance.crosshair.gameObject.SetActive(true);
                allGuns[selectedGun].gameObject.SetActive(true);
            }
        }
            UIcontroller.instance.TempSlider.value = heatCounter;
        //switch guns
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;
            if(selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            photonView.RPC("SetGun", RpcTarget.All, selectedGun);
            } else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                selectedGun--;

            if(selectedGun < 0)
            {
                selectedGun = allGuns.Length -1;
            }
                photonView.RPC("SetGun", RpcTarget.All, selectedGun);
            }
        for(int i = 0; i <allGuns.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString())) {
                selectedGun = i;
                photonView.RPC("SetGun", RpcTarget.All, selectedGun);
                }
        }

        }

        if (nicknameLabel != null)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.tag == "Player" && !photonView.IsMine) { 
                crosshair.color = new Color(1, 0, 0, 0.75f);
                UIcontroller.instance.crosshair.color = new Color(1, 0, 0, 0.75f);
                nicknameLabel.text = photonView.Owner.NickName;
                nicknameLabel.color = Color.red;
                nicknameLabel.transform.LookAt(Camera.main.transform.position);
                nicknameLabel.transform.Rotate(0, 180, 0);
                } else
                {
                    nicknameLabel.text = "";
                    crosshair.color = new Color(1, 1, 1, 0.75f);
                    UIcontroller.instance.crosshair.color = new Color(1, 1, 1, 0.75f);
                }
            }
        }

        anim.SetBool("grounded", isGrounded);
        anim.SetFloat("speed", moveDir.magnitude);

        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, allGuns[selectedGun].adsZoom, adsSpeed * Time.deltaTime);
            camGun.fieldOfView = Mathf.Lerp(camGun.fieldOfView, allGuns[selectedGun].adsZoom, adsSpeed * Time.deltaTime);
        } else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 70f, adsSpeed * Time.deltaTime);
            camGun.fieldOfView = Mathf.Lerp(camGun.fieldOfView, 70f, adsSpeed * Time.deltaTime);
        }

    }
    void Shoot()
    {
        
        //check if not in Settings Menu
        if (!UIcontroller.instance.settingsScreen.activeInHierarchy) {
            Vector2 bulletOffset = Random.insideUnitCircle * 20;
            Vector3 randomTarget = new Vector3(Screen.width / 2 + bulletOffset.x, Screen.height / 2 + bulletOffset.y, 0);
            Ray ray = Camera.main.ScreenPointToRay(randomTarget);
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                PhotonNetwork.Instantiate(playerHitImpact.name, hit.point, Quaternion.identity);
                hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, allGuns[selectedGun].shotDamage, PhotonNetwork.LocalPlayer.ActorNumber);
            } else {
                GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * .002f), Quaternion.LookRotation(hit.normal, Vector3.up));
                Destroy(bulletImpactObject, 3f);
            }

        }
        shotCounter = allGuns[selectedGun].timeBetweenShots;
        cooldown = allGuns[selectedGun].timeBetweenShots;
        heatCounter += allGuns[selectedGun].heatPerShot;
        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;
            UIcontroller.instance.overheatedMessage.gameObject.SetActive(true);
            UIcontroller.instance.crosshair.gameObject.SetActive(false);
            allGuns[selectedGun].gameObject.SetActive(false);
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
                UIcontroller.instance.healthNumber.color = Color.white;
                UIcontroller.instance.healthNumber.fontStyle = TMPro.FontStyles.Normal;
            }

            UIcontroller.instance.healthNumber.text = currentHealth.ToString();
            if (currentHealth > 0) { 
            StartCoroutine(BlinkText());
            }
        }
    }
    IEnumerator BlinkText()
    {
        for(int i = 0; i <= blinkTime; i++)
        {
            UIcontroller.instance.healthNumber.color = Color.red;
            UIcontroller.instance.healthNumber.fontStyle = TMPro.FontStyles.Bold;
            yield return new WaitForSeconds(.3f);
            UIcontroller.instance.healthNumber.fontStyle = TMPro.FontStyles.Normal;
            UIcontroller.instance.healthNumber.color = Color.white;
            yield return new WaitForSeconds(.3f);
        }
    }
    private void LateUpdate()
    {
        if (photonView.IsMine) { 
            if(MatchManager.instance.state == MatchManager.GameState.Playing)
            { 
        cam.transform.position = headPoint.position;
        cam.transform.rotation = headPoint.rotation;
            } else
            {
                cam.transform.position = MatchManager.instance.mapCamPoint.position;
                cam.transform.rotation = MatchManager.instance.mapCamPoint.rotation;
            }
        }
    }
    void SwitchGun()
    {
        
        foreach (Guns gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].gameObject.SetActive(true);
        allGuns[selectedGun].muzzleFlash.SetActive(false);

        heatCounter = 0;

    }
    [PunRPC]
    public void SetGun(int gunToSwitchTo)
    {
        if(gunToSwitchTo < allGuns.Length && !overHeated && Time.time > nextSwitch)
        {
            nextSwitch = Time.time + switchDelay;
            selectedGun = gunToSwitchTo;
            SwitchGun();
        }
    } 
}
