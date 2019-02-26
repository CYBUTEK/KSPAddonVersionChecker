using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//Helper class to keep up an update method which checks for the key combination needed to open the GUI
namespace KSP_AVC
{
    public class OverrideHelper : MonoBehaviour
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
            Logger.Log("OverrideHelper was created");
        }

        protected void Update()
        {
            bool modKey = (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
            if (modKey && Input.GetKeyDown(KeyCode.Insert))
            {
                if (this.GetComponent<ForceCompatibilityGui>() == null)
                {
                    this.gameObject.AddComponent<ForceCompatibilityGui>();
                    Logger.Log("Display ForceCompatibilityGui");
                    return;
                }
                else
                {
                    Destroy(this.GetComponent<ForceCompatibilityGui>());
                }
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                Destroy(this);
            }
        }

        protected void OnDestroy()
        {
            Logger.Log("OverrideHelper was destroyed");
        }
    }
}
