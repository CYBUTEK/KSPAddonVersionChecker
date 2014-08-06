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
using System.Linq;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class CheckGui : MonoBehaviour
    {
        #region Fields

        private bool hasCentred;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Initialisation

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            this.InitialiseStyles();
        }

        #endregion

        #region Styles

        private GUIStyle titleStyle;

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
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker", HighLogic.Skin.window);
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

        private void Window(int id)
        {
            try
            {
                GUILayout.BeginVertical(HighLogic.Skin.box);
                if (AddonLibrary.Populated)
                {
                    GUILayout.Label("Checked " + AddonLibrary.Addons.Count(a => a.IsProcessingComplete) + " of " + AddonLibrary.Addons.Count + " add-ons.", this.titleStyle, GUILayout.Width(300.0f));
                }
                else
                {
                    GUILayout.Label("Populating Library...", this.titleStyle, GUILayout.Width(300.0f));
                }
                GUILayout.EndVertical();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion
    }
}