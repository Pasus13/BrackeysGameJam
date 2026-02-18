using System.Collections;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TileGrid tileGrid;
    [SerializeField] private BoardData boardData;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float heightOffset = 1f;

    public delegate void GoalReachedHandler();
    public event GoalReachedHandler OnGoalReached;

    public delegate void MoveFailedHandler();
    public event MoveFailedHandler OnMoveFailed;

    private Vector2Int _currentGridPosition;
    private Vector2Int _moveDirection;
    private Coroutine _moveCoroutine;
    private bool _isMoving = false;

    private void Start()
    {
        if (boardData != null)
        {
            _currentGridPosition = boardData.characterStartPosition;
            _moveDirection = new Vector2Int((int)boardData.characterStartDirection.x, (int)boardData.characterStartDirection.z);

            Vector3 startWorldPos = tileGrid.GridToWorldPosition(_currentGridPosition);
            startWorldPos.y += heightOffset;
            transform.position = startWorldPos;

            Debug.Log($"[CharacterMover] Initialized at {_currentGridPosition}, direction: {_moveDirection}");
        }

        OnGoalReached += HandleGoalReached;
        OnMoveFailed += HandleMoveFailed;
    }

    private void OnDestroy()
    {
        OnGoalReached -= HandleGoalReached;
        OnMoveFailed -= HandleMoveFailed;
    }

    private void HandleGoalReached()
    {
        Debug.Log("[CharacterMover] Goal reached, transitioning to State_Win");
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.TransitionTo<State_Win>();
        }
    }

    private void HandleMoveFailed()
    {
        Debug.Log("[CharacterMover] Move failed, transitioning to State_Fail");
        if (GameStateMachine.Instance != null)
        {
            GameStateMachine.Instance.TransitionTo<State_Fail>();
        }
    }

    public void StartMoving()
    {
        if (_moveCoroutine == null)
        {
            _moveCoroutine = StartCoroutine(MoveRoutine());
            Debug.Log("[CharacterMover] Started moving");
        }
    }

    public void StopMoving()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
            _isMoving = false;
            Debug.Log("[CharacterMover] Stopped moving");
        }
    }

    private IEnumerator MoveRoutine()
    {
        _isMoving = true;

        while (_isMoving)
        {
            Vector2Int nextPosition = _currentGridPosition + _moveDirection;

            GameObject nextTile = tileGrid.GetTile(nextPosition);
            if (nextTile == null)
            {
                Debug.Log("[CharacterMover] No tile ahead, movement failed");
                OnMoveFailed?.Invoke();
                _isMoving = false;
                yield break;
            }

            TileComponent tileComponent = nextTile.GetComponent<TileComponent>();
            if (tileComponent != null && tileComponent.tileData != null)
            {
                if (tileComponent.tileData.isLocked)
                {
                    Debug.Log("[CharacterMover] Hit locked tile, movement failed");
                    OnMoveFailed?.Invoke();
                    _isMoving = false;
                    yield break;
                }
            }

            yield return StartCoroutine(MoveToTile(nextPosition));

            if (nextPosition == boardData.goalPosition)
            {
                Debug.Log("[CharacterMover] Reached goal!");
                OnGoalReached?.Invoke();
                _isMoving = false;
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator MoveToTile(Vector2Int targetGridPos)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = tileGrid.GridToWorldPosition(targetGridPos);
        targetPos.y += heightOffset;

        float elapsedTime = 0f;
        float duration = 1f / moveSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        _currentGridPosition = targetGridPos;

        Debug.Log($"[CharacterMover] Moved to {_currentGridPosition}");
    }

    public Vector2Int GetCurrentPosition()
    {
        return _currentGridPosition;
    }

    public void SetDirection(Vector2Int newDirection)
    {
        _moveDirection = newDirection;
        Debug.Log($"[CharacterMover] Direction changed to {_moveDirection}");
    }
}
