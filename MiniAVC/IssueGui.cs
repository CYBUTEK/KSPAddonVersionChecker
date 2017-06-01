// Copyright (C) 2014 CYBUTEK
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU
// General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program. If not,
// see <http://www.gnu.org/licenses/>.

using System;

using UnityEngine;

namespace MiniAVC
{
    public class IssueGui : MonoBehaviour
    {
        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle labelStyle;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private GUIStyle titleStyle;
        private ToolTipGui toolTipGui;

        public Addon Addon { get; set; }

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
                position = GUILayout.Window(GetInstanceID(), position, Window, Addon.Name, HighLogic.Skin.window);
                CentreWindow();
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
                InitialiseStyles();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void CentreWindow()
        {
            if (hasCentred || !(position.width > 0) || !(position.height > 0))
            {
                return;
            }
            position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            hasCentred = true;
        }

        private void DrawDownloadButton()
        {
            if (String.IsNullOrEmpty(Addon.RemoteInfo.Download))
            {
                return;
            }

            if (GUILayout.Button("UPDATE", buttonStyle))
            {
                Application.OpenURL(Addon.RemoteInfo.Download);
            }

            if (toolTipGui == null)
            {
                toolTipGui = gameObject.AddComponent<ToolTipGui>();
            }
            toolTipGui.Text = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) ? Addon.RemoteInfo.Download : String.Empty;
        }

        private void DrawNotCompatible()
        {
            if (Addon.IsCompatible)
            {
                return;
            }

            GUILayout.BeginVertical(HighLogic.Skin.box);
            GUILayout.Label($"Unsupported by KSP v{VersioningBase.GetVersionString()}, please use v{Addon.LocalInfo.KspVersion}.", titleStyle, GUILayout.Width(400.0f));
            GUILayout.EndVertical();
        }

        private void DrawUpdateAvailable()
        {
            if (!Addon.IsUpdateAvailable)
            {
                return;
            }

            GUILayout.BeginVertical(HighLogic.Skin.box);
            GUILayout.Label("AN UPDATE IS AVAILABLE", titleStyle);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Installed: " + Addon.LocalInfo.Version, labelStyle, GUILayout.Width(150.0f));
            GUILayout.Label("Available: " + Addon.RemoteInfo.Version, labelStyle, GUILayout.Width(150.0f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            DrawDownloadButton();
            GUILayout.EndVertical();
        }

        private void InitialiseStyles()
        {
            titleStyle = new GUIStyle(HighLogic.Skin.label)
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

            labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            buttonStyle = new GUIStyle(HighLogic.Skin.button)
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
                DrawUpdateAvailable();
                DrawNotCompatible();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("REMIND ME LATER", buttonStyle))
                {
                    Destroy(this);
                }
                if (GUILayout.Button("IGNORE THIS UPDATE", buttonStyle, GUILayout.Width(175f)))
                {
                    Addon.Settings.IgnoredUpdates.Add(Addon.Base64String);
                    Addon.Settings.Save();
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
    }
}