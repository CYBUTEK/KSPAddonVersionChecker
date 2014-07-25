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

using AddonVersionChecker.Extensions;

using UnityEngine;

#endregion

namespace AddonVersionChecker
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class FirstRunGui : MonoBehaviour
    {
        private bool isUpdate;
        private GUIStyle labelStyle;
        private int windowId;
        private Rect windowPosition = new Rect(Screen.width, Screen.height, 0, 0);

        public static bool IsOpen { get; private set; }

        private void Awake()
        {
            if (new System.Version(Settings.Instance.Version) < Assembly.GetExecutingAssembly().GetName().Version)
            {
                this.isUpdate = true;
                Settings.Instance.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            else if (!Settings.Instance.FirstRun)
            {
                Destroy(this);
            }
            else
            {
                Settings.Instance.FirstRun = false;
            }

            IsOpen = true;
            this.windowId = this.GetHashCode();

            this.labelStyle = new GUIStyle(HighLogic.Skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };
        }

        private void OnGUI()
        {
            this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, this.isUpdate ? "KSP-AVC Updated" : "KSP-AVC Installed", HighLogic.Skin.window, GUILayout.Width(300.0f)).CentreWindow();
        }

        private void Window(int windowId)
        {
            GUILayout.Label(this.isUpdate ? "Thank you for updating the KSP-AVC plugin." : "Thank you for installing the KSP-AVC plugin.", this.labelStyle);

            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            Settings.Save();
            IsOpen = false;
        }
    }
}