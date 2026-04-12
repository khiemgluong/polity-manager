using UnityEngine;
using UnityEngine.UI;

namespace Polity
{
    using static Polity.Manager;
    using static Polity.Member;
    public class PolityCamera : MonoBehaviour
    {
        public Image targetImage;
        public RectTransform panel;
        public Button quitButton;
        CanvasGroup canvasGroup;
        Text memberName, memberPolity, classText, fationText;
        Text parentName, partnerName, childrenName;
        bool isPaused;
        void Start()
        {
            canvasGroup = targetImage.GetComponent<CanvasGroup>();
            Transform t = targetImage.transform;
            // emblem = t.Find("Emblem").GetComponent<RawImage>();
            memberName = t.Find("Name").GetComponent<Text>();
            memberPolity = t.Find("Polity").GetComponent<Text>();
            classText = t.Find("Class").GetComponent<Text>();
            fationText = t.Find("Faction").GetComponent<Text>();
            /* --------------------------- FamilyStruct texts --------------------------- */
            parentName = t.Find("Parents").GetComponent<Text>();
            partnerName = t.Find("Partners").GetComponent<Text>();
            childrenName = t.Find("Children").GetComponent<Text>();

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
                Camera.main.fieldOfView -= scroll * 15f;
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 45, 135);
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                if (hit.collider.TryGetComponent<Member>(out var polityMember))
                {
                    canvasGroup.alpha = 1;
                    memberName.text = polityMember.name;

                    Faction polityStruct = polityMember.faction;
                    memberPolity.text = polityStruct.name;
                    //Get the emblem of just the polity if available
                    Faction emblemStruct = new()
                    {
                        name = polityStruct.name
                    };
                    // Texture emblemTexture = Singleton.GetPolityEmblem(emblemStruct);
                    // if (emblemTexture != null)
                    // {
                    //     emblem.gameObject.SetActive(true);
                    //     emblem.texture = emblemTexture;
                    // }
                    // else emblem.gameObject.SetActive(false);
                    // if (string.IsNullOrEmpty(polityStruct.coalitionName)
                    //     || polityStruct.coalitionName.Equals("\t"))
                    // {
                    //     classText.gameObject.SetActive(false);
                    //     fationText.gameObject.SetActive(false);
                    // }
                    // else
                    // {
                    //     classText.gameObject.SetActive(true);
                    //     classText.text = polityStruct.coalitionName;
                    // }
                    // if (polityStruct.name.Equals("\t"))
                    //     fationText.gameObject.SetActive(false);
                    // else
                    // {
                    //     fationText.gameObject.SetActive(true);
                    //     fationText.text = polityStruct.name;
                    // }
                    /* --------------------------- FamilyStruct texts --------------------------- */


                }
                else
                {
                    canvasGroup.alpha = 0;
                    memberName.text = "";
                    memberPolity.text = "";
                    fationText.text = "";

                    parentName.text = "";
                    partnerName.text = "";
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