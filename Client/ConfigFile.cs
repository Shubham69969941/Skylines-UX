namespace UXMod.Client {
    using System;
    using ColossalFramework;

    /// <summary>
    /// This class ensures that configuration file is created and exists for
    /// loading/saving client-specific values such as keybinds.
    /// </summary>
    public class ConfigFile {
        private readonly UXModRoot uxMod_;

        internal readonly string conf_name_;

        public ConfigFile(UXModRoot uxmod, string conf_name) {
            conf_name_ = conf_name;
            uxMod_ = uxmod;
            TryCreateConfig();
        }

        private void TryCreateConfig() {
            if (string.IsNullOrEmpty(uxMod_.owner_name_)) {
                return;
            }

            try {
                // Creating setting file for the keyboard settings
                if (GameSettings.FindSettingsFileByName(conf_name_) == null) {
                    GameSettings.AddSettingsFile(new SettingsFile {fileName = conf_name_});
                }
            }
            catch (Exception) {
                uxMod_.log_("Could not load/create the keyboard shortcuts file.");
            }
        }
    }
}