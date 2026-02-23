using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject gameMenuPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Win Panel Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button retryButtonWin;
    [SerializeField] private Button mainMenuButtonWin;

    [Header("Fail Panel Buttons")]
    [SerializeField] private Button retryButtonFail;
    [SerializeField] private Button mainMenuButtonFail;

    [Header("Game Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button retryButtonGameMenu;
    [SerializeField] private Button mainMenuButtonGameMenu;

    [Header("Credits Panel Buttons")]
    [SerializeField] private Button mainMenuButtonCredits;

    private Button _playButtonComponent;
    private bool _isPlayButtonEnabled = true;


    public void OnPlayButtonClicked()
    {
        if (!_isPlayButtonEnabled)
        {
            return;
        }

        _isPlayButtonEnabled = false;
        
        if (_playButtonComponent != null)
        {
            _playButtonComponent.interactable = false;
        }

        CharacterMover characterMover = FindAnyObjectByType<CharacterMover>();
        if (characterMover != null && BoardManager.Instance != null)
        {
            int characterBoard = characterMover.GetCurrentBoardIndex();
            if (BoardManager.Instance.ActiveBoardIndex != characterBoard)
            {
                BoardManager.Instance.SetActiveBoard(characterBoard);
                Debug.Log($"[GameUIManager] Switched to character's board {characterBoard} on Play");
            }
        }

        GameStateMachine.Instance?.TransitionTo<State_Playing>();
    }

    public void ShowPlayButton()
    {
        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
            _isPlayButtonEnabled = true;
            
            if (_playButtonComponent != null)
            {
                _playButtonComponent.interactable = true;
            }
        }
    }

    public void HidePlayButton()
    {
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
        }
    }

    public void ShowCreditsPanel()
    {
        creditsPanel.SetActive(true);
    }

    public void HideCreditsPanel()
    {
        creditsPanel.SetActive(false);
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            // Debug.Log("[GameUIManager] Win panel shown");
        }
    }

    public void HideWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    public void ShowFailPanel()
    {
        if (failPanel != null)
        {
            failPanel.SetActive(true);
            // Debug.Log("[GameUIManager] Fail panel shown");
        }
    }

    public void HideFailPanel()
    {
        if (failPanel != null)
        {
            failPanel.SetActive(false);
        }
    }

    public void ShowGameMenu()
    {
        if (gameMenuPanel != null)
        {
            gameMenuPanel.SetActive(true);
            Debug.Log("[GameUIManager] Game menu shown");
        }
    }

    public void HideGameMenu()
    {
        if (gameMenuPanel != null)
        {
            gameMenuPanel.SetActive(false);
        }
    }

    public void OnResumeButtonClicked()
    {
        Debug.Log("[GameUIManager] Resume button clicked");
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Resume();
        }
    }


    public void OnNextButtonClicked()
    {
        Debug.Log("[GameUIManager] Next button clicked");
        
        HideWinPanel();

        bool nextLevelLoaded = false;
        if (LevelManager.Instance != null)
        {
            if (LevelManager.Instance.HasNextLevel)
            {
                nextLevelLoaded = LevelManager.Instance.LoadNextLevel();
            }
            else
            {
                Debug.Log("[GameUIManager] No more levels!");
                ShowCreditsPanel();
            }
        }
        
        if (GameStateMachine.Instance != null && nextLevelLoaded)
        {
            GameStateMachine.Instance.TransitionTo<State_Setup>();
        }
    }

    public void OnRetryButtonClicked()
    {
        Debug.Log("[GameUIManager] Retry button clicked");
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ForceResume();
        }
        
        HideWinPanel();
        HideFailPanel();
        HideGameMenu();
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ReloadCurrentLevel();
        }
        
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.TransitionTo<State_Setup>();
        }
    }

    public void OnMainMenuButtonClicked()
    {
        Debug.Log("[GameUIManager] Main Menu button clicked - Loading Main Menu");
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ForceResume();
        }
        
        HideAllUI();
        
        SceneManager.LoadScene("MainMenu");
    }

    private void HideAllUI()
    {
        HidePlayButton();
        HideWinPanel();
        HideFailPanel();
        HideGameMenu();
        HideCreditsPanel();
    }
}
