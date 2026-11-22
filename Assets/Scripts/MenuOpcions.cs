using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuOpcions : MonoBehaviour
{
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private Camera headCamera;
    [SerializeField, Min(0.1f)] private float autoCloseDistance = 2f;

    private InputAction openBookAction;
    private GameObject bookInstance;
    private Vector3 playerPositionWhenOpened;
    private Transform buttonsRoot;
    private TextMeshProUGUI pageTitle;
    private TextMeshProUGUI pageDescription;

    private void OnEnable()
    {
        if (headCamera == null)
            headCamera = Camera.main;

        if (bookPrefab == null || headCamera == null)
        {
            Debug.LogError("Menu: assigna el prefab del llibre i la càmera XR a l'Inspector.", this);
            enabled = false;
            return;
        }

        openBookAction = new InputAction("OpenBook", InputActionType.Button);
        openBookAction.AddBinding("<Keyboard>/escape");
        openBookAction.AddBinding("<XRController>{LeftHand}/primaryButton");
        openBookAction.AddBinding("<XRController>{RightHand}/primaryButton");
        openBookAction.performed += ToggleBook;
        openBookAction.Enable();
    }

    private void OnDisable()
    {
        if (openBookAction != null)
        {
            openBookAction.performed -= ToggleBook;
            openBookAction.Disable();
            openBookAction.Dispose();
            openBookAction = null;
        }

        CloseBook();
    }

    private void Update()
    {
        if (bookInstance == null)
            return;

        Vector3 playerPosition = headCamera.transform.position;
        playerPosition.y = 0f;
        if ((playerPosition - playerPositionWhenOpened).sqrMagnitude >=
            autoCloseDistance * autoCloseDistance)
            CloseBook();
    }

    private void ToggleBook(InputAction.CallbackContext context)
    {
        if (bookInstance != null)
        {
            CloseBook();
            return;
        }

        Vector3 forward = Vector3.ProjectOnPlane(headCamera.transform.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;
        forward.Normalize();

        Vector3 position = headCamera.transform.position + forward * 0.65f;
        if (!TryGetGroundHeight(position, out float groundHeight))
        {
            Debug.LogWarning("No s'ha trobat el terreny sota el jugador; el llibre es col·locarà a y=0.", this);
            groundHeight = 0f;
        }

        playerPositionWhenOpened = headCamera.transform.position;
        playerPositionWhenOpened.y = 0f;
        position.y = groundHeight;
        bookInstance = Instantiate(bookPrefab, position, Quaternion.LookRotation(-forward, Vector3.up));
        bookInstance.SetActive(true);
        AlignBookToGround(bookInstance, groundHeight);
        if (!CreateBookMenu(bookInstance.transform))
        {
            CloseBook();
            return;
        }

        ShowMainPage();
    }

    private bool TryGetGroundHeight(Vector3 position, out float groundHeight)
    {
        foreach (Terrain terrain in Terrain.activeTerrains)
        {
            if (terrain.terrainData == null)
                continue;

            Vector3 localPosition = position - terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;
            if (localPosition.x < 0f || localPosition.x > terrainSize.x ||
                localPosition.z < 0f || localPosition.z > terrainSize.z)
                continue;

            groundHeight = terrain.SampleHeight(position) + terrain.transform.position.y;
            return true;
        }

        Vector3 rayOrigin = position + Vector3.up * 25f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            groundHeight = hit.point.y;
            return true;
        }

        groundHeight = 0f;
        return false;
    }

    private static void AlignBookToGround(GameObject book, float groundHeight)
    {
        Renderer[] renderers = book.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogError("El prefab del llibre no té cap Renderer.", book);
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        book.transform.position += Vector3.up * (groundHeight - bounds.min.y);
    }

    private bool CreateBookMenu(Transform book)
    {
        Canvas canvas = book.GetComponentInChildren<Canvas>(true);
        if (canvas == null)
        {
            Debug.LogError("El prefab Llibre necessita un Canvas fill.", this);
            return false;
        }

        canvas.gameObject.SetActive(true);
        pageTitle = CreateLabel(canvas.transform, "Títol", 170f, 780f, 40f, 30f);
        pageDescription = CreateLabel(canvas.transform, "Descripció", 118f, 780f, 56f, 22f);

        GameObject buttonRoot = new GameObject("Opcions", typeof(RectTransform));
        buttonRoot.transform.SetParent(canvas.transform, false);
        RectTransform buttonRootRect = buttonRoot.GetComponent<RectTransform>();
        buttonRootRect.anchorMin = Vector2.zero;
        buttonRootRect.anchorMax = Vector2.one;
        buttonRootRect.offsetMin = Vector2.zero;
        buttonRootRect.offsetMax = Vector2.zero;
        buttonsRoot = buttonRoot.transform;
        return true;
    }

    private static TextMeshProUGUI CreateLabel(
        Transform parent, string objectName, float y, float width, float height, float fontSize)
    {
        GameObject labelObject = new GameObject(objectName, typeof(RectTransform));
        labelObject.transform.SetParent(parent, false);

        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(width, height);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.fontSize = fontSize;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.16f, 0.09f, 0.04f);
        label.raycastTarget = false;
        return label;
    }

    private void ShowMainPage()
    {
        pageTitle.text = "MENÚ";
        pageDescription.text = "Dracula vs Gnomes";
        ClearButtons();
        AddButton("CONTINUAR", 40f, CloseBook);
        AddButton("AJUSTOS", -28f, ShowSettingsPage);
        AddButton("CONTROLS", -96f, ShowControlsPage);
        AddButton("SORTIR", -164f, exit);
    }

    private void ShowSettingsPage()
    {
        pageTitle.text = "AJUSTOS";
        UpdateVolumeDescription();
        ClearButtons();
        AddButton("VOLUM -", 35f, () => ChangeVolume(-0.1f));
        AddButton("VOLUM +", -45f, () => ChangeVolume(0.1f));
        AddButton("ENRERE", -155f, ShowMainPage);
    }

    private void ShowControlsPage()
    {
        pageTitle.text = "CONTROLS";
        pageDescription.text = "ESC o X/A: obrir o tancar el llibre\nApunta als botons del llibre i prem el gallet.";
        ClearButtons();
        AddButton("ENRERE", -155f, ShowMainPage);
    }

    private void UpdateVolumeDescription()
    {
        pageDescription.text = $"Volum del joc: {Mathf.RoundToInt(AudioListener.volume * 100f)}%";
    }

    private void ChangeVolume(float amount)
    {
        AudioListener.volume = Mathf.Clamp01(AudioListener.volume + amount);
        UpdateVolumeDescription();
    }

    private void AddButton(string label, float y, UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(
            label,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button));
        buttonObject.transform.SetParent(buttonsRoot, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, y);
        rect.sizeDelta = new Vector2(720f, 54f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.22f, 0.12f, 0.06f, 0.92f);

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(onClick);
        button.navigation = new Navigation { mode = Navigation.Mode.None };

        TextMeshProUGUI text = CreateLabel(buttonObject.transform, "Text", 0f, 700f, 48f, 26f);
        text.text = label;
        text.color = Color.white;
    }

    private void ClearButtons()
    {
        if (buttonsRoot == null)
            return;

        for (int i = buttonsRoot.childCount - 1; i >= 0; i--)
            Destroy(buttonsRoot.GetChild(i).gameObject);
    }

    private void CloseBook()
    {
        if (bookInstance == null)
            return;

        Destroy(bookInstance);
        bookInstance = null;
        buttonsRoot = null;
        pageTitle = null;
        pageDescription = null;
    }

    public void jugar()
    {
        Debug.Log("JUGAR!!!");

        // aqui ja ficarem la resta de coses necessaries
    }

    public void nivells()
    {
        Debug.Log("NIVELLS!");

        // ficarem elq calgui
    }

    public void exit()
    {
        Debug.Log("SORTIM DEL JOC!"); // ho poso per ara pq l'editor no tanca el joc
        Application.Quit();
    }
}
