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

        private readonly string textureDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Textures");

        private Rect listPosition;
        private GUIStyle listStyle;
        private Rect togglePosition;
        private GUIStyle toggleStyle;

        #endregion

        #region Properties

        public Addon Addon { get; set; }

        public Action<DropDownList, Addon> DrawAddonCallback { get; set; }

        public Action<DropDownList> DrawCallback { get; set; }

        public bool ShowList { get; set; }

        public ToolTipGui ToolTip { get; set; }

        #endregion

        #region Methods: public

        public void DrawButton(string label, Rect parent, float width)
        {
            this.ShowList = GUILayout.Toggle(this.ShowList, label, this.toggleStyle, GUILayout.Width(width));
            if (Event.current.type == EventType.repaint)
            {
                this.SetPosition(GUILayoutUtility.GetLastRect(), parent);
            }
        }

        #endregion

        #region Methods: protected

        protected void Awake()
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

        protected void OnDestroy()
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

        protected void OnGUI()
        {
            try
            {
                if (!this.ShowList)
                {
                    if (this.ToolTip != null && !String.IsNullOrEmpty(this.ToolTip.Text))
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

        protected void Update()
        {
            try
            {
                if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
                {
                    return;
                }

                if (!this.listPosition.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) && !this.togglePosition.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    this.ShowList = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private Texture2D GetTexture(string file, int width, int height)
        {
            try
            {
                var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                texture.LoadImage(File.ReadAllBytes(Path.Combine(this.textureDirectory, file)));
                return texture;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return null;
            }
        }

        private void InitialiseStyles()
        {
            this.listStyle = new GUIStyle
            {
                normal =
                {
                    background = this.GetTexture("DropDownBackground.png", 35, 25)
                },
                border = new RectOffset(5, 5, 0, 5),
                padding = new RectOffset(3, 3, 3, 3)
            };

            this.toggleStyle = new GUIStyle
            {
                normal =
                {
                    background = this.GetTexture("DropDownNormal.png", 35, 25),
                    textColor = Color.white
                },
                hover =
                {
                    background = this.GetTexture("DropDownHover.png", 35, 25),
                    textColor = Color.white
                },
                active =
                {
                    background = this.GetTexture("DropDownActive.png", 35, 25),
                    textColor = Color.white
                },
                onNormal =
                {
                    background = this.GetTexture("DropDownOnNormal.png", 35, 25),
                    textColor = Color.white
                },
                onHover =
                {
                    background = this.GetTexture("DropDownOnHover.png", 35, 25),
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

        private void SetPosition(Rect toggle, Rect parent)
        {
            this.togglePosition = toggle;
            this.togglePosition.x += parent.x;
            this.togglePosition.y += parent.y;
            this.listPosition.x = this.togglePosition.x;
            this.listPosition.y = this.togglePosition.y + this.togglePosition.height;
            this.listPosition.width = this.togglePosition.width;
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
    }
}