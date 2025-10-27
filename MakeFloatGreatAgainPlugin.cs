using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace Silksong.MakeFloatGreatAgain;

[BepInAutoPlugin(id: "com.demojameson.makefloatgreatagain", name: "Make Float Great Again")]
[HarmonyPatch]
public partial class MakeFloatGreatAgainPlugin : BaseUnityPlugin {
    private static ManualLogSource logger;
    private static ConfigEntry<bool> enabled;
    private static ConfigEntry<bool> allowHorizontalInput;
    private static ConfigEntry<bool> downInput, upInput, needolinInput, quickMapInput, invertCondition;
    private Harmony harmony;

    private void Awake() {
        logger = Logger;
        enabled = Config.Bind("General",
            "Enable Float Override",
            true,
            "Whether to enable float override functionality");
        allowHorizontalInput = Config.Bind("General",
            "Allow Horizontal Input",
            false,
            "Whether to allow horizontal input to trigger floating");
        invertCondition = Config.Bind("General",
            "Invert General.Inputs checks",
            false,
            "Whether to invert the result of General.Inputs checks");
        downInput = Config.Bind("General.Inputs",
            "Hold Down",
            true,
            "Whether down input blocks double jump");
        upInput = Config.Bind("General.Inputs",
            "Hold Up",
            false,
            "Whether up input blocks double jump");
        needolinInput = Config.Bind("General.Inputs",
            "Hold Needolin",
            false,
            "Whether needolin input blocks double jump");
        quickMapInput = Config.Bind("General.Inputs",
            "Hold Quick Map",
            false,
            "Whether quick map input blocks double jump");
        harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void OnDestroy() {
        harmony.UnpatchSelf();
    }

    [HarmonyPatch(typeof(HeroController), nameof(HeroController.CanDoubleJump))]
    [HarmonyPostfix]
    private static void HeroControllerCanDoubleJump(HeroController __instance, ref bool __result) {
        if (!__result || !enabled.Value) return;

        __result = !AllowFloat(__instance);
    }

    private static bool AllowFloat(HeroController heroController) {
        var inputActions = heroController.inputHandler.inputActions;

        if(InvertCondition(
            Condition(downInput, inputActions.Down.IsPressed),
            Condition(upInput, inputActions.Up.IsPressed),
            Condition(needolinInput, inputActions.DreamNail.IsPressed),
            Condition(quickMapInput, inputActions.QuickMap.IsPressed)
        )) {
            return HorizontalCondition(inputActions);
        }

        return false;
    }

    private static bool HorizontalCondition(HeroActions inputActions) {
	    return allowHorizontalInput.Value ? true : (!inputActions.Right.IsPressed && !inputActions.Left.IsPressed);
    }

    private static bool InvertCondition(params bool[] results) {
        bool output = invertCondition.Value;
        foreach(bool result in results) {
            output = invertCondition.Value ? (output && !result) : (output || result);
        }
        return output;
    }

    private static bool Condition(ConfigEntry<bool> isRequired, bool ifTrue) {
        return isRequired.Value ? ifTrue : false;
    }
}
