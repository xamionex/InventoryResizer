using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;

namespace InventoryResizer;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInProcess("Darkwood.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static PluginInfo pluginInfo;
    public static ConfigFile ConfigFile;
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "amione";
    public const string PluginName = "InventoryResizer";
    public const string PluginVersion = "1.0.1";
    public static ManualLogSource Log;
    public static FileSystemWatcher fileWatcher;
    public static ConfigEntry<int> RightSlots;
    public static ConfigEntry<int> DownSlots;
    public static ConfigEntry<int> InventoryRightSlots;
    public static ConfigEntry<int> InventoryDownSlots;
    public static ConfigEntry<bool> InventorySlots;
    public static ConfigEntry<bool> RemoveExcess;

    private void Awake()
    {
        pluginInfo = Info;
        Log = Logger;
        ConfigFile = new ConfigFile(System.IO.Path.Combine(Paths.ConfigPath, PluginGUID + ".cfg"), true);

        RightSlots = ConfigFile.Bind($"{PluginName}", "Storage Right Slots", 12, "Number that determines slots in workbench to the right");
        DownSlots = ConfigFile.Bind($"{PluginName}", "Storage Down Slots", 9, "Number that determines slots in workbench downward");
        InventoryRightSlots = ConfigFile.Bind($"{PluginName}", "Inventory Right Slots", 2, "Number that determines slots in inventory to the right");
        InventoryDownSlots = ConfigFile.Bind($"{PluginName}", "Inventory Down Slots", 9, "Number that determines slots in inventory downward");
        InventorySlots = ConfigFile.Bind($"{PluginName}", "Enable Inventory Changing", false, "This will circumvent the inventory progression and enables the 2 settings above this one to take effect");
        RemoveExcess = ConfigFile.Bind($"{PluginName}", "Remove Excess Slots", true, "Whether or not to remove slots that are outside the inventory you set. For example, you set your inventory to 9x9 (81 slots) but you had a previous mod do something bigger and you have something like 128 slots extra enabling this option will remove those excess slots and bring it down to 9x9 (81)");

        Harmony Harmony = new Harmony($"{PluginGUID}");
        Harmony.PatchAll(typeof(InventoryPatch));
        Log.LogInfo($"Plugin {PluginGUID} v{PluginVersion} is loaded!");

        fileWatcher = new FileSystemWatcher(Paths.ConfigPath, PluginGUID + ".cfg");
        fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
        fileWatcher.Changed += OnFileChanged;
        ConfigFile.ConfigReloaded += OnConfigReloaded;
        fileWatcher.EnableRaisingEvents = true;
    }

    private void OnConfigReloaded(object sender, EventArgs e)
    {
        Log.LogInfo($"Reloaded configuration file");
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        ConfigFile.Reload();
    }
}
