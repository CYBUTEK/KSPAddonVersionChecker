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
using System.Linq;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class IssueGui : MonoBehaviour
    {
        #region Fields

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

        #endregion

        #region Methods: private

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
                Logger.Log("IssueGui was created.");
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void DrawCompatibilityIssues()
        {
            try
            {
                GUILayout.BeginVertical(this.boxStyle);
                GUILayout.Label("COMPATIBILITY ISSUES", this.nameTitleStyle);
                foreach (var addon in AddonLibrary.Addons.Where(a => !a.HasError && !a.IsCompatible))
                {
                    GUILayout.Label("The currently installed version of " + addon.Name + " was built to run on KSP " + addon.LocalInfo.KspVersion, this.messageStyle);
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void DrawDropDownList(DropDownList list, Addon addon)
        {
            if (!String.IsNullOrEmpty(addon.RemoteInfo.ChangeLog))
            {
                if (GUILayout.Button("Change Log", this.buttonStyle))
                {
                    var changeLogGui = this.gameObject.AddComponent<ChangeLogGui>();
                    changeLogGui.Name = addon.RemoteInfo.Name;
                    changeLogGui.Text = addon.RemoteInfo.ChangeLog;
                    list.ShowList = false;
                }
            }

            if (!String.IsNullOrEmpty(addon.RemoteInfo.Download))
            {
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
        }

        private void DrawUpdateIssues()
        {
            try
            {
                GUILayout.BeginVertical(this.boxStyle);
                GUILayout.BeginHorizontal();
                GUILayout.Label("ADD-ON NAME", this.nameTitleStyle, GUILayout.Width(250.0f));
                GUILayout.Label("CURRENT", this.titleStyle, GUILayout.Width(100.0f));
                GUILayout.Label("AVAILABLE", this.titleStyle, GUILayout.Width(100.0f));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                foreach (var addon in AddonLibrary.Addons.Where(a => !a.HasError && a.IsUpdateAvailable))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(addon.Name, this.nameLabelStyle, GUILayout.Width(250.0f));
                    GUILayout.Label(addon.LocalInfo.Version.ToString(), this.labelStyle, GUILayout.Width(100.0f));
                    GUILayout.Label(addon.RemoteInfo.Version.ToString(), this.labelStyle, GUILayout.Width(100.0f));
                    if (!string.IsNullOrEmpty(addon.RemoteInfo.Download))
                    {
                        DropDownList dropDownList;
                        if (this.dropDownLists.ContainsKey(addon))
                        {
                            dropDownList = this.dropDownLists[addon];
                        }
                        else
                        {
                            dropDownList = this.gameObject.AddComponent<DropDownList>();
                            dropDownList.Addon = addon;
                            dropDownList.DrawAddonCallback = this.DrawDropDownList;
                            this.dropDownLists.Add(addon, dropDownList);
                        }
                        dropDownList.DrawButton("ACTIONS", this.position, 125.0f);
                    }
                    else
                    {
                        GUILayout.Label("-----", this.labelStyle);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void InitialiseStyles()
        {
            try
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
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnDestroy()
        {
            foreach (var dropDownList in this.dropDownLists.Values)
            {
                Destroy(dropDownList);
            }
            Logger.Log("IssueGui was destroyed.");
        }

        private void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker - Issue Monitor", HighLogic.Skin.window);
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

        private void Window(int id)
        {
            try
            {
                if (AddonLibrary.Addons.Any(a => !a.HasError && a.IsUpdateAvailable))
                {
                    this.DrawUpdateIssues();
                }
                if (AddonLibrary.Addons.Any(a => !a.HasError && !a.IsCompatible))
                {
                    this.DrawCompatibilityIssues();
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
    }
}