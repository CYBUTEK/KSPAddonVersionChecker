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
using System.IO;
using System.Reflection;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class DropDownList : MonoBehaviour
    {
        #region Fields

        private Rect listPosition;
        private Rect togglePosition;

        #endregion

        #region Properties

        public bool ShowList { get; set; }

        public ToolTipGui ToolTip { get; set; }

        public Addon Addon { get; set; }

        public Action<DropDownList> DrawCallback { get; set; }

        public Action<DropDownList, Addon> DrawAddonCallback { get; set; }

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
                this.InitialiseStyles();
                this.ToolTip = this.gameObject.AddComponent<ToolTipGui>();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Styles

        private GUIStyle listStyle;
        private GUIStyle toggleStyle;

        private void InitialiseStyles()
        {
            try
            {
                var texturesDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Textures");
                var normal = new Texture2D(35, 25, TextureFormat.ARGB32, false);
                var hover = new Texture2D(35, 25, TextureFormat.ARGB32, false);
                var active = new Texture2D(35, 25, TextureFormat.ARGB32, false);
                var onNormal = new Texture2D(35, 25, TextureFormat.ARGB32, false);
                var onHover = new Texture2D(35, 25, TextureFormat.ARGB32, false);
                var background = new Texture2D(35, 25, TextureFormat.ARGB32, false);
                normal.LoadImage(File.ReadAllBytes(Path.Combine(texturesDirectory, "DropDownNormal.png")));
                hover.LoadImage(File.ReadAllBytes(Path.Combine(texturesDirectory, "DropDownHover.png")));
                active.LoadImage(File.ReadAllBytes(Path.Combine(texturesDirectory, "DropDownActive.png")));
                onNormal.LoadImage(File.ReadAllBytes(Path.Combine(texturesDirectory, "DropDownOnNormal.png")));
                onHover.LoadImage(File.ReadAllBytes(Path.Combine(texturesDirectory, "DropDownOnHover.png")));
                background.LoadImage(File.ReadAllBytes(Path.Combine(texturesDirectory, "DropDownBackground.png")));

                this.listStyle = new GUIStyle
                {
                    normal =
                    {
                        background = background
                    },
                    border = new RectOffset(5, 5, 0, 5),
                    padding = new RectOffset(3, 3, 3, 3)
                };

                this.toggleStyle = new GUIStyle
                {
                    normal =
                    {
                        background = normal,
                        textColor = Color.white
                    },
                    hover =
                    {
                        background = hover,
                        textColor = Color.white
                    },
                    active =
                    {
                        background = active,
                        textColor = Color.white
                    },
                    onNormal =
                    {
                        background = onNormal,
                        textColor = Color.white
                    },
                    onHover =
                    {
                        background = onHover,
                        textColor = Color.white
                    },
                    border = new RectOffset(5, 25, 5, 5),
                    margin = new RectOffset(0, 0, 3, 3),
                    padding = new RectOffset(5, 30, 5, 5),
                    fixedHeight = 25.0f,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Updating

        private void Update()
        {
            try
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    if (!this.listPosition.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) && !this.togglePosition.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                    {
                        this.ShowList = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Drawing

        public void DrawButton(string label, Rect parent, float width)
        {
            try
            {
                this.ShowList = GUILayout.Toggle(this.ShowList, label, this.toggleStyle, GUILayout.Width(width));
                if (Event.current.type == EventType.repaint)
                {
                    this.SetPosition(GUILayoutUtility.GetLastRect(), parent);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void OnGUI()
        {
            try
            {
                if (!this.ShowList)
                {
                    if (!String.IsNullOrEmpty(this.ToolTip.Text))
                    {
                        this.ToolTip.Text = String.Empty;
                    }
                    return;
                }
                this.listPosition = GUILayout.Window(this.GetInstanceID(), this.listPosition, this.Window, String.Empty, this.listStyle);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        private void Window(int windowId)
        {
            try
            {
                GUI.BringWindowToFront(windowId);
                GUI.BringWindowToFront(this.ToolTip.GetInstanceID());

                if (this.DrawCallback != null)
                {
                    this.DrawCallback(this);
                }
                else if (this.DrawAddonCallback != null)
                {
                    this.DrawAddonCallback(this, this.Addon);
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Private Methods

        private void SetPosition(Rect toggle, Rect parent)
        {
            try
            {
                this.togglePosition = toggle;
                this.togglePosition.x += parent.x;
                this.togglePosition.y += parent.y;
                this.listPosition.x = this.togglePosition.x;
                this.listPosition.y = this.togglePosition.y + this.togglePosition.height;
                this.listPosition.width = this.togglePosition.width;
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
            try
            {
                if (this.ToolTip != null)
                {
                    Destroy(this.ToolTip);
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