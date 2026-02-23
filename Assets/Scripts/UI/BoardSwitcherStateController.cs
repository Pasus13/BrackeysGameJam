using UnityEngine;

public class BoardSwitcherStateController : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private HudUI hudUI;

    private void Start()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnStateChanged += OnGameStateChanged;
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OnPauseStateChanged += OnPauseStateChanged;
        }

        if (hudUI == null)
        {
            hudUI = GetComponent<HudUI>();
        }

        UpdateVisibility();
    }

    private void OnDestroy()
    {
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.OnStateChanged -= OnGameStateChanged;
        }

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OnPauseStateChanged -= OnPauseStateChanged;
        }
    }

    private void OnGameStateChanged(IGameState newState)
    {
        UpdateVisibility();
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (hudUI == null || GameStateMachine.Instance == null)
            return;

        IGameState currentState = GameStateMachine.Instance.CurrentState;

        bool shouldShow = currentState is State_Setup || 
                         (PauseManager.Instance != null && PauseManager.Instance.IsPaused);

        if (shouldShow)
        {
            hudUI.Show();
        }
        else
        {
            hudUI.Hide();
        }
    }
}
