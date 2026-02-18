using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject failText;

    private void Start()
    {
        if (playButton != null)
        {
            var buttonComponents = playButton.GetComponents<Component>();
            foreach (var comp in buttonComponents)
            {
                if (comp.GetType().Name == "Button")
                {
                    var buttonType = comp.GetType();
                    var onClickField = buttonType.GetProperty("onClick");
                    if (onClickField != null)
                    {
                        var onClickEvent = onClickField.GetValue(comp);
                        var addListenerMethod = onClickEvent.GetType().GetMethod("AddListener");
                        addListenerMethod.Invoke(onClickEvent, new object[] { (UnityEngine.Events.UnityAction)OnPlayButtonClicked });
                    }
                    break;
                }
            }
        }

        HideAllUI();
    }

    private void OnDestroy()
    {
    }

    private void OnPlayButtonClicked()
    {
        Debug.Log("[GameUIManager] Play button clicked");
        GameStateMachine.Instance?.TransitionTo<State_Playing>();
    }

    public void ShowPlayButton()
    {
        if (playButton != null)
        {
            playButton.gameObject.SetActive(true);
            Debug.Log("[GameUIManager] Play button shown");
        }
    }

    public void HidePlayButton()
    {
        if (playButton != null)
        {
            playButton.gameObject.SetActive(false);
            Debug.Log("[GameUIManager] Play button hidden");
        }
    }

    public void ShowWinText()
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            Debug.Log("[GameUIManager] Win text shown");
        }
    }

    public void HideWinText()
    {
        if (winText != null)
        {
            winText.gameObject.SetActive(false);
        }
    }

    public void ShowFailText()
    {
        if (failText != null)
        {
            failText.gameObject.SetActive(true);
            Debug.Log("[GameUIManager] Fail text shown");
        }
    }

    public void HideFailText()
    {
        if (failText != null)
        {
            failText.gameObject.SetActive(false);
        }
    }

    private void HideAllUI()
    {
        HidePlayButton();
        HideWinText();
        HideFailText();
    }
}
