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
using System.Collections.Generic;

using UnityEngine;

#endregion

namespace MiniAVC
{
    public class FirstRunGui : MonoBehaviour
    {
        #region Fields

        private bool hasCentred;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Properties

        public List<Addon> Addons { get; set; }

        public AddonSettings Settings { get; set; }

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
                Logger.Log("FirstRunGui was created.");
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
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "MiniAVC", HighLogic.Skin.window);
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
                GUILayout.BeginVertical(HighLogic.Skin.box);
                GUILayout.Label("Allow these add-ons to check for updates?", this.titleStyle, GUILayout.Width(300.0f));
                foreach (var addon in this.Addons)
                {
                    GUILayout.Label(addon.Name, this.labelStyle);
                }
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("YES", this.buttonStyle, GUILayout.Width(200.0f)))
                {
                    this.Settings.FirstRun = false;
                    this.Settings.AllowCheck = true;
                    this.Settings.Save();
                    foreach (var addon in this.Addons)
                    {
                        Logger.Log("Remote checking has been enabled for: " + addon.Name);
                        addon.RunProcessRemoteInfo();
                    }
                    Destroy(this);
                }
                if (GUILayout.Button("NO", this.buttonStyle))
                {
                    this.Settings.FirstRun = false;
                    this.Settings.AllowCheck = false;
                    this.Settings.Save();
                    foreach (var addon in this.Addons)
                    {
                        Logger.Log("Remote checking has been disabled for: " + addon.Name);
                        addon.RunProcessRemoteInfo();
                    }
                    Destroy(this);
                }
                GUILayout.EndHorizontal();
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
            Logger.Log("FirstRunGui was destroyed.");
        }

        #endregion
    }
}