using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using InventoryResizer;
using UnityEngine;

[HarmonyPatch]
public class InventoryPatch
{
	[HarmonyPatch(typeof(Inventory), nameof(Inventory.show))]
	[HarmonyTranspiler]
	public static IEnumerable<CodeInstruction> InventoryShow(IEnumerable<CodeInstruction> instructions)
	{
		var codes = new List<CodeInstruction>(instructions);
		for (int i = 0; i < codes.Count; i++)
		{
			// replaces 670 offset for workbench crafting with 1000
			if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand.ToString() == "670")
			{
				codes[i] = new CodeInstruction(OpCodes.Ldc_R4, Plugin.WorkbenchCraftingOffset.Value);
				break;
			}
		}
		return codes;
	}

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.show))]
    [HarmonyPrefix]
    public static void AddSlots(Inventory __instance, string labelName = "")
    {
        if (Plugin.InventorySlots.Value && __instance.invType == Inventory.InvType.playerInv)
        {
			__instance.maxColumns = Plugin.InventoryRightSlots.Value;
			if (__instance.slots.Count != Plugin.InventoryRightSlots.Value*Plugin.InventoryDownSlots.Value)
			{
				ModifySlots(__instance, __instance.maxColumns*Plugin.InventoryDownSlots.Value);
			}
        }
        if (labelName == "Storage")
        {
			__instance.maxColumns = Plugin.RightSlots.Value;
			if (__instance.slots.Count != Plugin.RightSlots.Value*Plugin.DownSlots.Value)
			{
				ModifySlots(__instance, __instance.maxColumns*Plugin.DownSlots.Value);
			}
        }
    }

	public static void ModifySlots(Inventory __instance, int MaximumSlots)
	{
		Plugin.Log.LogInfo($"Maximum slots allowed is {MaximumSlots} and we have {__instance.slots.Count}");
		if (MaximumSlots < __instance.slots.Count && Plugin.RemoveExcess.Value)
		{
			Plugin.Log.LogInfo($"Removing {__instance.slots.Count - MaximumSlots} slots");
			for (int i = __instance.slots.Count; i > MaximumSlots; i--)
			{
				__instance.slots.RemoveAt(i-1);
			}
		} else {
			Plugin.Log.LogInfo($"Adding {MaximumSlots - __instance.slots.Count} slots");
			for (int i = __instance.slots.Count; i < MaximumSlots; i++)
			{
				__instance.slots.Add(new InvSlot());
			}
		}
		return;
	}
}