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
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AddonListGui : MonoBehaviour
    {
        #region Fields

        private DropDownList dropDownList;

        #endregion

        #region Initialisation

        private void Awake()
        {
            try
            {
                DontDestroyOnLoad(this);
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
                this.dropDownList = this.gameObject.AddComponent<DropDownList>();
                this.dropDownList.DrawCallback = this.DrawListItems;
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        #endregion

        #region Styles

        private GUIStyle labelStyleLeft;
        private GUIStyle labelStyleLeftIssue;
        private GUIStyle labelStyleRight;
        private GUIStyle labelStyleRightIssue;

        private void InitialiseStyles()
        {
            try
            {
                this.labelStyleLeft = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    padding = new RectOffset(5, 5, 3, 3),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleLeft,
                    stretchWidth = true
                };

                this.labelStyleLeftIssue = new GUIStyle(this.labelStyleLeft)
                {
                    normal =
                    {
                        textColor = Color.yellow
                    }
                };

                this.labelStyleRight = new GUIStyle(this.labelStyleLeft)
                {
                    alignment = TextAnchor.LowerRight
                };

                this.labelStyleRightIssue = new GUIStyle(this.labelStyleRight)
                {
                    normal =
                    {
                        textColor = Color.yellow
                    }
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
                if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
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

        #region Drawing

        private void OnGUI()
        {
            if (HighLogic.LoadedScene == GameScenes.SETTINGS)
            {
                return;
            }

            if (AddonLibrary.Populated)
            {
                this.dropDownList.DrawButton("Show All KSP-AVC Ready Add-Ons", new Rect(), 400.0f);
            }
        }

        private void DrawListItems(DropDownList list)
        {
            foreach (var addon in AddonLibrary.Addons)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, addon.IsUpdateAvailable || !addon.IsCompatible ? this.labelStyleLeftIssue : this.labelStyleLeft);
                GUILayout.Label(addon.LocalInfo.Version.ToString(), addon.IsUpdateAvailable || !addon.IsCompatible ? this.labelStyleRightIssue : this.labelStyleRight);
                GUILayout.EndHorizontal();
            }
        }

        #endregion

        #region Destruction

        private void OnDestroy()
        {
            try
            {
                if (this.dropDownList != null)
                {
                    Destroy(this.dropDownList);
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