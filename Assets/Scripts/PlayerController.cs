using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera currentCamera;
    public TileSlideController slideController;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            slideController.Undo();
        }
    }

    private void HandleTileClick()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = currentCamera.ScreenPointToRay(mousePosition);

        bool somethingIsHit = Physics.Raycast(ray, out RaycastHit raycastHit);

        if (somethingIsHit)
        {
            if (raycastHit.transform.TryGetComponent<TileBase>(out var tileBase))
            {
                bool slideSuccess = slideController.TrySlide(tileBase.gridPosition);
                
                if (slideSuccess)
                {
                    Debug.Log($"Tile slid from {tileBase.gridPosition} to empty position");
                }
            }
        }
    }
}
