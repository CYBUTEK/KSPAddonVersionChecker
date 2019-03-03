using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace KSP_AVC
{
    public class CompatibilityOverrideGui : MonoBehaviour
    {
        #region Fields

        private readonly VersionInfo version = Assembly.GetExecutingAssembly().GetName().Version;
        private enum OverrideType { name, version, ignore, locked };
        private GUIStyle buttonStyle;
        private GUIStyle boxStyle;
        private GUIStyle topLevelTitleStyle;
        private GUIStyle topLevelTitleStyleVersion;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle toggleStyle;
        private GUIStyle textFieldStyle;
        private GUIStyle scrollList;
        private bool hasCentred;
        private bool toggleState;
        private string userInput;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private Vector2 scrollPositionInfo = Vector2.zero;
        private Vector2 scrollPositionVersion = Vector2.zero;
        private Vector2 scrollPositionIgnored = Vector2.zero;
        private Vector2 scrollPositionLocked = Vector2.zero;

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
            Logger.Log("ADDON LIBRARY");
            foreach (var item in AddonLibrary.Addons.Where(a => a.IsCompatible))
            {
                Logger.Log($"{item.Name} is locked by creator: {item.IsLockedByCreator}");
            }
        }

        protected void OnDestroy()
        {
            if(Configuration.CfgUpdated)
            {
                Configuration.SaveCfg();
            }            
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
            //Configuration.AddOverrideVersion("1.4.2", "1.4.8");
            //Configuration.AddOverrideVersion("1.4.4", "1.6.1");
            //Configuration.AddOverrideVersion("1.4.4", "1.5.1");
            //Configuration.RemoveOverrideVersion("1.4.3", "1.6.1");
            //Configuration.RemoveOverrideVersion("1.4.1", "1.5.1");
        }

        #endregion

        #region Methods : Private

        private void Window(int id)
        {
            if (AddonLibrary.Addons.Any(a => !a.IsCompatible))
            {
                this.DrawCompatibilityOverrideGui();
            }
            if (GUILayout.Button("CLOSE", this.buttonStyle))
            {
                //Configuration.SaveCfg();
                Destroy(this);
            }
            GUI.DragWindow();
        }

        //MainWindow
        private void DrawCompatibilityOverrideGui()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            DrawOverrideInfo();
            DrawOverrideVersion();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DrawOverrideIgnore();
            DrawOverrideLocked();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawOverrideLocked()
        {
            GUILayout.BeginVertical();
            DrawHeadingsOverrideLocked();
            scrollPositionLocked = GUILayout.BeginScrollView(scrollPositionLocked, this.scrollList, GUILayout.Width(430), GUILayout.Height(180));
            DrawListLockedMods();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawListLockedMods()
        {
            List<Addon> listLockedMods = AddonLibrary.Addons.Where(a => a.IsLockedByCreator).ToList();
            int m = listLockedMods.Count();
            for (int i = 0; i < m; i++)
            {
                var addon = listLockedMods[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, this.labelStyle, GUILayout.MinWidth(230.0f)); //Mod name
                GUILayout.EndHorizontal();
            }
        }

        private void DrawHeadingsOverrideLocked()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("LOCKED BY VERSIONFILE", this.topLevelTitleStyle, GUILayout.MinWidth(200));
            GUILayout.EndHorizontal();
        }

        private void DrawOverrideIgnore()
        {
            GUILayout.BeginVertical();
            DrawHeadingsOverrideIgnore();
            scrollPositionIgnored = GUILayout.BeginScrollView(scrollPositionIgnored, this.scrollList, GUILayout.Width(430), GUILayout.Height(180));
            DrawListIgnoredMods();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawListIgnoredMods()
        {
            List<Addon> listIgnoredMods = AddonLibrary.Addons.Where(a => Configuration.modsIgnoreOverride.Contains(a.Name)).ToList();
            int m = listIgnoredMods.Count();
            for (int i = 0; i < m; i++)
            {
                var addon = listIgnoredMods[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, this.labelStyle, GUILayout.MinWidth(230.0f)); //Mod name
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("REMOVE", this.buttonStyle))
                {
                    UpdateCompatibilityState(OverrideType.ignore, addon);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DrawHeadingsOverrideIgnore()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("IGNORE VERSION OVERRIDE", this.topLevelTitleStyle, GUILayout.MinWidth(200));
            GUILayout.EndHorizontal();
        }

        private void DrawOverrideVersion()
        {
            GUILayout.BeginVertical();
            DrawHeadingsOverrideVersion();
            scrollPositionVersion = GUILayout.BeginScrollView(scrollPositionVersion, this.scrollList, GUILayout.Width(200), GUILayout.Height(275));
            DrawVersionList();
            GUILayout.EndScrollView();
            DrawInputOverrideVersion(); //ADD button and text field
            GUILayout.EndVertical();
        }

        private void DrawVersionList()
        {
            var listKeys = Configuration.CompatibleVersions.Keys.ToList();
            foreach(var key in listKeys)
            {
                for (int i = 0; i < Configuration.CompatibleVersions[key].compatibleWithVersion.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Configuration.CompatibleVersions[key].currentVersion + " ==> " + Configuration.CompatibleVersions[key].compatibleWithVersion[i], this.labelStyle);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("X", this.buttonStyle, GUILayout.Width(25), GUILayout.Height(25)))
                    {
                        UpdateCompatibilityState(OverrideType.version, null, Configuration.CompatibleVersions[key].currentVersion + "," + Configuration.CompatibleVersions[key].compatibleWithVersion[i], true);
                    }
                    GUILayout.EndHorizontal();
                }                    
            }
            Configuration.deleteFinally();
        }

        private void DrawInputOverrideVersion()
        {
            GUILayout.BeginHorizontal();
            userInput = GUILayout.TextField(userInput, GUILayout.Width(110.0f), GUILayout.Height(20));
            GUILayout.FlexibleSpace();            
            if (GUILayout.Button("ADD", this.buttonStyle, GUILayout.Width(75), GUILayout.Height(20)))
            {
                UpdateCompatibilityState(OverrideType.version, null, userInput);
                Logger.Log($"INPUT: {userInput}");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawHeadingsOverrideVersion()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("VERSION OVERRIDE", this.topLevelTitleStyleVersion, GUILayout.MinWidth(200));
            GUILayout.EndHorizontal();
        }

        private void DrawOverrideInfo()
        {
            GUILayout.BeginVertical();
            DrawHeadingsOverrideInfo();
            scrollPositionInfo = GUILayout.BeginScrollView(scrollPositionInfo, this.scrollList, GUILayout.Width(650), GUILayout.Height(300));
            
            DrawIncompatibleMods();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void DrawIncompatibleMods()
        {
            List<Addon> listIncompatibleMods = AddonLibrary.Addons.Where(a => !a.IsCompatible).ToList();
            int m = listIncompatibleMods.Count();
            for (int i = 0; i < m; i++)
            {
                var addon = listIncompatibleMods[i];
                if(addon.IsLockedByCreator)
                {
                    continue;
                }
                string range = (addon.LocalInfo.KspVersionMin != null ? addon.LocalInfo.KspVersionMin.ToString() : "NULL") + " to " + (addon.LocalInfo.KspVersionMax != null ? addon.LocalInfo.KspVersionMax.ToString() : "NULL");
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, this.labelStyle, GUILayout.MinWidth(230.0f)); //Mod name
                GUILayout.Space(10);
                GUILayout.Label(range, labelStyle, GUILayout.MinWidth(135.0f)); //Compatible for
                GUILayout.Space(10);
                if (addon.IsForcedCompatibleByVersion) //Override
                    GUILayout.Label("Version", labelStyle, GUILayout.MinWidth(90.0f));
                else if (addon.IsForcedCompatibleByName)
                    GUILayout.Label("Name", labelStyle, GUILayout.MinWidth(90.0f));
                else
                    GUILayout.Label("None", labelStyle, GUILayout.MinWidth(90.0f));
                GUILayout.Space(30);
                DrawToggleButtonName(addon);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("X", this.buttonStyle, GUILayout.Width(25), GUILayout.Height(25)))
                {
                    UpdateCompatibilityState(OverrideType.ignore, addon);
                }
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
            }
        }

        private void DrawHeadingsOverrideInfo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("INCOMPATIBLE MODS", this.topLevelTitleStyle, GUILayout.MinWidth(230.0f));
            GUILayout.Space(10);
            GUILayout.Label("COMPATIBLE FOR", this.topLevelTitleStyle, GUILayout.MinWidth(135.0f));
            GUILayout.Space(10);
            GUILayout.Label("OVERRIDE BY", this.topLevelTitleStyle, GUILayout.MinWidth(90.0f));
            GUILayout.Space(20);
            GUILayout.Label("ENABLED", this.topLevelTitleStyle, GUILayout.MinWidth(80.0f));
            GUILayout.Space(10);
            GUILayout.Label("IGNORE", this.topLevelTitleStyle, GUILayout.MinWidth(60.0f));
            GUILayout.EndHorizontal();
        }

        private void DrawToggleButtonName(Addon addon)
        {
            if(addon.IsForcedCompatibleByVersion)
            {
                GUILayout.Toggle(true, "", toggleStyle);
                return;
            }

            toggleState = CompatibilityState(OverrideType.name, addon);
            toggleState = GUILayout.Toggle(toggleState, "", toggleStyle);
            if (toggleState != CompatibilityState(OverrideType.name, addon))
            {
                UpdateCompatibilityState(OverrideType.name, addon);
            }
        }

        private bool CompatibilityState(OverrideType type, Addon addon)
        {
            switch(type)
            {
                case OverrideType.name:
                    {
                        return addon.IsForcedCompatibleByName;
                    }

                case OverrideType.version:
                    {
                        return addon.IsForcedCompatibleByVersion;
                    }

                case OverrideType.ignore:
                    {
                        return Configuration.modsIgnoreOverride.Contains(addon.Name);
                    }
                case OverrideType.locked:
                    {
                        return addon.IsLockedByCreator;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        private void UpdateCompatibilityState(OverrideType type, Addon addon = null, string versionInfo = "", bool b = false)
        {
            switch(type)
            {
                case OverrideType.name:
                    {
                        if (!Configuration.OverrideCompatibilityByName.Contains(addon.Name))
                        {
                            Configuration.AddOverrideName(addon);
                            return;
                        }
                        Configuration.RemoveOverrideName(addon);
                        return;
                    }

                case OverrideType.version:
                    {
                        if(validateInput(versionInfo))
                        {
                            Logger.Log("Valid input!");
                            List<string> inputs = reformatInput(versionInfo);
                            foreach (var item in inputs)
                            {
                                Logger.Log($"inputs: {item}");
                            }
                            var dictKeys = Configuration.CompatibleVersions.Keys;
                            if (dictKeys.Contains(inputs[0]) && b)
                            {
                                Logger.Log("Entry exists!");
                                int j = inputs.Count();
                                for (int i = 1; i < j; i++) //so far, the list will always have just two entries in this case but just to be sure, I've build the loop anyway
                                {
                                    Configuration.RemoveOverrideVersion(inputs[0], inputs[i]);
                                }
                                return;
                            }
                            Logger.Log("Entry doesn't exists!");
                            int m = inputs.Count();
                            for (int i = 1; i < m; i++)
                            {
                                Configuration.AddOverrideVersion(inputs[0], inputs[i]);
                            }
                        }
                        Logger.Log("Invalid input!");
                        return;
                    }

                case OverrideType.ignore:
                    {
                        if (!Configuration.modsIgnoreOverride.Contains(addon.Name))
                        {
                            Configuration.AddToIgnore(addon);
                            return;
                        }
                        Configuration.RemoveFromIgnore(addon);
                        return;
                    }

                default:
                    {
                        Logger.Log("Unable to update compatibility override");
                        return;
                    }
            }
        }

        private bool validateInput(string userInput)
        {
            string regexPattern = @"(\d\.\d)\,\s?(\d\.\d)"; //just a rough pattern, should filter out most invalid inputs but definitly not all of them
            bool match = Regex.IsMatch(userInput, regexPattern);

            return match;
        }

        //looks ridiculous stupid but I need a strict format ("old version","new version") for the version list
        //funny results if invalid user input slips through the validation
        private List<string> reformatInput(string userInput)
        {
            string[] splitted = userInput.Split(',').Select(x => x.Trim()).ToArray();
            List<string> reformattedStrings = new List<string>();
            int m = splitted.Length;
            for(int i = 0; i < m; i++)
            {
                reformattedStrings.Add(splitted[i]);
            }

            return reformattedStrings;
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
                fixedHeight = 500,
            };

            this.scrollList = new GUIStyle(HighLogic.Skin.box);

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
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

            this.topLevelTitleStyleVersion = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                alignment = TextAnchor.MiddleLeft
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold,
            };

            this.textFieldStyle = new GUIStyle(HighLogic.Skin.textField)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold,
            };

            this.toggleStyle = new GUIStyle(HighLogic.Skin.toggle)
            {
                
            };
        }

        #endregion
    }
}
