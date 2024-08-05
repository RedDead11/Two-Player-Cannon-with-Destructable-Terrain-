using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bot : MonoBehaviour
{
    private int lives = 3;
    private GameObject turret;
    private GameObject firePoint;
    private GameObject playerObject;
    public Player playerScript;

    [Header("Missile")]
    public GameObject missile;
    private float lastFireTime;
    [SerializeField] private float cooldown;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    public Image liveImage;
    public Sprite[] liveSprites;
    private string loserName;

    [Header("Player Settings")]
    private bool hasRotatedRight;
    public float aimingTime = 1.5f;  // Time spent aiming before firing
    private float launchForce;

    [Header("Bot Accuracy")]
    public float inaccuracyRange = 10f;  // Degrees of inaccuracy for aiming
    public float minLaunchForce = 10f;
    public float maxLaunchForce = 20f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        turret = transform.GetChild(0).gameObject;  // get the first child
        firePoint = turret.transform.GetChild(0).gameObject;

        hasRotatedRight = false;

        lastFireTime = -cooldown;     // Allow immediate firing at game start

        StartCoroutine(AutoFireRoutine());
    }

    private void FixedUpdate()
    {
        RotateTurret();
        LoadLivesImage();
    }

    IEnumerator AutoFireRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(aimingTime);

            if (canFire())
            {
                SetRandomLaunchForce();
                Fire();
            }
        }
    }

    bool canFire()
    {
        return Time.time >= lastFireTime + cooldown;
    }

    void SetRandomLaunchForce()
    {
        launchForce = Random.Range(minLaunchForce, maxLaunchForce); // Set random force value between min and max
    }

    void Fire()
    {
        if (turret.transform.rotation.z > 0 && turret.transform.rotation.z < 55)
        {
            GameObject newMissile = Instantiate(missile, firePoint.transform.position, firePoint.transform.rotation);

            audioSource.Play();

            Rigidbody2D missileRb = newMissile.GetComponent<Rigidbody2D>();
            missileRb.velocity = firePoint.transform.up * launchForce;

            lastFireTime = Time.time;
        }
    }

    void RotateTurret()
    {
        float zRotation = turret.transform.eulerAngles.z;

        if (zRotation > 180)
            zRotation -= 360;

        if (zRotation >= 65)
            hasRotatedRight = true;

        if (zRotation <= -65)
            hasRotatedRight = false;

        if (!hasRotatedRight)                                       // Rotate right
            turret.transform.Rotate(new Vector3(0, 0, 1), 65 * Time.deltaTime);

        if (hasRotatedRight)
            turret.transform.Rotate(new Vector3(0, 0, 1), -65 * Time.deltaTime);
    }

    void DisplayResult()
    {
        winnerText.text = "Player Wins";
        gameOverPanel.SetActive(true);
    }

    void LoadLivesImage()
    {
        if (lives == 3)
            liveImage.sprite = liveSprites[3];

        else if (lives == 2)
            liveImage.sprite = liveSprites[2];

        else if (lives == 1)
            liveImage.sprite = liveSprites[1];

        else
            liveImage.sprite = liveSprites[0];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Missile"))
        {
            if (lives > 1)
            {
                lives--;
                playerScript.IncreaseCoins(10);
            }

            else
            {
                Time.timeScale = 0f;
                loserName = gameObject.name;
                DisplayResult();
                Destroy(gameObject);
            }
        }
    }
}
