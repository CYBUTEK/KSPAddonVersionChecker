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

using UnityEngine;

#endregion

namespace MiniAVC
{
    public class IssueGui : MonoBehaviour
    {
        #region Fields

        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle labelStyle;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private GUIStyle titleStyle;
        private ToolTipGui toolTipGui;

        #endregion

        #region Properties

        public Addon Addon { get; set; }

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
            Logger.Log("IssueGui was created.");
        }

        protected void OnDestroy()
        {
            Logger.Log("IssueGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, this.Addon.Name, HighLogic.Skin.window);
                this.CentreWindow();
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

        #endregion

        #region Methods: private

        private void CentreWindow()
        {
            if (this.hasCentred || !(this.position.width > 0) || !(this.position.height > 0))
            {
                return;
            }
            this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            this.hasCentred = true;
        }

        private void DrawDownloadButton()
        {
            if (String.IsNullOrEmpty(this.Addon.RemoteInfo.Download))
            {
                return;
            }

            if (GUILayout.Button("DOWNLOAD", this.buttonStyle))
            {
                Application.OpenURL(this.Addon.RemoteInfo.Download);
            }

            if (this.toolTipGui == null)
            {
                this.toolTipGui = this.gameObject.AddComponent<ToolTipGui>();
            }
            this.toolTipGui.Text = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) ? this.Addon.RemoteInfo.Download : String.Empty;
        }

        private void DrawNotCompatible()
        {
            if (this.Addon.IsCompatible)
            {
                return;
            }

            GUILayout.BeginVertical(HighLogic.Skin.box);
            GUILayout.Label("Unsupported KSP version... Please use " + this.Addon.LocalInfo.KspVersion, this.titleStyle, GUILayout.Width(300.0f));
            GUILayout.EndVertical();
        }

        private void DrawUpdateAvailable()
        {
            if (!this.Addon.IsUpdateAvailable)
            {
                return;
            }

            GUILayout.BeginVertical(HighLogic.Skin.box);
            GUILayout.Label("AN UPDATE IS AVAILABLE", this.titleStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Installed: " + this.Addon.LocalInfo.Version, this.labelStyle, GUILayout.Width(150.0f));
            GUILayout.Label("Available: " + this.Addon.RemoteInfo.Version, this.labelStyle, GUILayout.Width(150.0f));
            GUILayout.EndHorizontal();
            this.DrawDownloadButton();
            GUILayout.EndVertical();
        }

        private void InitialiseStyles()
        {
            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        private void Window(int id)
        {
            try
            {
                this.DrawUpdateAvailable();
                this.DrawNotCompatible();

                if (GUILayout.Button("CLOSE", this.buttonStyle))
                {
                    Destroy(this);
                }

                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}