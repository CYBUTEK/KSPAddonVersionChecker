using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_AVC
{
    class CompatibilityOverrideAdvSettingsGui : MonoBehaviour
    {
        #region Fields

        //private enum OverrideType { name, version, ignore, locked };
        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle labelStyleWhite;
        private GUIStyle labelStyleYellow;
        private GUIStyle labelStyleCyan;
        private GUIStyle toggleStyle;
        private GUIStyle scrollList;
        private GUIStyle topLevelTitleStyle;
        private bool hasCentred;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private Vector2 scrollPosition = Vector2.zero;

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
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker - Advanced Settings", HighLogic.Skin.window);
                this.CentreWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods : Private

        private void Window(int id)
        {
            this.DrawIgnoreOverride();
            if (GUILayout.Button("CLOSE", this.buttonStyle))
            {
                Configuration.SaveCfg();
                Destroy(this);
            }
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

        private void DrawIgnoreOverride()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(this.boxStyle);
            DrawAdvancedInfo();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DrawIgnoreOverrideSettings();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DrawDefaultValueSettings();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawDefaultValueSettings()
        {
            bool toggleState = Configuration.ShowDefaultValues;
            GUILayout.BeginVertical();
            DrawHeadingsDefaultValue();
            GUILayout.BeginHorizontal(this.scrollList);
            GUILayout.Label("Show preset values in addon list", this.labelStyleWhite);
            GUILayout.FlexibleSpace();
            toggleState = GUILayout.Toggle(toggleState, "", this.toggleStyle);
            if(toggleState != Configuration.ShowDefaultValues)
            {
                Configuration.ShowDefaultValues = !Configuration.ShowDefaultValues;
            }
            GUILayout.Space(25);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawHeadingsDefaultValue()
        {
            GUILayout.Label("OTHER SETTINGS", this.topLevelTitleStyle);
        }

        private void DrawAdvancedInfo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("This is the ignore list for the compatibile version override." +
                "\nAny mod on the ignore list will no longer be affected by the version range. " +
                "It is still possible to put an ignored mod, on the \"ALWAYS OVERRIDE\" list.", this.labelStyle);
            GUILayout.EndHorizontal();
        }

        private void DrawIgnoreOverrideSettings()
        {
            GUILayout.BeginVertical();
            DrawHeadingsIgnoreInfo();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, this.scrollList, GUILayout.Width(430), GUILayout.Height(180));
            DrawIgnoreList();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawHeadingsIgnoreInfo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ACTIVE VERSION OVERRIDE", this.topLevelTitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.Label("IGNORE", this.topLevelTitleStyle);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
        }

        private void DrawIgnoreList()
        {
            List<Addon> overrideMods = AddonLibrary.Addons.Where(x => x.IsForcedCompatibleByVersion || GuiHelper.CompatibilityState(OverrideType.ignore, x)).OrderBy(x => x.Name).ToList();
            int m = overrideMods.Count;
            for(int i = 0; i < m; i++)
            {
                var addon = overrideMods[i];
                bool toggleState = GuiHelper.CompatibilityState(OverrideType.ignore, addon);
                GUIStyle labelStyleIgnore = GuiHelper.CompatibilityState(OverrideType.ignore, addon) ? this.labelStyleYellow : this.labelStyleCyan;

                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, labelStyleIgnore);
                GUILayout.FlexibleSpace();

                toggleState = GUILayout.Toggle(toggleState, "", this.toggleStyle);
                if(toggleState != GuiHelper.CompatibilityState(OverrideType.ignore, addon))
                {
                    GuiHelper.UpdateCompatibilityState(OverrideType.ignore, addon);
                }
                GUILayout.Space(25);
                GUILayout.EndHorizontal();
            }
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

            this.labelStyleWhite = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };

            this.labelStyleYellow = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.yellow
                },
                alignment = TextAnchor.MiddleLeft,
            };

            this.labelStyleCyan = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.cyan
                },
                alignment = TextAnchor.MiddleLeft,
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

            this.scrollList = new GUIStyle(HighLogic.Skin.box);

            this.toggleStyle = new GUIStyle(HighLogic.Skin.toggle);
        }

        #endregion
    }
}
