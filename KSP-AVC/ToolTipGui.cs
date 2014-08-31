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

namespace KSP_AVC
{
    public class ToolTipGui : MonoBehaviour
    {
        #region Fields

        private GUIContent content;
        private GUIStyle labelStyle;
        private Rect position;

        #endregion

        #region Properties

        public string Text
        {
            get { return (this.content ?? GUIContent.none).text; }
            set { this.content = new GUIContent(value); }
        }

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
            Logger.Log("ToolTipGui was created.");
        }

        protected void OnDestroy()
        {
            Logger.Log("ToolTipGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                if (this.content == null || String.IsNullOrEmpty(this.content.text))
                {
                    return;
                }

                GUILayout.Window(this.GetInstanceID(), this.position, this.Window, String.Empty, GUIStyle.none);
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

        protected void Update()
        {
            this.position.size = this.labelStyle.CalcSize(this.content ?? GUIContent.none);
            this.position.x = Mathf.Clamp(Input.mousePosition.x + 20.0f, 0, Screen.width - this.position.width);
            this.position.y = Screen.height - Input.mousePosition.y + (this.position.x < Input.mousePosition.x + 20.0f ? 20.0f : 0);
        }

        #endregion

        #region Methods: private

        private static Texture2D GetBackgroundTexture()
        {
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(1, 1, new Color(1.0f, 1.0f, 1.0f, 1.0f));
            texture.Apply();
            return texture;
        }

        private void InitialiseStyles()
        {
            this.labelStyle = new GUIStyle
            {
                padding = new RectOffset(4, 4, 2, 2),
                normal =
                {
                    textColor = Color.black,
                    background = GetBackgroundTexture()
                },
                fontSize = 11
            };
        }

        private void Window(int windowId)
        {
            try
            {
                GUI.BringWindowToFront(windowId);
                GUILayout.Label(this.content ?? GUIContent.none, this.labelStyle);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}