using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;

namespace Silksong.MakeFloatGreatAgain;

[BepInAutoPlugin(id: "demojameson.silksong.makefloatgreatagain", name: "Make Float Great Again")]
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
            "Float Override Input",
            true,
            "Whether to enable float override input");
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
            "Hold Down",
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
    [HarmonyILManipulator]
    private static void IlCanDoubleJump(ILContext ctx) {
        var ilCursor = new ILCursor(ctx);
        // if (playerData.hasDoubleJump && !doubleJumped && !IsDashLocked() && !cState.wallSliding && !cState.backDashing && !IsAttackLocked() && !cState.bouncing && !cState.shroomBouncing && !cState.onGround && !cState.doubleJumping && Config.CanDoubleJump)
        // ↓
        // if (playerData.hasDoubleJump && AllowDoubleJump() && !doubleJumped...
        if (!ilCursor.TryGotoNext(MoveType.After,
                instruction => instruction.OpCode == OpCodes.Ldfld &&
                               instruction.Operand.ToString().EndsWith(nameof(PlayerData.hasDoubleJump)))) {
            logger.LogWarning("Failed to find instruction in HeroController.CanDoubleJump()");
            return;
        }

        ilCursor.Emit(OpCodes.Ldarg_0).EmitDelegate<Func<bool, HeroController, bool>>(AllowDoubleJump);
    }

    private static bool AllowDoubleJump(bool hasDoubleJump, HeroController heroController) {
        if (!enabled.Value) {
            return hasDoubleJump;
        }

        var inputActions = heroController.inputHandler.inputActions;

        return hasDoubleJump && !(
            HorizontalCondition(inputActions) &&
            InvertCondition(
                Condition(downInput, inputActions.Down.IsPressed) ||
                Condition(upInput, inputActions.Up.IsPressed) ||
                Condition(needolinInput, inputActions.DreamNail.IsPressed) ||
                Condition(quickMapInput, inputActions.QuickMap.IsPressed)
            )
        );
    }

    private static bool HorizontalCondition(HeroActions inputActions) {
        return allowHorizontalInput.Value ? true : (!inputActions.Right.IsPressed && !inputActions.Left.IsPressed);
    }

    private static bool InvertCondition(bool result) {
        return invertCondition.Value ? !result : result;
    }

    private static bool Condition(bool isRequired, bool ifTrue) {
        return isRequired ? ifTrue : true;
    }
}
