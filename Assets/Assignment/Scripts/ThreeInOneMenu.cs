using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>Shared main menu and Escape pause screen for the three games.</summary>
public sealed class ThreeInOneMenu : MonoBehaviour
{
    private const string MainMenuScene = "MainMenu";
    private static ThreeInOneMenu instance;
    private bool isPaused;
    private GUIStyle brandStyle, accentStyle, subtitleStyle, buttonStyle, footerStyle;
    private Texture2D menuBackground, buttonNormal, buttonHover, buttonActive;
    private int styledHeight;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateForLoadedScene()
    {
        if (instance != null) return;
        var host = new GameObject("Three In One Menu Controller");
        DontDestroyOnLoad(host);
        instance = host.AddComponent<ThreeInOneMenu>();
        SceneManager.sceneLoaded += instance.OnSceneLoaded;
    }

    private void Awake() => menuBackground = Resources.Load<Texture2D>("NeonCoastMenuBG");

    private void OnDestroy()
    {
        if (instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(buttonNormal); Destroy(buttonHover); Destroy(buttonActive);
        instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == MainMenuScene) return;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }

    private void OnGUI()
    {
        EnsureStyles();
        if (SceneManager.GetActiveScene().name == MainMenuScene) DrawMainMenu();
        else if (isPaused) DrawPauseMenu();
    }

    private void DrawMainMenu()
    {
        if (menuBackground != null) DrawCoverTexture(menuBackground);
        else DrawSolid(new Rect(0, 0, Screen.width, Screen.height), new Color(.03f, .02f, .13f));
        DrawSolid(new Rect(0, 0, Screen.width * .46f, Screen.height), new Color(.018f, .012f, .09f, .76f));
        DrawSolid(new Rect(Screen.width * .46f, 0, Screen.width * .16f, Screen.height), new Color(.018f, .012f, .09f, .28f));
        DrawSolid(new Rect(0, 0, 7, Screen.height), new Color(1f, .13f, .54f, .9f));

        float x = Mathf.Max(46, Screen.width * .065f);
        float width = Mathf.Min(Screen.width * .34f, 520);
        GUILayout.BeginArea(new Rect(x, Screen.height * .09f, width, Screen.height * .84f));
        GUILayout.FlexibleSpace();
        GUILayout.Label("MY LITTLE", brandStyle);
        GUILayout.Label("K", accentStyle);
        GUILayout.Space(4);
        GUILayout.Label("THREE WORLDS", subtitleStyle);
        GUILayout.Space(Mathf.Clamp(Screen.height * .045f, 20, 52));
        GameButton("MAD DRIVER", "Prototype 1");
        GameButton("FLY LIKE A BIRD", "Challenge 1");
        GameButton("SUMO BALL", "Challenge 4");
        GUILayout.Space(8);
        if (GUILayout.Button("QUIT GAME", buttonStyle, GUILayout.Height(ButtonHeight()))) QuitGame();
        GUILayout.FlexibleSpace();
        GUILayout.Label("SELECT A GAME  •  ESC TO PAUSE", footerStyle);
        GUILayout.EndArea();
    }

    private void DrawPauseMenu()
    {
        DrawSolid(new Rect(0, 0, Screen.width, Screen.height), new Color(.015f, .008f, .07f, .84f));
        float width = Mathf.Min(Screen.width * .38f, 560);
        GUILayout.BeginArea(new Rect((Screen.width - width) * .5f, Screen.height * .2f, width, Screen.height * .6f));
        GUILayout.FlexibleSpace();
        GUILayout.Label("PAUSED", accentStyle);
        GUILayout.Space(28);
        if (GUILayout.Button("RESUME", buttonStyle, GUILayout.Height(ButtonHeight()))) Resume();
        if (GUILayout.Button("RESTART", buttonStyle, GUILayout.Height(ButtonHeight()))) Restart();
        if (GUILayout.Button("BACK TO MAIN MENU", buttonStyle, GUILayout.Height(ButtonHeight()))) GoToMainMenu();
        GUILayout.FlexibleSpace();
        GUILayout.EndArea();
    }

    private void GameButton(string label, string sceneName)
    {
        if (!GUILayout.Button(label, buttonStyle, GUILayout.Height(ButtonHeight()))) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private float ButtonHeight() => Mathf.Clamp(Screen.height * .062f, 46, 68);
    private void Resume() { isPaused = false; Time.timeScale = 1f; }
    private void Restart() { Time.timeScale = 1f; isPaused = false; SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    private void GoToMainMenu() { Time.timeScale = 1f; isPaused = false; SceneManager.LoadScene(MainMenuScene); }

    private static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private static void DrawSolid(Rect rect, Color color)
    {
        Color previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = previous;
    }

    private static void DrawCoverTexture(Texture texture)
    {
        float screenAspect = (float)Screen.width / Screen.height;
        float textureAspect = (float)texture.width / texture.height;
        Rect source = new Rect(0, 0, 1, 1);
        if (screenAspect > textureAspect)
        {
            float visibleHeight = textureAspect / screenAspect;
            source.y = (1f - visibleHeight) * .5f;
            source.height = visibleHeight;
        }
        else
        {
            float visibleWidth = screenAspect / textureAspect;
            source.x = (1f - visibleWidth) * .5f;
            source.width = visibleWidth;
        }
        GUI.DrawTextureWithTexCoords(new Rect(0, 0, Screen.width, Screen.height), texture, source);
    }

    private void EnsureStyles()
    {
        if (brandStyle != null && styledHeight == Screen.height) return;
        styledHeight = Screen.height;
        if (buttonNormal == null)
        {
            buttonNormal = MakeTexture(new Color(.04f, .025f, .13f, .7f));
            buttonHover = MakeTexture(new Color(1f, .08f, .5f, .86f));
            buttonActive = MakeTexture(new Color(.32f, .12f, .66f, .95f));
        }
        brandStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerLeft, fontSize = Mathf.Clamp(Screen.height / 13, 42, 84), fontStyle = FontStyle.Bold, normal = { textColor = Color.white }, margin = new RectOffset(0, 0, 0, -14) };
        accentStyle = new GUIStyle(brandStyle) { fontSize = Mathf.Clamp(Screen.height / 7, 74, 144), normal = { textColor = new Color(1f, .16f, .54f) } };
        subtitleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = Mathf.Clamp(Screen.height / 50, 14, 22), fontStyle = FontStyle.Bold, normal = { textColor = new Color(.52f, .88f, 1f) } };
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft, fontSize = Mathf.Clamp(Screen.height / 37, 17, 29), fontStyle = FontStyle.Bold,
            padding = new RectOffset(22, 14, 0, 0), margin = new RectOffset(0, 0, 3, 3),
            normal = { background = buttonNormal, textColor = Color.white }, hover = { background = buttonHover, textColor = Color.white },
            active = { background = buttonActive, textColor = Color.white }, focused = { background = buttonHover, textColor = Color.white }
        };
        footerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontSize = Mathf.Clamp(Screen.height / 65, 11, 17), normal = { textColor = new Color(1f, 1f, 1f, .72f) } };
    }

    private static Texture2D MakeTexture(Color color)
    {
        var texture = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
