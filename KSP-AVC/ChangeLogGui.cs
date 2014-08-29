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

        private Rect position = new Rect(0, 0, Screen.width, Screen.height);
        private Vector2 scrollPosition;

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Text { get; set; }

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
                Logger.Log("ChangeLogGui was created.");
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

        private GUIStyle closeStyle;
        private GUIStyle labelStyle;

        private void InitialiseStyles()
        {
            try
            {
                this.closeStyle = new GUIStyle(HighLogic.Skin.button)
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fixedHeight = 25.0f,
                };

                this.labelStyle = new GUIStyle(HighLogic.Skin.label)
                {
                    stretchWidth = true,
                    stretchHeight = true,
                    wordWrap = true
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
            GUI.skin = null;
            this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, this.Name + " - Change Log", HighLogic.Skin.window);
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

        #region Destruction

        private void OnDestroy()
        {
            Logger.Log("ChangeLogGui was destroyed.");
        }

        #endregion
    }
}