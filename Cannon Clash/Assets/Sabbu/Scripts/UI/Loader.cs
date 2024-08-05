using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Loader : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public GameObject shopPanel;

    public Image[] skinImages;       // Array of images representing each skin
    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public TextMeshProUGUI coinsText;

    [Header("Skin Settings")]
    public Material[] availableSkins;
    private int selectedSkinIndex = 0;

    private int getCoins;

    private void Start()
    {
        getCoins = PlayerPrefs.GetInt("Coins", 0);

        if (coinsText != null)
        {
            coinsText.text = ("You have " + getCoins + " coins.");
        }

        // Ensure UnlockedSkins string is long enough
        if (!PlayerPrefs.HasKey("UnlockedSkins"))
        {
            // Assuming you have 4 skins, initialize the string with 4 characters, '1' for unlocked, '0' for locked
            PlayerPrefs.SetString("UnlockedSkins", "1000");
        }
        else
        {
            // Ensure the string is long enough
            string unlockedSkins = PlayerPrefs.GetString("UnlockedSkins");
            if (unlockedSkins.Length < 4)
            {
                unlockedSkins = unlockedSkins.PadRight(4, '0'); // Pad with '0's to ensure length is 4
                PlayerPrefs.SetString("UnlockedSkins", unlockedSkins);
            }
        }

        UpdateSkinUI();
    }

    // Methods to set specific skins
    public void SetSkin0()
    {
        SelectSkin(0);
    }

    public void SetSkin1()
    {
        SelectSkin(1);
    }

    public void SetSkin2()
    {
        SelectSkin(2);
    }

    public void SetSkin3()
    {
        SelectSkin(3);
    }

    public void SelectSkin(int index)
    {
        string unlockedSkins = PlayerPrefs.GetString("UnlockedSkins", "1000");

        if (index < unlockedSkins.Length && unlockedSkins[index] == '1')
        {
            selectedSkinIndex = index;
            PlayerPrefs.SetInt("SelectedSkinIndex", selectedSkinIndex);
            Debug.Log("Selected Skin: " + selectedSkinIndex);
        }
        else
        {
            Debug.Log("Skin is locked");
        }
    }

    public void UnlockSkin(int index)
    {
        string unlockedSkins = PlayerPrefs.GetString("UnlockedSkins", "1000");

        if (index < unlockedSkins.Length && unlockedSkins[index] == '0' && getCoins >= 100)
        {
            getCoins -= 100;                                             // Deduct 100 coins when unlocking skin
            PlayerPrefs.SetInt("Coins", getCoins);                       // Save in player prefs
            getCoins = PlayerPrefs.GetInt("Coins", 0);                   // Get coins again
            coinsText.text = ("You have " + getCoins + " coins.");       // Show coins

            char[] unlockedSkinsArray = unlockedSkins.ToCharArray();
            unlockedSkinsArray[index] = '1';
            unlockedSkins = new string(unlockedSkinsArray);
            PlayerPrefs.SetString("UnlockedSkins", unlockedSkins);
            Debug.Log("Skin unlocked: " + index);

            UpdateSkinUI();
        }
        else
        {
            Debug.Log("Not enough coins or skin already unlocked");
        }
    }

    public void UpdateSkinUI()
    {
        string unlockedSkins = PlayerPrefs.GetString("UnlockedSkins", "1000");

        for (int i = 0; i < skinImages.Length; i++)
        {
            if (i < unlockedSkins.Length && unlockedSkins[i] == '1')
            {
                skinImages[i].sprite = unlockedSprite;
            }
            else
            {
                skinImages[i].sprite = lockedSprite;
            }
        }
    }

    public void PlayButton()
    {
        int randomIndex = Random.Range(0, 3);

        SceneManager.LoadScene("2 Player" + randomIndex.ToString());
        Time.timeScale = 1f;
    }

    public void PlayBot()
    {
        int randomIndex = Random.Range(0, 3);

        SceneManager.LoadScene("Bot" + randomIndex.ToString());
        Time.timeScale = 1f;
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

    public void PauseButton()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeButton()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void WatchAd()
    {
        Debug.Log("AD");
        Time.timeScale = 1f;
    }

    public void ShopButton()
    {
        if (shopPanel != null)
        {
            RectTransform rectTransform = shopPanel.GetComponent<RectTransform>();

            // Check the current state and toggle the panel's visibility and scale
            if (shopPanel.activeSelf)
            {
                // Scale down to 0,0 and then deactivate
                rectTransform.DOScale(Vector2.zero, 0.8f).SetEase(Ease.Linear).OnComplete(() => shopPanel.SetActive(false));
            }
            else
            {
                // Activate the panel and scale up to the desired size
                shopPanel.SetActive(true);
                rectTransform.localScale = Vector2.zero; // Ensure the panel starts from 0,0 scale
                rectTransform.DOScale(new Vector2(1, 1), 1f).SetEase(Ease.Linear);
            }
        }
    }
}
