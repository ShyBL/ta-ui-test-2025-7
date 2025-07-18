using UnityEngine;
using UnityEditor;
using System;

public class WelcomeWindow : EditorWindow
{
    private const string WINDOW_TITLE = "Welcome to my Assignemnt";
    private const string AUTHOR_NAME = "Blechman Shy";
    private const string GITHUB_URL = "https://github.com/ShyBL";
    private const string LINKEDIN_URL = "https://www.linkedin.com/in/shy-blechman/e";

    private const string FREE_TEXT = @"Welcome to this Unity project! 

    This project contains- 
    • Custom animated button script and animated energy bar 
    • Particle effect to indicate Auto Spin, and other vfx 
    • Rigged Character with idle animation
    • Editor tools used as part of the workflow
    • Demo script to showcase capabilities of all components

    If you have any questions don't hesitate to reach through my email or linkedin, links below.";

    private Vector2 scrollPosition;
    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle linkStyle;
    private GUIStyle footerStyle;
    private bool stylesInitialized = false;

    [InitializeOnLoadMethod]
    static void InitializeOnLoad()
    {
        EditorApplication.delayCall += ShowWelcomeWindow;
    }

    static void ShowWelcomeWindow()
    {
        // Only show on first load or when specifically requested
        if (SessionState.GetBool("WelcomeWindowShown", false) == false)
        {
            GetWindow<WelcomeWindow>(true, "Welcome", true);
            SessionState.SetBool("WelcomeWindowShown", true);
        }
    }

    [MenuItem("Window/Welcome Screen")]
    static void ShowWindow()
    {
        GetWindow<WelcomeWindow>(false, "Welcome");
    }

    private void OnEnable()
    {
        this.minSize = new Vector2(500, 400);
        this.maxSize = new Vector2(600, 500);
    }

    private void InitializeStyles()
    {
        if (stylesInitialized) return;
        titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14, alignment = TextAnchor.UpperCenter, normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
        };
        subtitleStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 12, alignment = TextAnchor.UpperCenter, fontStyle = FontStyle.Italic, normal = { textColor = EditorGUIUtility.isProSkin ? Color.gray : Color.gray }
        };
        bodyStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
        {
            fontSize = 12, alignment = TextAnchor.UpperLeft, wordWrap = true, normal = { textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black }
        };
        linkStyle = new GUIStyle(EditorStyles.linkLabel)
        {
            fontSize = 12, alignment = TextAnchor.MiddleCenter, normal = { textColor = new Color(0.4f, 0.6f, 1f) }
        };
        footerStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 10, alignment = TextAnchor.UpperCenter, normal = { textColor = EditorGUIUtility.isProSkin ? Color.gray : Color.gray }
        };
        stylesInitialized = true;
    }

    private void OnGUI()
    {
        InitializeStyles();
        using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollView.scrollPosition;
            GUILayout.Space(20);

            // Title
            EditorGUILayout.LabelField(WINDOW_TITLE, titleStyle);
            GUILayout.Space(10);

            // Subtitle with name and date
            string subtitle = $"Created by {AUTHOR_NAME} • {DateTime.Now:MMMM yyyy}";
            EditorGUILayout.LabelField(subtitle, subtitleStyle);
            GUILayout.Space(20);

            // Separator line
            DrawSeparator();
            GUILayout.Space(15);

            // Free text content
            EditorGUILayout.LabelField(FREE_TEXT, bodyStyle);
            GUILayout.Space(20);

            // Another separator
            DrawSeparator();
            GUILayout.Space(15);

            // Footer with links
            EditorGUILayout.LabelField("Connect with me:", footerStyle);
            GUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                // GitHub Link
                if (GUILayout.Button("GitHub", linkStyle, GUILayout.Width(80)))
                {
                    Application.OpenURL(GITHUB_URL);
                }

                GUILayout.Space(20);

                // LinkedIn Link
                if (GUILayout.Button("LinkedIn", linkStyle, GUILayout.Width(80)))
                {
                    Application.OpenURL(LINKEDIN_URL);
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10);

            // Close button
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", GUILayout.Width(80), GUILayout.Height(25)))
                {
                    this.Close();
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(10);
        }
    }

    private void DrawSeparator()
    {
        var rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, EditorGUIUtility.isProSkin ? Color.gray : Color.gray);
    }

    private void OnDestroy()
    {
        // Reset the session state when window is closed so it can be shown again next time
        SessionState.SetBool("WelcomeWindowShown", false);
    }
}