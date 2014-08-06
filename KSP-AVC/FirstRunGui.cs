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

using System.Reflection;

using UnityEngine;

#endregion

namespace KSP_AVC
{
    public class FirstRunGui : MonoBehaviour
    {
        #region Fields

        private bool hasCentred;
        private Rect position = new Rect(Screen.width, Screen.height, 0, 0);

        #endregion

        #region Properties

        public bool HasBeenUpdated { get; set; }

        #endregion

        #region Initialisation

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Logger.Log("FirstRunGui was created.");
        }

        private void Start()
        {
            this.InitialiseStyles();
        }

        #endregion

        #region Styles

        private GUIStyle buttonStyle;
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

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                }
            };
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "KSP-AVC Plugin - " + (this.HasBeenUpdated ? "Updated" : "Installed"), HighLogic.Skin.window);
            if (!this.hasCentred && this.position.width > 0 && this.position.height > 0)
            {
                this.position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                this.hasCentred = true;
            }
        }

        private void Window(int id)
        {
            GUILayout.BeginVertical(HighLogic.Skin.box);
            if (this.HasBeenUpdated)
            {
                GUILayout.Label("You have successfully updated KSP-AVC to v" + this.GetVersion(), this.titleStyle, GUILayout.Width(350.0f));
            }
            else
            {
                GUILayout.Label("You have successfully installed KSP-AVC v" + this.GetVersion(), this.titleStyle, GUILayout.Width(350.0f));
            }
            GUILayout.EndVertical();
            if (GUILayout.Button("CLOSE", this.buttonStyle))
            {
                Destroy(this);
            }
            GUI.DragWindow();
        }

        #endregion

        #region Private Methods

        private System.Version GetVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            return version.Revision != 0 ? version
                : version.Build != 0 ? new System.Version(version.Major, version.Minor, version.Build)
                    : new System.Version(version.Major, version.Minor);
        }

        #endregion

        #region Destruction

        private void OnDestroy()
        {
            Logger.Log("FirstRunGui was destroyed.");
        }

        #endregion
    }
}