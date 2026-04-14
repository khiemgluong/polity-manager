using UnityEngine;
using UnityEngine.UI;

namespace Polity
{
    public class PolityCamera : MonoBehaviour
    {
        public Image targetImage;
        public RectTransform panel;
        public Button quitButton;
        CanvasGroup canvasGroup;
        Text factionText;
        bool isPaused;
        void Start()
        {
            canvasGroup = targetImage.GetComponent<CanvasGroup>();
            Transform t = targetImage.transform;
            factionText = t.Find("Faction").GetComponent<Text>();
            /* --------------------------- FamilyStruct texts --------------------------- */

            panel.gameObject.SetActive(false);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked); // Add listener
        }

        void OnQuitButtonClicked()
        {
            Debug.Log("Quit button clicked!");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        void Update()
        {
            if (targetImage == null)
            {
                Debug.LogError("Target Image not assigned in the inspector");
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    panel.gameObject.SetActive(true);
                    Time.timeScale = 0;
                }
                else
                {
                    panel.gameObject.SetActive(false);
                    Time.timeScale = 1;
                }
            }

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                UnityEngine.Camera.main.fieldOfView -= scroll * 15f;
                UnityEngine.Camera.main.fieldOfView = Mathf.Clamp(UnityEngine.Camera.main.fieldOfView, 45, 135);
            }

            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                if (hit.collider.TryGetComponent<IMember>(out var member))
                {
                    canvasGroup.alpha = 1;
                    factionText.text = member.Faction.Name;
                }
                else
                {
                    canvasGroup.alpha = 0;
                    factionText.text = "";
                }

            }
            else canvasGroup.alpha = 0;
            Vector2 mousePosition = Input.mousePosition;
            RectTransform parentRect = targetImage.rectTransform.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                mousePosition,
                targetImage.canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetImage.canvas.worldCamera,
                out Vector2 localPoint);

            targetImage.rectTransform.anchoredPosition = localPoint;
        }
    }
}