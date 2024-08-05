using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int lives = 3;
    private GameObject turret;
    private GameObject firePoint;
    private GameObject capsule;  // used for skins

    [Header("Missile")]
    public GameObject missile;
    private float lastFireTime;
    [SerializeField] private float cooldown;

    [Header("UI")]
    public Slider slider;
    public float force;
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI coinsText;
    public Image liveImage;
    public Sprite[] liveSprites;
    private string loserName;

    [Header("Player Settings")]
    public bool isPlayerOne;
    public static int coins;

    private bool hasRotatedRight;
    public bool isHoldingFire;

    [Header("Sounds")]
    public AudioSource audioSource1;

    [Header("Skin Settings")]
    public Material[] availableSkins;
    private Renderer capsuleRenderer;

    private void Start()
    {
        // Get player child
        turret = transform.GetChild(0).gameObject;  // get the first child
        firePoint = turret.transform.GetChild(0).gameObject;  // child of 1st child

        // Get 2nd child
        capsule = transform.GetChild(1).gameObject;
        capsuleRenderer = capsule.GetComponent<Renderer>();
        ApplySkinToPlayer();

        hasRotatedRight = false;
        isHoldingFire = false;

        lastFireTime = -cooldown;  // Allow immediate firing at game start

        LoadCoinsData();
    }

    void ApplySkinToPlayer()
    {
        if (isPlayerOne)
        {
            int selectedSkinIndex = PlayerPrefs.GetInt("SelectedSkinIndex", 0);
            string unlockedSkins = PlayerPrefs.GetString("UnlockedSkins", "100");

            if (selectedSkinIndex >= 0 && selectedSkinIndex < availableSkins.Length && unlockedSkins[selectedSkinIndex] == '1')
                capsuleRenderer.material = availableSkins[selectedSkinIndex];

            else
                Debug.Log("Selected skin is locked or invalid");

        }

        else                  // apply skin 0 for player 2
            capsuleRenderer.material = availableSkins[0];
    }

    private void OnDestroy()
    {
        // Save coins data to PlayerPrefs when player is destroyed
        SaveCoinsData();
    }

    private void OnApplicationQuit()
    {
        SaveCoinsData();
    }

    private void Update()
    {
        ShowText();
        LoadLivesImage();
        MobileInput();

        // PC CONTROLS

        if (isPlayerOne && Input.GetKey(KeyCode.Space) && canFire())
        {
            force++;
            ShowGauge();
            SliderMechanic();
        }

        if (isPlayerOne && Input.GetKeyUp(KeyCode.Space) && canFire())
        {
            Fire();
            ResetGauge();
            HideGauge();
        }

        // Player 2

        if (!isPlayerOne && Input.GetKey(KeyCode.L) && canFire())
        {
            force++;
            ShowGauge();
            SliderMechanic();
        }

        if (!isPlayerOne && Input.GetKeyUp(KeyCode.L) && canFire())
        {
            Fire();
            ResetGauge();
            HideGauge();
        }
    }

    private void FixedUpdate()
    {
        RotateTurret();
    }

    bool canFire()
    {
        return Time.time >= lastFireTime + cooldown;
    }

    void Fire()
    {
        GameObject newMissile = Instantiate(missile, firePoint.transform.position, firePoint.transform.rotation);

        audioSource1.Play();

        Rigidbody2D missileRb = newMissile.GetComponent<Rigidbody2D>();
        if (slider.value > 0)
            missileRb.velocity = firePoint.transform.up * slider.value / 2;

        lastFireTime = Time.time;
    }

    void RotateTurret()
    {
        // Get current rotation of turret in degrees
        float zRotation = turret.transform.eulerAngles.z;

        // Normalize angle between range -180 and 180
        if (zRotation > 180)
            zRotation -= 360;

        if (zRotation >= 65)
            hasRotatedRight = true;

        if (zRotation <= -65)
            hasRotatedRight = false;

        if (!hasRotatedRight)  // Rotate right
            turret.transform.Rotate(new Vector3(0, 0, 1), 65 * Time.deltaTime);

        if (hasRotatedRight)
            turret.transform.Rotate(new Vector3(0, 0, 1), -65 * Time.deltaTime);
    }

    // POWER GAUGE UI

    void SliderMechanic()
    {
        slider.value = force;
    }

    void ResetGauge()
    {
        force = 0;
        slider.value = 0;
    }

    void HideGauge()
    {
        slider.gameObject.SetActive(false);
    }

    void ShowGauge()
    {
        slider.gameObject.SetActive(true);
    }

    void LoadLivesImage()
    {
        if(lives == 3)
            liveImage.sprite = liveSprites[3];

        else if (lives == 2)
            liveImage.sprite = liveSprites[2];

        else if(lives == 1)
            liveImage.sprite = liveSprites[1];

        else
            liveImage.sprite = liveSprites[0];
    }

    void ShowText()
    {
        coinsText.text = "Coins: " + coins.ToString();
    }

    void DisplayResult()
    {
        if (loserName == "Player 1")
        {
            winnerText.text = "Player 2 Wins";
        }

        if (loserName == "Player 2")
        {
            winnerText.text = "Player 1 Wins";
        }

        gameOverPanel.SetActive(true);
    }

    void MobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            bool isTouchOnLeftSide = touch.position.x < Screen.width / 2;
            bool isTouchOnRightSide = touch.position.x > Screen.width / 2;

            if (isTouchOnLeftSide && isPlayerOne && canFire())
            {
                // Continuous force increment while touch is held
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (!isHoldingFire)
                    {
                        isHoldingFire = true;
                    }
                    force += Time.deltaTime * 50;  // Increase force gradually over time
                    ShowGauge();
                    SliderMechanic();
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Fire();
                    ResetGauge();
                    HideGauge();
                    isHoldingFire = false;
                }
            }
            else if (isTouchOnRightSide && !isPlayerOne && canFire())
            {
                // Continuous force increment while touch is held
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (!isHoldingFire)
                    {
                        isHoldingFire = true;
                    }
                    force += Time.deltaTime * 50;  // Increase force gradually over time
                    ShowGauge();
                    SliderMechanic();
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    Fire();
                    ResetGauge();
                    HideGauge();
                    isHoldingFire = false;
                }
            }
        }
    }

    public void IncreaseCoins(int amount)
    {
        coins += amount;
        SaveCoinsData();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Missile")
        {
            IncreaseCoins(10);
            lives--;
            Destroy(collision.gameObject);

            if (lives == 0)
            {
                Time.timeScale = 0f;
                loserName = gameObject.name;
                DisplayResult();
                SaveCoinsData();
            }
        }
    }

    void LoadCoinsData()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
    }

    void SaveCoinsData()
    {
        PlayerPrefs.SetInt("Coins", coins);
    }
}
