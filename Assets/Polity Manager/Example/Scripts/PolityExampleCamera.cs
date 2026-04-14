using UnityEngine;
using UnityEngine.UI;

namespace Polity
{
    public class PolityExampleCamera : MonoBehaviour
    {
        PolityExampleInputActions inputActions;
        PolitySpawner spawner;
        Canvas canvas;
        CanvasGroup floating;
        public RectTransform overlay;
        RectTransform floatingRect;
        public Button quitButton;
        public GameObject cursor;
        Text factionText, selectedMemberText;
        bool isPaused;
        IMember selectedMember;
        void Awake()
        {
            inputActions = new PolityExampleInputActions();
            inputActions.Enable();
            inputActions.Main.Enable();

            spawner = FindFirstObjectByType<PolitySpawner>();
            canvas = FindFirstObjectByType<Canvas>();
            floating = canvas.GetComponentInChildren<CanvasGroup>();
            floatingRect = floating.GetComponent<RectTransform>();

            inputActions.Main.Pause.performed += ctx =>
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    overlay.gameObject.SetActive(true);
                    cursor.SetActive(false);
                    Time.timeScale = 0;
                }
                else
                {
                    overlay.gameObject.SetActive(false);
                    if (!cursor.activeSelf)
                        cursor.SetActive(true);
                    Time.timeScale = 1;
                }
            };
        }
        void Start()
        {
            Transform t = floating.transform;
            factionText = t.Find("Faction").GetComponent<Text>();
            selectedMemberText = canvas.transform.Find("SelectedMember").GetComponent<Text>();
            /* --------------------------- FamilyStruct texts --------------------------- */

            overlay.gameObject.SetActive(false);
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                if (hit.collider.TryGetComponent<IMember>(out var member))
                {
                    floating.alpha = 1;
                    factionText.text = member.Faction.Name;
                    if (inputActions.Main.Select.WasPressedThisFrame())
                    {
                        if (selectedMember == member)
                            selectedMember = null;
                        else
                            selectedMember = member;
                    }
                }
                else
                {
                    floating.alpha = 0;
                    factionText.text = "";
                    if (inputActions.Main.Select.WasPressedThisFrame())
                        spawner.SpawnNPC(hit.point);
                }
                cursor.transform.position = hit.point + Vector3.up * .01f;
            }
            else
            {
                floating.alpha = 0;
                factionText.text = "";
                cursor.SetActive(false);
            }

            if (selectedMember != null)
            {
                Debug.Log("Selected member: " + selectedMember.transform.name);
                selectedMemberText.text = selectedMember.transform.name;
            }
            else
            {
                selectedMemberText.text = "";
            }
            Vector2 mousePosition = Input.mousePosition;
            RectTransform parentRect = floatingRect.parent as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                mousePosition,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector2 localPoint);

            floatingRect.anchoredPosition = localPoint;
        }

        void OnDestroy()
        {
            inputActions.Dispose();
        }

        void OnQuitButtonClicked()
        {
            Debug.Log("Quit button clicked!");
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}