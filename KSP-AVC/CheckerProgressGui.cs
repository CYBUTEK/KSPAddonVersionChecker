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
    public class CheckerProgressGui : MonoBehaviour
    {
        #region Fields
        
        private bool hasCentred;
        private string message = String.Empty;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);

        private GUIStyle titleStyle;

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
            Logger.Log("CheckerProgressGui was created.");
        }

        protected void OnDestroy()
        {
            Logger.Log("CheckerProgressGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP Add-on Version Checker", HighLogic.Skin.window);
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

        protected void Update()
        {
            try
            {
                this.message = AddonLibrary.Populated
                    ? "Checked " + AddonLibrary.ProcessCount + " of " + AddonLibrary.TotalCount + " add-ons."
                    : "Populating Library...";
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void CentreWindow()
        {
            if (this.hasCentred || !(this.position.width > 0) || !(this.position.height > 0))
            {
                return;
            }
            this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            this.hasCentred = true;
        }

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

        private void Window(int id)
        {
            try
            {
                GUILayout.BeginVertical(HighLogic.Skin.box);
                GUILayout.Label(this.message, this.titleStyle, GUILayout.Width(300.0f));
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