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

        private bool hasCentred;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Properties

        public Addon Addon { get; set; }

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                this.enabled = false;
                Logger.Log("IssueGui was created.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void Start()
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

        #region Styles

        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle titleStyle;

        private void InitialiseStyles()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, this.Addon.Name + " - MiniAVC", HighLogic.Skin.window);
                if (!this.hasCentred && this.position.width > 0 && this.position.height > 0)
                {
                    this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    this.hasCentred = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void Window(int id)
        {
            try
            {
                if (this.Addon.IsUpdateAvailable)
                {
                    GUILayout.BeginVertical(HighLogic.Skin.box);
                    GUILayout.Label("AN UPDATE IS AVAILABLE", this.titleStyle);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Installed: " + this.Addon.LocalInfo.Version, this.labelStyle, GUILayout.Width(150.0f));
                    GUILayout.Label("Available: " + this.Addon.RemoteInfo.Version, this.labelStyle, GUILayout.Width(150.0f));
                    GUILayout.EndHorizontal();

                    if (!string.IsNullOrEmpty(this.Addon.RemoteInfo.Download))
                    {
                        if (GUILayout.Button("DOWNLOAD", this.buttonStyle))
                        {
                            Application.OpenURL(this.Addon.RemoteInfo.Download);
                        }
                    }
                    GUILayout.EndVertical();
                }

                if (!this.Addon.IsCompatible)
                {
                    GUILayout.BeginVertical(HighLogic.Skin.box);
                    GUILayout.Label("Unsupported KSP version... Please use " + this.Addon.LocalInfo.KspVersion, this.titleStyle, GUILayout.Width(300.0f));
                    GUILayout.EndVertical();
                }

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

        #region Destruction

        private void OnDestroy()
        {
            Logger.Log("IssueGui was destroyed.");
        }

        #endregion
    }
}