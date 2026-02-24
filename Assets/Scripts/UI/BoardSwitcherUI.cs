using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI boardInfoText;
    [SerializeField] private TextMeshProUGUI levelCounterText;

    [Header("Settings")]
    [SerializeField] private bool startHidden = false;

    private void Start()
    {
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.OnBoardChanged += OnBoardChanged;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLoaded += OnLevelLoaded;
        }

        if (startHidden)
        {
            gameObject.SetActive(false);
        }
        else
        {
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.OnBoardChanged -= OnBoardChanged;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLoaded -= OnLevelLoaded;
        }
    }

    public void OnPreviousButtonClicked()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
        {
            Debug.Log("[HudUI] Cannot switch board while paused");
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.SwitchToPreviousBoard();
        }
    }

    public void OnNextButtonClicked()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
        {
            Debug.Log("[HudUI] Cannot switch board while paused");
            return;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        if (BoardManager.Instance != null)
        {
            BoardManager.Instance.SwitchToNextBoard();
        }
    }

    private void OnBoardChanged(int previousIndex, int newIndex)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (BoardManager.Instance == null)
        {
            if (boardInfoText != null)
            {
                boardInfoText.text = "No BoardManager";
            }
            return;
        }

        UpdateButtonStates();
        UpdateBoardInfo();
        UpdateLevelCounter();
    }

    private void UpdateButtonStates()
    {
        int boardCount = BoardManager.Instance.BoardCount;
        bool showButtons = boardCount > 1;

        if (previousButton != null)
        {
            previousButton.gameObject.SetActive(showButtons);
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(showButtons);
        }
    }

    private void UpdateBoardInfo()
    {
        if (boardInfoText == null) return;

        int currentIndex = BoardManager.Instance.ActiveBoardIndex;
        int totalBoards = BoardManager.Instance.BoardCount;

        if (totalBoards == 0)
        {
            boardInfoText.text = "No Boards Loaded";
            return;
        }

        Board activeBoard = BoardManager.Instance.ActiveBoard;
        if (activeBoard != null && activeBoard.BoardData != null)
        {
            boardInfoText.text = $"Board {currentIndex + 1}/{totalBoards}\n{activeBoard.BoardData.name}";
        }
        else
        {
            boardInfoText.text = $"Board {currentIndex + 1}/{totalBoards}";
        }
    }

    private void OnLevelLoaded(int levelIndex)
    {
        UpdateLevelCounter();
    }

    private void UpdateLevelCounter()
    {
        if (levelCounterText == null) return;

        if (LevelManager.Instance == null)
        {
            levelCounterText.text = "Level: --/--";
            return;
        }

        int currentLevel = LevelManager.Instance.CurrentLevelIndex + 1;
        int totalLevels = LevelManager.Instance.TotalLevels;

        levelCounterText.text = $"Level: {currentLevel}/{totalLevels}";
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
