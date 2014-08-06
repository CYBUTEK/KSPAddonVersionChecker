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

using System.Linq;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class IssueGui : MonoBehaviour
    {
        #region Fields

        private bool hasCentred;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Initialisation

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Logger.Log("IssueGui was created.");
        }

        private void Start()
        {
            this.InitialiseStyles();
        }

        #endregion

        #region Styles

        private GUIStyle boxStyle;
        private GUIStyle closeStyle;
        private GUIStyle downloadButtonStyle;
        private GUIStyle labelStyle;
        private GUIStyle messageStyle;
        private GUIStyle nameLabelStyle;
        private GUIStyle nameTitleStyle;
        private GUIStyle titleStyle;

        private void InitialiseStyles()
        {
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

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fixedWidth = 100.0f,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            this.nameLabelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedWidth = 300.0f,
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleLeft,
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedWidth = 100.0f,
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleCenter,
            };

            this.downloadButtonStyle = new GUIStyle(HighLogic.Skin.button)
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
                fixedWidth = this.nameLabelStyle.fixedWidth + (this.labelStyle.fixedWidth * 3)
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
            this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker - Issue Monitor", HighLogic.Skin.window);
            if (!this.hasCentred && this.position.width > 0 && this.position.height > 0)
            {
                this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                this.hasCentred = true;
            }
        }

        private void Window(int id)
        {
            if (AddonLibrary.Addons.Any(a => a.IsUpdateAvailable))
            {
                this.DrawUpdateIssues();
            }
            if (AddonLibrary.Addons.Any(a => !a.IsCompatible))
            {
                this.DrawCompatibilityIssues();
            }
            if (GUILayout.Button("CLOSE", this.closeStyle))
            {
                Destroy(this);
            }
            GUI.DragWindow();
        }

        private void DrawUpdateIssues()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("ADD-ON NAME", this.nameTitleStyle);
            GUILayout.Label("CURRENT", this.titleStyle);
            GUILayout.Label("AVAILABLE", this.titleStyle);
            GUILayout.Label("DOWNLOAD", this.titleStyle);
            GUILayout.EndHorizontal();

            foreach (var addon in AddonLibrary.Addons.Where(a => a.IsUpdateAvailable))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, this.nameLabelStyle);
                GUILayout.Label(addon.LocalInfo.Version.ToString(), this.labelStyle);
                GUILayout.Label(addon.RemoteInfo.Version.ToString(), this.labelStyle);
                if (!string.IsNullOrEmpty(addon.RemoteInfo.Download))
                {
                    if (GUILayout.Button("DOWNLOAD", this.downloadButtonStyle))
                    {
                        Application.OpenURL(addon.RemoteInfo.Download);
                    }
                }
                else
                {
                    GUILayout.Label("-----", this.labelStyle);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void DrawCompatibilityIssues()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.Label("COMPATIBILITY ISSUES", this.nameTitleStyle);
            foreach (var addon in AddonLibrary.Addons.Where(a => !a.IsCompatible))
            {
                GUILayout.Label("The currently installed version of " + addon.Name + " was built to run on KSP " + addon.LocalInfo.KspVersion, this.messageStyle);
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Destroyed

        private void OnDestroy()
        {
            Logger.Log("IssueGui was destroyed.");
        }

        #endregion
    }
}