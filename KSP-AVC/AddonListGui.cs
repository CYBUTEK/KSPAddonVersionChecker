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

using System;

using UnityEngine;

namespace KSP_AVC
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class AddonListGui : MonoBehaviour
    {
        private readonly Rect dropDownPosition = new Rect();
        private DropDownList dropDownList;

        private GUIStyle labelStyleLeft;
        private GUIStyle labelStyleLeftIssue;
        private GUIStyle labelStyleRight;
        private GUIStyle labelStyleRightIssue;

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
            Logger.Log("AddonListGui was created.");
        }

        protected void OnDestroy()
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
            Logger.Log("AddonListGui was destroyed.");
        }

        protected void OnGUI()
        {
            try
            {
                if (HighLogic.LoadedScene == GameScenes.SETTINGS)
                {
                    return;
                }

                this.dropDownList.DrawButton("Show All KSP-AVC Ready Add-Ons", this.dropDownPosition, 400.0f);
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
                this.CreateDropDownList();
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

        private void CreateDropDownList()
        {
            this.dropDownList = this.gameObject.AddComponent<DropDownList>();
            this.dropDownList.DrawCallback = this.DrawListItems;
        }

        private void DrawListItems(DropDownList list)
        {
            if (AddonLibrary.Addons == null)
            {
                return;
            }

            foreach (var addon in AddonLibrary.Addons)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(addon.Name, addon.IsUpdateAvailable || !addon.IsCompatible ? this.labelStyleLeftIssue : this.labelStyleLeft);
                GUILayout.Label(addon.LocalInfo.Version.ToString(), addon.IsUpdateAvailable || !addon.IsCompatible ? this.labelStyleRightIssue : this.labelStyleRight);
                GUILayout.EndHorizontal();
            }
        }

        private void InitialiseStyles()
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
    }
}