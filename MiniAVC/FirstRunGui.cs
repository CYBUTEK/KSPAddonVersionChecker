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

        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle labelStyle;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private GUIStyle titleStyle;

        #endregion

        #region Properties

        public List<Addon> Addons { get; set; }

        public AddonSettings Settings { get; set; }

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
            Logger.Log("FirstRunGui was created.");
        }

        protected void OnDestroy()
        {
            Logger.Log("FirstRunGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "MiniAVC", HighLogic.Skin.window);
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

        private void DrawAddonList()
        {
            GUILayout.Label("Allow these add-ons to check for updates?", this.titleStyle, GUILayout.Width(300.0f));
            foreach (var addon in this.Addons)
            {
                GUILayout.Label(addon.Name, this.labelStyle);
            }
        }

        private void DrawButtonNo()
        {
            if (!GUILayout.Button("NO", this.buttonStyle))
            {
                return;
            }

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

        private void DrawButtonYes()
        {
            if (!GUILayout.Button("YES", this.buttonStyle, GUILayout.Width(200.0f)))
            {
                return;
            }

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
                GUILayout.BeginVertical(HighLogic.Skin.box);
                this.DrawAddonList();
                GUILayout.EndVertical();
                GUILayout.BeginHorizontal();
                this.DrawButtonYes();
                this.DrawButtonNo();
                GUILayout.EndHorizontal();
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