using ResoniteModLoader;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using FrooxEngine.ProtoFlux.Runtimes.Execution.Nodes.FrooxEngine.Slots;
using FrooxEngine.ProtoFlux.CoreNodes;
using FrooxEngine.ProtoFlux;
using Elements.Core;

namespace ShowObjectRootSlotNameInSubtitleOnInspectorsForSelectedSlotMod
{
    public class ShowObjectRootSlotNameInSubtitleOnInspectorsForSelectedSlotMod : ResoniteMod
    {
        public override string Name => "ShowObjectRootSlotNameInSubtitleOnInspectorsForSelectedSlotMod";
        public override string Author => "badhaloninja";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/badhaloninja/ShowObjectRootSlotNameInSubtitleOnInspectorsForSelectedSlotMod";

        public override void OnEngineInit()
        {
            Harmony harmony = new("ninja.badhalo.ShowObjectRootSlotNameInSubtitleOnInspectorsForSelectedSlotMod");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(SceneInspector), "OnAttach")]
        private class SceneInspector_OnAttach_Patch
        {
            public static void Postfix(SceneInspector __instance)
            {
                if (__instance.Slot.GetComponent<GenericUIContainer>()?.Title?.Target is not SyncField<string> TextRoot) return;

                UIBuilder ui = new(TextRoot.Slot.Parent);
                RadiantUI_Constants.SetupEditorStyle(ui);

                ui.Style.TextAutoSizeMax = 24f;
                
                var text = ui.Text(null, alignment: Alignment.BottomLeft);
                text.AlignmentMode.Value = Elements.Assets.AlignmentMode.LineBased;
                text.Slot.AttachComponent<IgnoreLayout>();

                text.RectTransform.OffsetMin.Value = new(32f, 0f);

                var flux = text.Slot.AddSlot("SelectedObjectRootFlux");

                var source = flux.AttachComponent<FrooxEngine.FrooxEngine.ProtoFlux.CoreNodes.ReferenceSource<Slot>>();
                var objroot = flux.AttachComponent<GetObjectRoot>();
                var slotName = flux.AttachComponent<GetSlotName>();
                var drive = (ProtoFluxNode)flux.AttachComponent(ProtoFluxHelper.GetDriverNode(typeof(string)));

                source.TrySetRootSource(__instance.ComponentView);
                objroot.Instance.TrySet(source);
                slotName.Instance.TrySet(objroot);
                drive.TryConnectInput(drive.GetInput(0), slotName, false, false);
                ((IDrive)drive).TrySetRootTarget(text.Content);
            }
        }
    }
}