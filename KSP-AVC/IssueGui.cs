﻿// 
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

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace KSP_AVC
{
    public class IssueGui : MonoBehaviour
    {
        private readonly Dictionary<Addon, DropDownList> dropDownLists = new Dictionary<Addon, DropDownList>();

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private bool hasCentred;
        private GUIStyle labelStyle;
        private GUIStyle messageStyle;
        private GUIStyle nameLabelStyle;
        private GUIStyle nameTitleStyle;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);
        private GUIStyle titleStyle;

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
            try
            {
                foreach (var dropDownList in this.dropDownLists.Values)
                {
                    Destroy(dropDownList);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            Logger.Log("IssueGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker - Issue Monitor", HighLogic.Skin.window);
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

        private void CentreWindow()
        {
            if (this.hasCentred || !(this.position.width > 0) || !(this.position.height > 0))
            {
                return;
            }
            this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            this.hasCentred = true;
        }

        private DropDownList CreateActionList(Addon addon)
        {
            var dropDownList = this.gameObject.AddComponent<DropDownList>();
            dropDownList.Addon = addon;
            dropDownList.DrawAddonCallback = this.DrawActionList;
            this.dropDownLists.Add(addon, dropDownList);
            return dropDownList;
        }

        private void CreateChangeLogWindow(Addon addon)
        {
            var changeLogGui = this.gameObject.AddComponent<ChangeLogGui>();
            changeLogGui.Name = addon.RemoteInfo.Name;
            changeLogGui.Text = addon.RemoteInfo.ChangeLog;
        }

        private void DrawActionList(DropDownList list, Addon addon)
        {
            this.DrawActionListChangeLogButton(list, addon);
            this.DrawActionListDownloadButton(list, addon);
        }

        private void DrawActionListButton(Addon addon)
        {
            if (String.IsNullOrEmpty(addon.RemoteInfo.Download) && String.IsNullOrEmpty(addon.RemoteInfo.ChangeLog))
            {
                return;
            }

            (this.dropDownLists.ContainsKey(addon) ? this.dropDownLists[addon] : this.CreateActionList(addon)).DrawButton("ACTIONS", this.position, 125.0f);
        }

        private void DrawActionListChangeLogButton(DropDownList list, Addon addon)
        {
            if (String.IsNullOrEmpty(addon.RemoteInfo.ChangeLog))
            {
                return;
            }
            if (!GUILayout.Button("Change Log", this.buttonStyle))
            {
                return;
            }

            this.CreateChangeLogWindow(addon);
            list.ShowList = false;
        }

        private void DrawActionListDownloadButton(DropDownList list, Addon addon)
        {
            if (String.IsNullOrEmpty(addon.RemoteInfo.Download))
            {
                return;
            }

            if (GUILayout.Button("Download", this.buttonStyle))
            {
                Application.OpenURL(addon.RemoteInfo.Download);
                list.ShowList = false;
            }
            if (Event.current.type == EventType.repaint)
            {
                list.ToolTip.Text = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) ? list.ToolTip.Text = addon.RemoteInfo.Download : String.Empty;
            }
        }

        private void DrawCompatibilityIssueSection()
        {
            GUILayout.BeginVertical(this.boxStyle);
            GUILayout.Label("COMPATIBILITY ISSUES", this.nameTitleStyle);
            foreach (var addon in AddonLibrary.Addons.Where(a => !a.HasError && !a.IsCompatible))
            {
                GUILayout.Label("The currently installed version of " + addon.Name + " was built to run on KSP " + addon.LocalInfo.KspVersion, this.messageStyle);
            }
            GUILayout.EndVertical();
        }

        private void DrawUpdateIssue(Addon addon)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(addon.Name, this.nameLabelStyle, GUILayout.Width(250.0f));
            GUILayout.Label(addon.LocalInfo.Version.ToString(), this.labelStyle, GUILayout.Width(100.0f));
            GUILayout.Label(addon.RemoteInfo.Version.ToString(), this.labelStyle, GUILayout.Width(100.0f));
            this.DrawActionListButton(addon);
            GUILayout.EndHorizontal();
        }

        private void DrawUpdateIssueHeadings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("ADD-ON NAME", this.nameTitleStyle, GUILayout.Width(250.0f));
            GUILayout.Label("CURRENT", this.titleStyle, GUILayout.Width(100.0f));
            GUILayout.Label("AVAILABLE", this.titleStyle, GUILayout.Width(100.0f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawUpdateIssueSection()
        {
            GUILayout.BeginVertical(this.boxStyle);
            this.DrawUpdateIssueHeadings();

            foreach (var addon in AddonLibrary.Addons.Where(a => !a.HasError && a.IsUpdateAvailable))
            {
                this.DrawUpdateIssue(addon);
            }
            GUILayout.EndVertical();
        }

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
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            this.nameLabelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleLeft,
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedHeight = 25.0f,
                alignment = TextAnchor.MiddleCenter,
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fixedWidth = this.nameLabelStyle.fixedWidth + (this.labelStyle.fixedWidth * 3)
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
                if (AddonLibrary.Addons.Any(a => !a.HasError && a.IsUpdateAvailable))
                {
                    this.DrawUpdateIssueSection();
                }
                if (AddonLibrary.Addons.Any(a => !a.HasError && !a.IsCompatible))
                {
                    this.DrawCompatibilityIssueSection();
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
    }
}