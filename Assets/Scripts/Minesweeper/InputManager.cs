using UnityEngine;
using UnityEngine.InputSystem;

namespace Minesweeper
{
    public class InputManager : MonoBehaviour
    {
        public Camera mainCam;
    
        // Start is called before the first frame update
        void Start()
        {
            mainCam = Camera.main;
        }
    
        public void OnClickLeft(InputAction.CallbackContext context)
        {
            OnMouseClick(context, ClickType.LEFT);
        }
    
        public void OnClickWheel(InputAction.CallbackContext context)
        {
            OnMouseClick(context, ClickType.WHEEL);
        }
    
        public void OnClickRight(InputAction.CallbackContext context)
        {
            OnMouseClick(context, ClickType.RIGHT);
        }

        private void OnMouseClick(InputAction.CallbackContext context, ClickType clickType)
        {
            if (!context.started) return;
            var rayHit = Physics2D.GetRayIntersection(mainCam.ScreenPointToRay(Mouse.current.position.ReadValue()));
            if (!rayHit.collider) return;

            if (rayHit.collider.TryGetComponent<BoardCellController>(out var cell))
            {
                cell.OnClick(clickType);
            }
        
        }
    }
}