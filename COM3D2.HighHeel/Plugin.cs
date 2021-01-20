using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;

namespace COM3D2.HighHeel
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.habeebweeb.com3d2.highheel";
        public const string PluginName = "COM3D2.HighHeel";
        public const string PluginVersion = "1.0.0";
        public const string PluginString = PluginName + " " + PluginVersion;

        private const string ConfigName = "Configuration.cfg";
        private const string DatabaseName = "database.json";

        private static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, PluginName);
        private static readonly string DatabasePath = Path.Combine(ConfigPath, DatabaseName);
        public readonly PluginConfig Configuration;
        public new readonly ManualLogSource Logger;

        private readonly UI ui = new();

        public Dictionary<string, MaidConfig> Database { get; private set; }

        public static Plugin? Instance { get; private set; }

        public Plugin()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Hooks));
            Configuration = new(new(Path.Combine(ConfigPath, ConfigName), false, Info.Metadata));
            Logger = base.Logger;

            try { Database = LoadDatabase(); }
            catch { Database = new(); }

            ui.OnReloadEvent += (_, _) =>
            {
                try { Database = ReloadDatabase(Database); }
                catch (Exception e)
                {
                    var errorVerb = e is IOException ? "load" : "parse";
                    Logger.LogWarning($"Could not {errorVerb} database because: {e.Message}");
                }
            };

            ui.OnSaveEvent += (_, _) => SaveDatabase(Database);
        }

        private void Update()
        {
            if (Configuration.UIShortcut.Value.IsUp())
            {
                ui.Visible = !ui.Visible;
                if (ui.Visible) ui.RefreshMaidList();
            }

            ui.Update();
        }

        private void OnGUI() => ui.Draw();

        private static Dictionary<string, MaidConfig> ReloadDatabase(IDictionary<string, MaidConfig> currentDb) =>
            new Dictionary<string, MaidConfig>(currentDb)
                .Concat(LoadDatabase())
                .GroupBy(kvp => kvp.Key)
                .ToDictionary(group => group.Key, group => group.Last().Value);

        private static Dictionary<string, MaidConfig> LoadDatabase()
        {
            string databaseJson = File.ReadAllText(DatabasePath);
            var database = JsonConvert.DeserializeObject<Dictionary<string, MaidConfig>>(databaseJson);

            return new(database);
        }

        private static void SaveDatabase(Dictionary<string, MaidConfig> database)
        {
            try
            {
                File.WriteAllText(DatabasePath, JsonConvert.SerializeObject(database, Formatting.Indented));
            }
            catch (Exception e)
            {
                if (Instance == null) return;

                var errorVerb = e is IOException ? "save" : "parse";
                Instance.Logger.LogWarning($"Could not {errorVerb} database because: {e.Message}");
            }
        }
    }
}
