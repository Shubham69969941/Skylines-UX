namespace UXMod.Keyboard {
    using Client;
    using ColossalFramework;
    using JetBrains.Annotations;
    using UnityEngine;

    /// <summary>
    /// Contains one or two SavedInputKeys, and event handler when the key is changed.
    /// Create these one per
    /// </summary>
    public class KeybindSetting {
        /// <summary>
        /// Used by the GUI to tell the button event handler which key is being edited
        /// </summary>
        public struct Editable {
            public KeybindSetting target_;
            public SavedInputKey target_key_;
        }

        /// <summary>
        /// Groups input keys by categories, also helps to know the usage for conflict search.
        /// </summary>
        public string category_;

        /// <summary>
        /// The key itself, bound to a config file value
        /// </summary>
        public SavedInputKey Key { get; }

        /// <summary>
        /// A second key, which can possibly be used or kept null
        /// </summary>
        [CanBeNull]
        public SavedInputKey AlternateKey { get; }

        private OnKeyChangedHandler onKeyChanged_;

        public delegate void OnKeyChangedHandler();

        public KeybindSetting(UXModRoot uxmod,
                              string cat,
                              string config_file_key,
                              InputKey? default_key1 = null) {
            category_ = cat;
            Key = new SavedInputKey(
                config_file_key,
                uxmod.keybinds_conf_.conf_name_,
                default_key1 ?? SavedInputKey.Empty,
                true);
        }

        public void OnKeyChanged(OnKeyChangedHandler on_changed) {
            onKeyChanged_ = on_changed;
        }

        public void NotifyKeyChanged() {
            onKeyChanged_?.Invoke();
        }

        public KeybindSetting(UXModRoot uxmod,
                              string cat,
                              string config_file_key,
                              InputKey? default_key1,
                              InputKey? default_key2) {
            category_ = cat;
            Key = new SavedInputKey(
                config_file_key,
                uxmod.keybinds_conf_.conf_name_,
                default_key1 ?? SavedInputKey.Empty,
                true);
            AlternateKey = new SavedInputKey(
                config_file_key + "_Alternate",
                uxmod.keybinds_conf_.conf_name_,
                default_key2 ?? SavedInputKey.Empty,
                true);
        }

        /// <summary>
        /// Produce a keybind tooltip text, or two if alternate key is set. Prefixed if not empty.
        /// </summary>
        /// <param name="prefix">Prefix will be added if any key is not empty</param>
        /// <returns>String tooltip with the key shortcut or two</returns>
        public string ToLocalizedString(string prefix = "") {
            var result = default(string);
            if (!Keybind.IsEmpty(Key)) {
                result += prefix + Keybind.ToLocalizedString(Key);
            }

            if (AlternateKey == null || Keybind.IsEmpty(AlternateKey)) {
                return result;
            }

            if (result.IsNullOrWhiteSpace()) {
                result += prefix;
            } else {
                result += " | ";
            }

            return result + Keybind.ToLocalizedString(AlternateKey);
        }

        public bool IsPressed(Event e) {
            return Key.IsPressed(e)
                   || (AlternateKey != null && AlternateKey.IsPressed(e));
        }

        /// <summary>
        /// Check whether main or alt key are the same as k
        /// </summary>
        /// <param name="k">Find key</param>
        /// <returns>We have the key</returns>
        public bool HasKey(InputKey k) {
            return Key.value == k
                   || (AlternateKey != null && AlternateKey.value == k);
        }
    }
}