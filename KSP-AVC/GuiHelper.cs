using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Helper class to keep up an update method which checks for the key combination needed to open the GUI
namespace KSP_AVC
{
    public class GuiHelper : MonoBehaviour
    {
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
            this.name = "GuiHelper";
            Logger.Log("GuiHelper was created");
        }

        protected void Update()
        {
            bool modKey = GameSettings.MODIFIER_KEY.GetKey(); 
            if (modKey && Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToggleGUI();
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                Destroy(this);
            }
        }

        protected void OnDestroy()
        {
            Logger.Log("GuiHelper was destroyed");
        }

        public void ToggleGUI()
        {
            if (!this.GetComponent<CompatibilityOverrideGui>())
            {
                this.gameObject.AddComponent<CompatibilityOverrideGui>();
                return;
            }
            else
            {
                Destroy(this.GetComponent<CompatibilityOverrideGui>());
            }
        }
    }
}
