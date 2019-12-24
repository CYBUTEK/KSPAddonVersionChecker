using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_AVC
{
    class CompatibilityOverrideEnableGui : MonoBehaviour
    {
        #region Fields

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle labelStyle;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private GUIStyle topLevelTitleStyle;

        #endregion

        #region Methods: protected

        protected void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            this.name = "OverrideEnableGui";
        }

        protected void Start()
        {
            try
            {
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void OnDestroy()
        {
            if (Configuration.CfgUpdated)
            {
                Configuration.SaveCfg();
            }
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker - Compatibility Override", HighLogic.Skin.window);
                this.CentreWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods : Private

        private void DrawQuestion()
        {
            GUILayout.BeginHorizontal(this.boxStyle, GUILayout.Width(400));
            GUILayout.Label("The Compatibility Override is currently disabled!\nDo you want to enable it?", this.labelStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("YES", this.buttonStyle))
            {
                Configuration.OverrideIsDisabledGlobal = false;
                this.gameObject.AddComponent<CompatibilityOverrideGui>();
                Destroy(this);
            }
            if (GUILayout.Button("NO", this.buttonStyle))
            {
                Destroy(this);
            }
            GUILayout.EndHorizontal();
        }

        private void Window(int id)
        {
            DrawQuestion();
            DrawButtons();
            GUI.DragWindow();
        }

        private void CentreWindow()
        {
            if (this.hasCentred || !(this.position.width > 0) || !(this.position.height > 0))
            {
                return;
            }
            this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            this.hasCentred = true;
        }

        #endregion

        #region Styles

        private void InitialiseStyles()
        {
            this.boxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5),
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold,
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                alignment = TextAnchor.MiddleLeft
            };

            this.topLevelTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };
        }

        #endregion
    }
}
