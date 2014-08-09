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
    public class ToolTipGui : MonoBehaviour
    {
        #region Fields

        private GUIContent content;
        private Rect position;
        private string text = string.Empty;

        #endregion

        #region Properties

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                this.content = new GUIContent(this.text);
            }
        }

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
                Logger.Log("ToolTipGui was created.");
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

        private GUIStyle labelStyle;

        private void InitialiseStyles()
        {
            var background = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            background.SetPixel(1, 1, new Color(1.0f, 1.0f, 1.0f, 1.0f));
            background.Apply();

            this.labelStyle = new GUIStyle
            {
                padding = new RectOffset(4, 4, 2, 2),
                normal =
                {
                    textColor = Color.black,
                    background = background
                },
                fontSize = 11
            };
        }

        #endregion

        #region Updating

        private void Update()
        {
            this.position.size = this.labelStyle.CalcSize(this.content);
            this.position.x = Mathf.Clamp(Input.mousePosition.x + 20.0f, 0, Screen.width - this.position.width);
            this.position.y = Screen.height - Input.mousePosition.y + (this.position.x < Input.mousePosition.x + 20.0f ? 20.0f : 0);
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            try
            {
                if (string.IsNullOrEmpty(this.text))
                {
                    return;
                }

                GUILayout.Window(this.GetInstanceID(), this.position, this.Window, string.Empty, GUIStyle.none);
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
                GUILayout.Label(this.content, this.labelStyle);
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
            Logger.Log("ToolTipGui was destroyed.");
        }

        #endregion
    }
}