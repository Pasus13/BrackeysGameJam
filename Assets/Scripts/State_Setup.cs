using UnityEngine;

public class State_Setup : IGameState
{
    private readonly GameStateMachine _stateMachine;
    private readonly InputManager _inputManager;
    private readonly GameUIManager _uiManager;

    public State_Setup(GameStateMachine stateMachine, InputManager inputManager, GameUIManager uiManager)
    {
        _stateMachine = stateMachine;
        _inputManager = inputManager;
        _uiManager = uiManager;
    }

    public void Enter()
    {
        Debug.Log("[State_Setup] Entered");

        if (_inputManager != null)
        {
            _inputManager.SetInputEnabled(true);
        }

        if (_uiManager != null)
        {
            _uiManager.ShowPlayButton();
            _uiManager.HideWinText();
            _uiManager.HideFailText();
        }
    }

    public void Exit()
    {
        Debug.Log("[State_Setup] Exited");

        if (_uiManager != null)
        {
            _uiManager.HidePlayButton();
        }
    }

    public void Tick()
    {
    }
}
