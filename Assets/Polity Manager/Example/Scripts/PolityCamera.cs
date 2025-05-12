using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KL
{
    using static KL.PolityManager;
    using static KL.PolityMember;
    public class PolityCamera : MonoBehaviour
    {
        public Image targetImage;
        public RectTransform panel;
        public Button quitButton;
        RawImage emblem;
        CanvasGroup canvasGroup;
        TextMeshProUGUI memberName, memberPolity, classText, fationText;
        TextMeshProUGUI parentName, partnerName, childrenName;
        bool isPaused;
        void Start()
        {
            canvasGroup = targetImage.GetComponent<CanvasGroup>();
            Transform t = targetImage.transform;
            emblem = t.Find("Emblem").GetComponent<RawImage>();
            memberName = t.Find("Name").GetComponent<TextMeshProUGUI>();
            memberPolity = t.Find("Polity").GetComponent<TextMeshProUGUI>();
            classText = t.Find("Class").GetComponent<TextMeshProUGUI>();
            fationText = t.Find("Faction").GetComponent<TextMeshProUGUI>();
            /* --------------------------- FamilyStruct texts --------------------------- */
            parentName = t.Find("Parents").GetComponent<TextMeshProUGUI>();
            partnerName = t.Find("Partners").GetComponent<TextMeshProUGUI>();
            childrenName = t.Find("Children").GetComponent<TextMeshProUGUI>();

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
                if (hit.collider.TryGetComponent<PolityMember>(out var polityMember))
                {
                    canvasGroup.alpha = 1;
                    memberName.text = polityMember.name;

                    PolityStruct polityStruct = polityMember.reader.Struct;
                    memberPolity.text = polityStruct.polityName;
                    //Get the emblem of just the polity if available
                    PolityStruct emblemStruct = new()
                    {
                        polityName = polityStruct.polityName
                    };
                    Texture emblemTexture = PM.GetPolityEmblem(emblemStruct);
                    if (emblemTexture != null)
                    {
                        emblem.gameObject.SetActive(true);
                        emblem.texture = emblemTexture;
                    }
                    else emblem.gameObject.SetActive(false);
                    if (polityStruct.className.Equals("\t"))
                    {
                        classText.gameObject.SetActive(false);
                        fationText.gameObject.SetActive(false);
                    }
                    else
                    {
                        classText.gameObject.SetActive(true);
                        classText.text = polityStruct.className;
                    }
                    if (polityStruct.factionName.Equals("\t"))
                        fationText.gameObject.SetActive(false);
                    else
                    {
                        fationText.gameObject.SetActive(true);
                        fationText.text = polityStruct.factionName;
                    }
                    /* --------------------------- FamilyStruct texts --------------------------- */

                    FamilyStruct familyStruct = polityMember.family;
                    if (familyStruct.parents.Count == 0)
                        parentName.gameObject.SetActive(false);
                    else
                    {
                        parentName.gameObject.SetActive(true);
                        if (familyStruct.parents.Count > 1)
                            parentName.text = "Parents: " + familyStruct.parents.Count;
                        else parentName.text = "Parent: " + familyStruct.parents[0].name;
                    }
                    if (familyStruct.partners.Count == 0)
                        partnerName.gameObject.SetActive(false);
                    else
                    {
                        partnerName.gameObject.SetActive(true);
                        if (familyStruct.partners.Count > 1)
                            partnerName.text = "Partners: " + familyStruct.partners.Count;
                        else partnerName.text = "Partner: " + familyStruct.partners[0].name;
                    }
                    if (familyStruct.children.Count == 0)
                        childrenName.gameObject.SetActive(false);
                    else
                    {
                        childrenName.gameObject.SetActive(true);
                        if (familyStruct.children.Count > 1)
                            childrenName.text = "Children: " + familyStruct.children.Count;
                        else childrenName.text = "Child: " + familyStruct.children[0].name;
                    }

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
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)targetImage.canvas.transform,
                                        mousePosition, targetImage.canvas.worldCamera, out Vector2 canvasPosition);
            targetImage.rectTransform.anchoredPosition = canvasPosition;
        }
    }
}