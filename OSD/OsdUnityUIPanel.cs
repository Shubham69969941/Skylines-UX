namespace UXMod.OSD {
    using Client;
    using ColossalFramework.UI;

    public class OsdUnityUIPanel : UIPanel {
        protected override void OnPositionChanged() {
            var uxmod = this.objectUserData as UXModRoot;
            if (uxmod == null) {
                // the objectUserData field contains something wrong
                return;
            }

            bool pos_changed = uxmod.OSDPanelX != (int)absolutePosition.x
                              || uxmod.OSDPanelY != (int)absolutePosition.y;

            if (pos_changed) {
                uxmod.log_($"OSD position changed to {absolutePosition.x}|{absolutePosition.y}");

                uxmod.OSDPanelX.value = (int)absolutePosition.x;
                uxmod.OSDPanelY.value = (int)absolutePosition.y;
            }

            base.OnPositionChanged();
        }
    }
}