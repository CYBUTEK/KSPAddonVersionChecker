// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using System;
using System.Linq;

using UnityEngine;

#endregion

namespace KSP_AVC.Toolbar
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class ToolbarWindow : MonoBehaviour
    {
        #region Fields

        private string addonList;
        private GUIStyle labelGreen;
        private GUIStyle labelYellow;
        private GUIStyle labelCyan;
        private Rect position = new Rect(0.0f, 0.0f, 400.0f, 0.0f);
        private Vector2 scrollPosition = new Vector2(0, 0);
        private bool showAddons;
        private bool useScrollView;
        private GUIStyle windowStyle;
        private GameObject helper;
        private GuiHelper InstanceGuiHelper;
        private string overrideAvailable = "<color=#00FF00>Compatibility Override GUI</color>";
        private string overrideNotAvailable = "<color=#FF0000>Wait for AVC processing...</color>";
        private string buttonText;

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

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, String.Empty, this.windowStyle);
                this.CheckScrollViewUsage();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        protected void Start()
        {
            if (Configuration.CfgLoaded && Configuration.AvcInterval == -1)
            {
                Destroy(this);
            }
            else if (Configuration.CfgLoaded && DateTime.Compare(DateTime.Now, Configuration.NextRun) <= 0 && Configuration.AvcInterval != 0)
            {
                Destroy(this);
            }
            try
            {
                if (AssemblyLoader.loadedAssemblies.Any(a => a.name == "DevHelper"))
                {
                    this.position.y = 60.0f;
                }
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

        }

        protected void Update()
        {
            try
            {
                if (HighLogic.LoadedScene != GameScenes.LOADING && HighLogic.LoadedScene != GameScenes.MAINMENU)
                {
                    Destroy(this);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

            if(helper == null)
            {
                helper = GameObject.Find("GuiHelper");
                buttonText = overrideNotAvailable;
                if (helper != null)
                {
                    InstanceGuiHelper = helper.GetComponent<GuiHelper>();
                    buttonText = overrideAvailable;
                }
            }
        }

        #endregion

        #region Methods: private

        private void CheckScrollViewUsage()
        {
            if (this.position.height < Screen.height * 0.5f || this.useScrollView)
            {
                return;
            }

            this.useScrollView = true;
            this.position.height = Screen.height * 0.5f;
        }

        private void OverrideGUI()
        {
            if (GUILayout.Button(buttonText))
            {
                if (InstanceGuiHelper != null)
                {
                    InstanceGuiHelper.ToggleGUI();  
                }                  
            }            
        }

        private void CopyToClipboard()
        {
            if (!GUILayout.Button("Copy to Clipboard"))
            {
                return;
            }

            var kspVersion = AddonInfo.ActualKspVersion.ToString();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (IntPtr.Size == 8)
                {
                    kspVersion += " (Win64)";
                }
                else
                {
                    kspVersion += " (Win32)";
                }
            }
            else
            {
                kspVersion += " (" + Environment.OSVersion.Platform + ")";
            }

            var copyText = "KSP: " + kspVersion +
                           " - Unity: " + Application.unityVersion +
                           " - OS: " + SystemInfo.operatingSystem +
                           this.addonList;

            var textEditor = new TextEditor
            {
               text = copyText
            };

            textEditor.SelectAll();
            textEditor.Copy();
        }

        private void DrawAddonBoxEnd()
        {
            if (this.useScrollView)
            {
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.EndVertical();
            }
        }
        bool err = false;
        private void DrawAddonBoxStart()
        {
            if (this.useScrollView)
            {
                try
                {

                    this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, GUILayout.Height(Screen.height * 0.5f));

                }
                catch  (Exception ex )
                {
                    // Strange, the above call generates a single exception, not the first time called.  But after that
                    // it's ok, so this will just bypass the error

                    err = true;
                    return;
                }
            }
            else
            {
                GUILayout.BeginVertical(GUI.skin.scrollView);
            }
        }

        private void DrawAddonList()
        {
            this.DrawAddonBoxStart();
            if (err)
            {
                err = false;
                return;
            }
            this.DrawAddons();
            this.DrawAddonBoxEnd();

            GUILayout.Space(5.0f);
            this.CopyToClipboard();
            this.OverrideGUI();
            GUILayout.Space(5.0f);
        }

        private void DrawAddons()
        {
            this.addonList = String.Empty;
            foreach (var addon in AddonLibrary.Addons)
            {
                var labelStyle = (addon.IsForcedCompatibleByVersion || addon.IsForcedCompatibleByName) && !addon.IsUpdateAvailable ? this.labelCyan : (!addon.IsCompatible || addon.IsUpdateAvailable) ? this.labelYellow : this.labelGreen;
                this.addonList += Environment.NewLine + addon.Name + " - " + addon.LocalInfo.Version;
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, labelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(addon.LocalInfo.Version != null ? addon.LocalInfo.Version.ToString() : String.Empty, labelStyle);
                GUILayout.EndHorizontal();
                
            }
        }

        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle
            {
                normal =
                {
                    background = Utils.GetTexture("OverlayBackground.png", 400, 100)
                },
                border = new RectOffset(3, 3, 20, 3),
                padding = new RectOffset(10, 10, 23, 5)
            };

            this.labelGreen = new GUIStyle
            {
                normal =
                {
                    textColor = Color.green
                }
            };

            this.labelYellow = new GUIStyle
            {
                normal =
                {
                    textColor = Color.yellow
                }
            };

            this.labelCyan = new GUIStyle
            {
                normal =
                {
                    textColor = Color.cyan
                }
            };
        }

        private void Window(int windowId)
        {
            try
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Installed Add-ons: " + AddonLibrary.TotalCount);
                GUILayout.FlexibleSpace();
                if (GUILayout.Toggle(this.showAddons, "Show Add-ons") != this.showAddons)
                {
                    this.showAddons = !this.showAddons;
                    this.position.height = 0.0f;
                }
                GUILayout.EndHorizontal();
                if (this.showAddons)
                {
                    this.DrawAddonList();
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

#endregion
    }
}