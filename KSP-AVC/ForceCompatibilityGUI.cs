#region Using Directives

using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class ForceCompatibilityGui : MonoBehaviour
    {
        #region Fields

        private readonly VersionInfo version = Assembly.GetExecutingAssembly().GetName().Version;
        private GUIStyle buttonStyle;
        private GUIStyle boxStyle;
        private GUIStyle nameTitleStyle;
        private GUIStyle enableTitleStyle;
        private GUIStyle messageStyle;
        private GUIStyle toggleStyle;
        private bool hasCentred;
        //private string message;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        //private string title;
        //private GUIStyle titleStyle;
        private GUIStyle titleStyle;
        //private GUILayout toggleButton;

        #endregion

        #region Properties

        public bool HasBeenUpdated { get; set; }

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
            Logger.Log("Awake ForceCompatibilityGui.");
        }

        protected void OnDestroy()
        {
            //Save override settings per mod name
            OverrideSettings.Instance.Save();
            Logger.Log("OnDestroy ForceCompatibilityGui.");
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker - Override Compatibility", HighLogic.Skin.window);
                this.CentreWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Start()
        {

            //Load override settings
            OverrideSettings.Instance.Load();

            try
            {
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Update()
        {
            
        }

        #endregion

        #region Methods: private

        private void DrawCompatibilityOverride()
        {
            GUILayout.BeginVertical(this.boxStyle);
            this.DrawHeadings();

            foreach (var addon in AddonLibrary.Addons.Where(a => !a.IsCompatible))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, this.messageStyle, GUILayout.MinWidth(575.0f));
                GUILayout.FlexibleSpace();
                //Toggle button needs to be here
                addon.IsForcedCompatibleByName = GUILayout.Toggle(addon.IsForcedCompatibleByName, string.Empty,toggleStyle);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        //private void

        private void DrawHeadings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("INCOMPATIBLE MOD", this.nameTitleStyle);
            GUILayout.Label("OVERRIDE", this.enableTitleStyle);
            GUILayout.EndHorizontal();
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

        private void InitialiseStyles()
        {
            this.boxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            this.nameTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.enableTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleRight,
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                }
            };

            this.toggleStyle = new GUIStyle(HighLogic.Skin.toggle)
            {
                //padding =
                //{
                //    top = 10,
                //    bottom = 6
                //},
                //wordWrap = false,
                //fontStyle = FontStyle.Bold,
                //margin = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleRight,
            };

        }

        private void Window(int id)
        {
            if (AddonLibrary.Addons.Any(a => !a.IsCompatible))
            {
                this.DrawCompatibilityOverride();
            }
            if (GUILayout.Button("CLOSE", this.buttonStyle))
            {
                OverrideSettings.Instance.Save();
                Destroy(this);
            }
            GUI.DragWindow();
        }

        #endregion
    }
}