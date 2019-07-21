namespace UXMod.API {
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using ColossalFramework;
    using ICities;
    using JetBrains.Annotations;

    /// <summary>
    /// Copy this file to your project. Use UXModAPI to find a running UXMod and use its API.
    /// It can be a standalone UXMod, or UXMod installed from the Workshop, or a copy of UXMod
    /// included in Traffic Manager: President Edition (with a different namespace).
    /// </summary>
    [UsedImplicitly]
    public class UXModAPI {
        /// <summary>
        /// Log function provided by the UXMod client, will log into their log file or do nothing
        /// </summary>
        private Action<string> log_func_;

        /// <summary>
        /// This will contain type for the ClientMod from the UXMod assembly.
        /// </summary>
        private Type uxmod_type_;

        /// <summary>
        /// Contains UXMod.Client.ClientMod (if binding succeeded)
        /// </summary>
        public object client_mod_;

        public Type client_mod_type_;

        public UXModAPI(Action<string> log_func) {
            log_func_ = log_func ?? (s => { });

            // Try find UXMod assembly and prefer it if it was a success
            var uxmod_asm = FindNamespace("UXMod.Server");
            log_func_(uxmod_asm != null
                          ? $"UXMod: Detected UXMod in {uxmod_asm}"
                          : "UXMod assembly was not found");
            if (Internal_TryBind(uxmod_asm, "UXMod.Client.ClientMod")) {
                return;
            }

            // TM:PE assembly might contain UXMod, will be used if UXMod is not present
            var tmpe_asm = FindNamespace("TrafficManager");
            log_func_(tmpe_asm != null
                          ? $"UXMod: Detected TM:PE in {tmpe_asm}"
                          : "TM:PE assembly was not found");

            if (Internal_TryBind(tmpe_asm, "TrafficManager.UXMod.Client.ClientMod")) {
                return;
            }

            log_func_($"UXMod: Failed to bind, UXMod will not be available");
        }

        /// <summary>
        /// Tries to find compatible UXMod API entry points
        /// </summary>
        /// <param name="assembly">The assembly to try</param>
        /// <returns></returns>
        public bool Internal_TryBind(Assembly assembly, string client_mod_ns) {
            try {
                client_mod_type_ = assembly.GetType(client_mod_ns);
                if (client_mod_type_ == null) {
                    return false;
                }

                client_mod_ = Activator.CreateInstance( client_mod_type_, "TMPE");
                return true;
            }
            catch (Exception e) {
                log_func_($"UXMod: Binding failed with {e}");
            }

            return false;
        }

        private Assembly FindNamespace(string namespace_str) {
            var lext_field = typeof(LoadingWrapper).GetField(
                "m_LoadingExtensions",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (lext_field == null) {
                log_func_("UXMod: Could not get loading extensions field");
                return null;
            }

            var loading_extensions = (List<ILoadingExtension>) lext_field
                .GetValue(Singleton<LoadingManager>.instance.m_LoadingWrapper);

            if (loading_extensions != null) {
                foreach (var extension in loading_extensions) {
                    var ext_type = extension.GetType();
                    if (ext_type.Namespace == null) {
                        continue;
                    }

                    var ns_str = ext_type.Namespace;
                    if (namespace_str.Equals(ns_str)) {
                        return ext_type.Assembly;
                    }
                }
            } else {
                log_func_("UXMod: Could not get loading extensions");
            }

            return null;
        }

        /// <summary>
        /// Stores a reference to keybind setting, which is a dual keyboard/mouse
        /// setting which can also be edited in your Mod Options with the help
        /// of KeybindSettingsPageAPI.
        /// </summary>
        public class KeybindSettingAPI {
            private object uxmod_;
            private object keybindSetting_;
        }

        /// <summary>
        /// Allows to create a panel or page in your Mod Options and fill it with
        /// controls to edit/delete the keybinds.
        /// </summary>
        [UsedImplicitly]
        public class KeybindSettingsPageAPI {
            private object uxmod_;

            public string ToInternationalizedString() {
                return "?";
            }

            public bool IsPressed() {
                return false;
            }
        }

        /// <summary>
        /// Stores a reference to the OSD panel and can send commands to it.
        /// </summary>
        [UsedImplicitly]
        public class OSDPanelAPI {
            private object panel_;

            public OSDPanelAPI(object clientMod) {
                panel_ = panel;
                panel_.Clear();
            }

            public OSDPanelAPI Title(string text) {
                panel_.Title = text;
                return this;
            }

            public OSDPanelAPI Shortcut(string text, KeybindSettingAPI setting) {
                panel_.items_.Add(new OsdItem_Shortcut(text, setting));
                return this;
            }

            /// <summary>
            /// Final call in the setup chain, refreshes the OSD panel with new content
            /// </summary>
            public void Show() {
                panel_.Update();
                panel_ = null; // end of work for the configurator object
            }
        }

        [UsedImplicitly]
        public Configurator SetOSD() {
            return new Configurator(this.client_mod_);
//            client_mod_type_?.InvokeMember(
//                    "Hello",
//                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
//                    null,
//                    client_mod_,
//                    new object[] { s });
        }
    }
}