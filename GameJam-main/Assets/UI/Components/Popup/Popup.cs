using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Components
{
    public class Popup
    {
        private readonly UIDocument doc;
        private readonly string name;
        private bool isHelpVisible = false;
        private readonly System.Action onClose = null;

        public Popup(string popupName, UIDocument document, System.Action action = null)
        {
            doc = document;
            onClose = action;
            name = popupName;
            if (doc != null && doc.rootVisualElement != null)
            {
                doc.rootVisualElement.visible = false;
            }
        }

        public void Close()
        {
            if (doc != null && doc.rootVisualElement != null)
            {
                doc.rootVisualElement.visible = false;
                isHelpVisible = false; // FIX: Update the flag
                onClose?.Invoke();
            }
            else
            {
                Debug.LogError("popup is null");
            }
        }

        public void SetPosition(UIDocument target, float xPercent = 50f, float yPercent = 50f)
        {
            if (target == null || doc == null || doc.rootVisualElement == null) return;

            VisualElement targetRoot = target.rootVisualElement;
            VisualElement docRoot = doc.rootVisualElement;

            // docRoot.style.position = Position.Absolute;

            // float centerX = (targetRoot.layout.width - 500) / 2f;
            // float centerY = (targetRoot.layout.height - 500) / 2f;

            // docRoot.style.left = centerX;
            // docRoot.style.top = centerY;
        }

        public void Open()
        {
            if (doc != null && doc.rootVisualElement != null)
            {
                doc.rootVisualElement.visible = true;
                isHelpVisible = true; // FIX: Update the flag
            }
            else
            {
                Debug.LogError($"popup is null for open.");
            }
        }

        public bool Toggle()
        {
            if (isHelpVisible)
            {
                Close();
                return false;
            }
            else
            {
                Open();
                return true;
            }
        }

        public void AssignActionToButton(string buttonName, System.Action action)
        {
            var button = doc.rootVisualElement.Q<Button>(buttonName);
            if (button != null)
            {
                button.clicked += action;
            }
            else
            {
                Debug.LogError($"Could not find '{buttonName}' in UI Document");
            }
        }
    }
}