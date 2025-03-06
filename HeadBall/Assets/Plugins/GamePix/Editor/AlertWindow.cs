using UnityEditor;
using UnityEngine;

namespace GamePix.Editor
{
    public class AlertWindow : EditorWindow
    {
        private static AlertParameters parameters;
        private GUIStyle bottomGroupStyle;
        private GUIStyle messageStyle;
        private GUIStyle commentStyle;
        private GUIStyle buttonStyle;
        private static readonly Vector2 windowSize = new Vector2(300, 200);
        private static readonly int commentMaxLength = 200;
        
        public static void Show(AlertParameters alertParameters)
        {
            parameters = alertParameters;
            if (parameters.Comment.Length > commentMaxLength)
            {
                parameters.Comment = parameters.Comment.Substring(0, commentMaxLength) + "...";
            }
            var wnd = GetWindow(typeof(AlertWindow), true, parameters.Title, true);
            wnd.minSize = wnd.maxSize = windowSize;
        }
        
        void OnEnable ()
        {
            if (bottomGroupStyle == null)
            {
                bottomGroupStyle = new GUIStyle();
                bottomGroupStyle.alignment = TextAnchor.LowerLeft;
                
                messageStyle = new GUIStyle(EditorStyles.label);
                messageStyle.fontSize = 16;
                messageStyle.wordWrap = true;
                messageStyle.fontStyle = FontStyle.Bold;
                messageStyle.alignment = TextAnchor.MiddleCenter;
                
                commentStyle = new GUIStyle(EditorStyles.label);
                commentStyle.normal.textColor = Color.red;
                commentStyle.fontSize = 11;
                commentStyle.wordWrap = true;
                
                buttonStyle = new GUIStyle(EditorStyles.miniButton);
                buttonStyle.fontSize = 16;
                buttonStyle.fixedHeight = 40;
            }
        }

        void OnGUI()
        {
            GUILayout.Space(25f);
            GUILayout.Label(parameters.Message, messageStyle);
            GUILayout.Space(10f);
            GUILayout.Label(parameters.Comment, commentStyle);
            GUILayout.FlexibleSpace();
            using var h = new EditorGUILayout.VerticalScope(bottomGroupStyle);
            if (GUILayout.Button(parameters.Button, buttonStyle))
            {
                parameters.Action?.Invoke();
                Close();
            }

            GUILayout.Space(10);
        }
    }
}