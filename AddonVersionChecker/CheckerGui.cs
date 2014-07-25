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

using AddonVersionChecker.Extensions;

using UnityEngine;

#endregion

namespace AddonVersionChecker
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class CheckerGui : MonoBehaviour
    {
        #region Fields

        private readonly int windowId = typeof(CheckerGui).GetHashCode();
        private Rect windowPosition = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Constructors

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            this.InitialiseStyles();
        }

        #endregion

        #region Styles

        private GUIStyle boxStyle;
        private GUIStyle closeStyle;
        private GUIStyle downloadStyle;
        private GUIStyle messageStyle;
        private GUIStyle nameStyle;
        private GUIStyle nameTitleStyle;
        private GUIStyle versionStyle;
        private GUIStyle versionTitleStyle;
        private GUIStyle windowStyle;

        private void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window);

            this.boxStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 5, 5)
            };

            this.nameTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fixedWidth = 300.0f,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold
            };

            this.versionTitleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fixedWidth = 100.0f,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            this.nameStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedWidth = 300.0f,
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleLeft,
            };

            this.versionStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedWidth = 100.0f,
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleCenter,
            };

            this.downloadStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fixedWidth = 100.0f,
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleCenter,
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedWidth = this.nameStyle.fixedWidth + (this.versionStyle.fixedWidth * 3)
            };

            this.closeStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fixedHeight = 25.0f
            };
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            if (AddonManager.IsLocked || !AddonManager.HasIssues || FirstRunGui.IsOpen)
            {
                return;
            }

            this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, "KSP Add-on Version Checker - Issue Monitor", this.windowStyle).CentreWindow();
        }

        private void Window(int id)
        {
            if (AddonManager.HasUpdateIssues)
            {
                this.DrawUpdateIssues();
            }

            if (AddonManager.HasCompatibilityIssues)
            {
                this.DrawCompatibilityIssues();
            }

            if (GUILayout.Button("CLOSE", this.closeStyle))
            {
                Destroy(this);
            }
        }

        private void DrawUpdateIssues()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ADD-ON NAME", this.nameTitleStyle);
            GUILayout.Label("CURRENT", this.versionTitleStyle);
            GUILayout.Label("AVAILABLE", this.versionTitleStyle);
            GUILayout.Label("DOWNLOAD", this.versionTitleStyle);
            GUILayout.EndHorizontal();

            foreach (var addon in AddonManager.Addons)
            {
                if (addon.UpdateAvailable)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(addon.Name, this.nameStyle);
                    GUILayout.Label(addon.AddonVersion.ToString(), this.versionStyle);
                    GUILayout.Label(addon.RemoteAddonData.AddonVersion.ToString(), this.versionStyle);
                    if (addon.RemoteAddonData.Download.Length > 0)
                    {
                        if (GUILayout.Button("DOWNLOAD", this.downloadStyle))
                        {
                            Application.OpenURL(addon.RemoteAddonData.Download);
                        }
                    }
                    else
                    {
                        GUILayout.Label("-----", this.versionStyle);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void DrawCompatibilityIssues()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.Label("COMPATIBILITY ISSUES", this.nameTitleStyle);
            foreach (var addon in AddonManager.Addons)
            {
                if (!addon.GameCompatibleVersion)
                {
                    GUILayout.Label(addon.Name + " version " + addon.AddonVersion + " was built to run on KSP " + addon.GameVersion + ".", this.messageStyle);
                }
                else if (!addon.GameCompatibleMininmum)
                {
                    if (addon.GameVersionMaximum == AddonData.DefaultMaximumVersion)
                    {
                        GUILayout.Label(addon.Name + " version " + addon.AddonVersion + " was built to run on KSP " + addon.GameVersionMinimum + " and above.", this.messageStyle);
                    }
                    else
                    {
                        GUILayout.Label(addon.Name + " version " + addon.AddonVersion + " was built to run on KSP " + addon.GameVersionMinimum + " - " + addon.GameVersionMaximum + ".", this.messageStyle);
                    }
                }
                else if (!addon.GameCompatibleMaximum)
                {
                    if (addon.GameVersionMinimum == AddonData.DefaultMinimumVersion)
                    {
                        GUILayout.Label(addon.Name + " version " + addon.AddonVersion + " was built to run on KSP " + addon.GameVersionMaximum + " and below.", this.messageStyle);
                    }
                    else
                    {
                        GUILayout.Label(addon.Name + " version " + addon.AddonVersion + " was built to run on KSP " + addon.GameVersionMinimum + " - " + addon.GameVersionMaximum + ".", this.messageStyle);
                    }
                }
            }
            GUILayout.EndVertical();
        }

        #endregion
    }
}