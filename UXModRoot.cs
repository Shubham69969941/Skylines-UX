namespace UXMod.Client {
    using System;
    using System.Collections.Generic;
    using ColossalFramework;
    using JetBrains.Annotations;
    using OSD;
    using UnityEngine;

    /// <summary>
    /// A root object which contains everything that UXMod needs to function.
    /// Create one UXModRoot class object once for your own mod.
    ///
    /// It should be OK to create UXModRoot in place every time you need to reset the OSD panel,
    /// but there is time cost to it.
    /// </summary>
    [UsedImplicitly]
    public class UXModRoot {
        internal const string UXMOD_PANEL_NAME = "UXMod_OSD_Panel";

        /// <summary>
        /// Client mod using the UXMod gives their custom string for use as config
        /// filename prefix. Setting this to empty disables the keybinds config.
        /// The config filename becomes "UXMod_<prefix>_Keybinds.cgs" in
        /// "AppData/Local/Colossal Order/Cities_Skylines"
        /// </summary>
        internal readonly string owner_name_;

        internal ConfigFile keybinds_conf_;
        internal ConfigFile uxmod_conf_;

        internal Action<string> log_;

        /// <summary>
        /// The scene graph is searched for the UXMod panel, if necessary we can also create and
        /// manage it if no other mod did.
        /// The panel variable can be in 2 states:
        /// 1. Panel is found and is owned by someone else (GameObject)
        /// 2. Panel is created and owned by us (OSDPanel class)
        /// </summary>
        private Either<GameObject, OnScreenDisplayPanel> panel_;

        internal SavedBool OSDPanelVisible;
        internal SavedInt OSDPanelX;
        internal SavedInt OSDPanelY;

        public UXModRoot(string owner_name, Action<string> log_fun) {
            owner_name_ = owner_name;
            log_ = log_fun;

            keybinds_conf_ = new ConfigFile(this, $"{owner_name_}_Keybinds");

            uxmod_conf_ = new ConfigFile(this, "UXMod_Settings");
            OSDPanelVisible = new SavedBool("OSDPanelVisible", uxmod_conf_.conf_name_);
            OSDPanelX = new SavedInt("OSDPanelX", uxmod_conf_.conf_name_);
            OSDPanelY = new SavedInt("OSDPanelY", uxmod_conf_.conf_name_);

            // Look for the panel by name
            var o = GameObject.Find(UXMOD_PANEL_NAME);
            panel_ = o != null
                         ? new Either<GameObject, OnScreenDisplayPanel>(o)
                         : new Either<GameObject, OnScreenDisplayPanel>(
                             new OnScreenDisplayPanel(this));
        }

        /// <summary>
        /// Set up a new text and actions to be displayed on the OSD panel
        /// </summary>
        /// <returns></returns>
        [UsedImplicitly]
        public OsdConfigurator OSD() {
            return new OsdConfigurator(this);
        }

        /// <summary>
        /// Called from OSD configurator to deliver the command to the panel
        /// </summary>
        internal void SendUpdateOsdMessage(List<List<object>> commands) {
            if (panel_.IsLeft) {
                // Foreign-owned panel, maybe created by some other mod
                panel_.Left.SendMessage(
                    "UpdateOsd",
                    commands,
                    SendMessageOptions.DontRequireReceiver);
            } else {
                // This panel is owned by this mod
                panel_.Right.Reprogram(commands);
            }
        }
    }
}
