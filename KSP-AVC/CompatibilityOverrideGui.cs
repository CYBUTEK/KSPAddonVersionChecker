using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace KSP_AVC
{
    class CompatibilityOverrideGui : MonoBehaviour
    {
        #region Fields

        private readonly VersionInfo version = Assembly.GetExecutingAssembly().GetName().Version;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private bool hasCentred;
        private bool ShowAdvancedSettings = Configuration.OverrideIsDisabledGlobal;
        private GUIStyle buttonStyle;
        private GUIStyle scrollList;
        private GUIStyle topLevelTitleStyle;
        private GUIStyle centeredTitelStyle;
        private GUIStyle buttonStyleRed;
        private GUIStyle labelStyle;
        private GUIStyle labelStyleBold;
        private GUIStyle labelStyleYellow;
        private GUIStyle labelStyleCyan;
        private GUIStyle buttonStyleGreen;
        private Vector2 scrollPositionVersionInfo = Vector2.zero;
        private Vector2 scrollPositionAddonList = Vector2.zero;
        private Vector2 scrollPositionNameList = Vector2.zero;

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
            Logger.Log("Awake CompatibilityOverrideGui.");
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
            Logger.Log("Destroy ForceCompatibilityGui.");
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

        #region Methods : private
        
        private void DrawDisabled()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("DISABLED", this.centeredTitelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #endregion

        #region WindowOverrideVersionInfo

        //Version override box
        private void DrawOverrideVersionInfo()
        {            
            GUILayout.BeginVertical();
            DrawHeadingsOverrideVersion();
            scrollPositionVersionInfo = GUILayout.BeginScrollView(scrollPositionVersionInfo, this.scrollList, GUILayout.Width(230), GUILayout.Height(275));
            DrawVersionList();
            GUILayout.EndScrollView();
            DrawInputOverrideVersion();
            GUILayout.EndVertical();
        }

        //Heading for the version override box
        private void DrawHeadingsOverrideVersion()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("VERSION OVERRIDE", this.topLevelTitleStyle, GUILayout.MinWidth(200));
            GUILayout.EndHorizontal();
        }

        //List of active version overrides
        private void DrawVersionList()
        {
            if (Configuration.OverrideIsDisabledGlobal)
            {
                DrawDisabled();
                return;
            }
            var listKeys = Configuration.CompatibleVersions.Keys.ToList();
            foreach (var key in listKeys)
            {
                for (int i = 0; i < Configuration.CompatibleVersions[key].compatibleWithVersion.Count; i++)
                {                    
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Configuration.CompatibleVersions[key].currentVersion.Replace("-1", "*") + " \u279C " + Configuration.CompatibleVersions[key].compatibleWithVersion[i], this.labelStyle);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("X", this.buttonStyleRed, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        GuiHelper.UpdateCompatibilityState(OverrideType.version, null, Configuration.CompatibleVersions[key].currentVersion + "," + Configuration.CompatibleVersions[key].compatibleWithVersion[i], true);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            Configuration.DeleteFinally();
        }

        //Input textfield and "ADD" button
        private void DrawInputOverrideVersion()
        {
            GUILayout.BeginHorizontal();
            GuiHelper.userInput = GUILayout.TextField(GuiHelper.userInput, GUILayout.Width(150.0f), GUILayout.Height(20));
            if (GUILayout.Button("ADD", this.buttonStyle, GUILayout.Width(75), GUILayout.Height(20)))
            {
                GuiHelper.UpdateCompatibilityState(OverrideType.version, null, GuiHelper.userInput);
                Logger.Log($"AVC Compatibility Override, Version input: {GuiHelper.userInput}");
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        #region WindowOverrideAddonList

        private void DrawOverrideAddonList()
        {
            GUILayout.BeginVertical();
            DrawHeadingsAddonList();
            scrollPositionAddonList = GUILayout.BeginScrollView(scrollPositionAddonList, this.scrollList, GUILayout.MinWidth(430), GUILayout.Height(300));
            DrawIncompatibleMods();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawHeadingsAddonList()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(63);
            GUILayout.Label("INCOMPATIBLE MODS", this.topLevelTitleStyle, GUILayout.MinWidth(230.0f));
            GUILayout.Space(10);
            GUILayout.Label("FOR KSP", this.topLevelTitleStyle, GUILayout.MinWidth(60));
            GUILayout.EndHorizontal();
        }

        private void DrawIncompatibleMods()
        {
            if (Configuration.OverrideIsDisabledGlobal)
            {
                DrawDisabled();
                return;
            }
            List<Addon> listIncompatibleMods = AddonLibrary.Addons.Where(a => !a.IsCompatible).ToList();
            int m = listIncompatibleMods.Count();
            for (int i = 0; i < m; i++)
            {
                Addon addon = listIncompatibleMods[i];
                GUIStyle coloredLabel = (GuiHelper.CompatibilityState(OverrideType.version, addon) || GuiHelper.CompatibilityState(OverrideType.name, addon)) ? this.labelStyleCyan : this.labelStyleYellow;
                VersionInfo versionNumber = addon.LocalInfo.KspVersionMaxIsNull && addon.LocalInfo.KspVersionMinIsNull ? addon.LocalInfo.KspVersion : addon.LocalInfo.KspVersionMax;

                GUILayout.BeginHorizontal();
                DrawButtonArrowLeft(addon);
                GUILayout.Space(18);
                GUILayout.Label(addon.Name, coloredLabel, GUILayout.MinWidth(230.0f));
                GUILayout.Space(10);
                GUILayout.Label($"{versionNumber}", coloredLabel, GUILayout.MinWidth(65));
                GUILayout.Space(18);
                DrawButtonArrowRight(addon);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        private void DrawButtonArrowRight(Addon addon)
        {
            if (GuiHelper.CompatibilityState(OverrideType.locked, addon) || GuiHelper.CompatibilityState(OverrideType.name, addon))
            {
                GUILayout.Space(25); //fill the space which would usually taken by the button at this position
                return;
            }

            GUIStyle coloredButtonStyle = GuiHelper.CompatibilityState(OverrideType.version, addon) ? buttonStyleGreen : buttonStyle;
            string buttonLabel = "\u25B6"; //unicode for a triangle, pointing to the right
            if (GUILayout.Button(buttonLabel, coloredButtonStyle, GUILayout.Width(25), GUILayout.Height(25)))
            {
                if (!GuiHelper.CompatibilityState(OverrideType.version, addon))
                {
                    GuiHelper.UpdateCompatibilityState(OverrideType.version, null, addon.LocalInfo.KspVersionMaxIsNull && addon.LocalInfo.KspVersionMinIsNull ? addon.LocalInfo.KspVersion.ToString() : addon.LocalInfo.KspVersionMax.ToString()); //addon.LocalInfo.KspVersionMax == AddonInfo.ActualKspVersion ? addon.LocalInfo.KspVersion.ToString() : addon.LocalInfo.KspVersionMax.ToString()
                }
            }            
        }

        private void DrawButtonArrowLeft(Addon addon)
        {
            if(GuiHelper.CompatibilityState(OverrideType.locked, addon) || (GuiHelper.CompatibilityState(OverrideType.version, addon) && !GuiHelper.CompatibilityState(OverrideType.ignore, addon)))
            {
                GUILayout.Space(33); //fill the space which would usually taken by the button at this position
                return;
            }

            GUIStyle coloredButtonStyle = GuiHelper.CompatibilityState(OverrideType.name, addon) ? buttonStyleGreen : buttonStyle;
            string buttonLabel = "\u25C0"; //unicode for a triangle, pointing to the left
            if (GUILayout.Button(buttonLabel, coloredButtonStyle, GUILayout.Width(25), GUILayout.Height(25)))
            {
                if (!GuiHelper.CompatibilityState(OverrideType.name, addon))
                {
                    GuiHelper.UpdateCompatibilityState(OverrideType.name, addon); 
                }
            }
        }

        #endregion

        #region WindowOverrideNameList

        private void DrawOverrideNameList()
        {
            GUILayout.BeginVertical();
            DrawHeadingsOverrideInfo();
            scrollPositionNameList = GUILayout.BeginScrollView(scrollPositionNameList, this.scrollList, GUILayout.Width(230), GUILayout.Height(300));
            DrawCompatibleByName();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawHeadingsOverrideInfo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ALWAYS COMPATIBLE", this.topLevelTitleStyle, GUILayout.MinWidth(200));
            GUILayout.EndHorizontal();
        }

        private void DrawCompatibleByName()
        {
            if (Configuration.OverrideIsDisabledGlobal)
            {
                DrawDisabled();
                return;
            }
            List<Addon> listCompatibleByName = AddonLibrary.Addons.Where(a => a.IsForcedCompatibleByName).ToList();
            int m = listCompatibleByName.Count();
            for (int i = 0; i < m; i++)
            {
                var addon = listCompatibleByName[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, this.labelStyle, GUILayout.MinWidth(190.0f));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", this.buttonStyleRed, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    GuiHelper.UpdateCompatibilityState(OverrideType.name, addon);
                }
                GUILayout.EndHorizontal();
            }
        }

        #endregion

        #region WindowAdvancedSettings

        private void DrawCompatibilityOverrideGui2()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            DrawOverrideNameList();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        #endregion

        #region MainWindow

        private void DrawCompatibilityOverrideGui()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            DrawOverrideNameList();
            GUILayout.Space(10);
            DrawOverrideAddonList();
            GUILayout.Space(13);
            DrawOverrideVersionInfo();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void Window(int id)
        {
            if (AddonLibrary.Addons.Any(a => !a.IsCompatible))
            {
                this.DrawCompatibilityOverrideGui();
            }
            DrawBottomButtons();
            GUI.DragWindow();
        }

        private void DrawBottomButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ADVANCED SETTINGS", this.buttonStyle, GUILayout.Width(180)))
            {
                if (!this.GetComponent<AdvancedOverrideSettingsGui>())
                {
                    this.gameObject.AddComponent<AdvancedOverrideSettingsGui>();
                    return;
                }
                else
                {
                    Destroy(this.GetComponent<AdvancedOverrideSettingsGui>());
                }
            }
            GUILayout.FlexibleSpace();
            string buttonLabel = "DISABLE OVERRIDE";
            if (Configuration.OverrideIsDisabledGlobal)
            {
                buttonLabel = "ENABLE OVERRIDE";
            }
            if (GUILayout.Button(buttonLabel, this.buttonStyle, GUILayout.Width(180)))
            {
                Configuration.ToggleOverrideFeature();
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("RESET", this.buttonStyle, GUILayout.Width(180)))
            {
                foreach (Addon addon in AddonLibrary.Addons.Where(x => !x.IsCompatible))
                {
                    if (GuiHelper.CompatibilityState(OverrideType.ignore, addon))
                    {
                        Configuration.RemoveFromIgnore(addon);
                    }
                    if (GuiHelper.CompatibilityState(OverrideType.name, addon))
                    {
                        Configuration.RemoveOverrideName(addon);
                    }
                }
                if (Configuration.CompatibleVersions.Count != 0)
                {
                    Configuration.CompatibleVersions.Clear();
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("HELP", this.buttonStyle, GUILayout.Width(180)))
            {
                if (!this.GetComponent<CompatibilityOverrideHelpGui>())
                {
                    this.gameObject.AddComponent<CompatibilityOverrideHelpGui>();
                    return;
                }
                else
                {
                    Destroy(this.GetComponent<CompatibilityOverrideHelpGui>());
                }
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("CLOSE", this.buttonStyle, GUILayout.Width(180)))
            {
                Configuration.SaveCfg();
                Destroy(this);
            }
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

        #endregion

        #region Styles

        private void InitialiseStyles()
        {
            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold,
            };

            this.buttonStyleGreen = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.green
                },
                fontStyle = FontStyle.Bold,
            };

            this.scrollList = new GUIStyle(HighLogic.Skin.box);

            this.topLevelTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };

            this.centeredTitelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };

            this.buttonStyleRed = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.red
                },
                fontStyle = FontStyle.Bold,
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                alignment = TextAnchor.MiddleLeft
            };

            this.labelStyleBold = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
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
        }
        #endregion
    }
}
