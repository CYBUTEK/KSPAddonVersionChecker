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
    public class ChangeLogGui : MonoBehaviour
    {
        #region Fields

        private GUIStyle closeStyle;
        private GUIStyle labelStyle;
        private Rect position = new Rect(0, 0, Screen.width, Screen.height);
        private Vector2 scrollPosition;

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Text { get; set; }

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
            Logger.Log("ChangeLogGui was created.");
        }

        protected void OnDestroy()
        {
            Logger.Log("ChangeLogGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                GUI.skin = null;
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, this.Name + " - Change Log", HighLogic.Skin.window);
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

        private void InitialiseStyles()
        {
            this.closeStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
            };

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontStyle = FontStyle.Bold,
                stretchWidth = true,
                stretchHeight = true,
                wordWrap = true
            };
        }

        private void Window(int id)
        {
            try
            {
                GUI.skin = HighLogic.Skin;
                this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true);
                GUI.skin = null;
                GUILayout.Label(this.Text, this.labelStyle);
                GUILayout.EndScrollView();

                if (GUILayout.Button("CLOSE", this.closeStyle))
                {
                    Destroy(this);
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