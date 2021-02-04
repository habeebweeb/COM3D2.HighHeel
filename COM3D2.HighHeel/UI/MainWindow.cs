using System;
using System.Collections.Generic;
using UnityEngine;

namespace COM3D2.HighHeel.UI
{
    using static Core.ShoeConfig;

    public class MainWindow
    {
        private const int WindowId = 523961;

        private static readonly GUILayoutOption NoExpand = GUILayout.ExpandWidth(false);
        private static readonly GUILayoutOption ConfigNameLayout = GUILayout.Width(140f);

        private static readonly GUIContent EnabledLabel = new(Plugin.PluginString);
        private static readonly GUIContent CloseLabel = new("x");
        private static readonly GUIContent ReloadLabel = new("Reload");
        private static readonly GUIContent EditModeLabel = new("Edit Mode");
        private static readonly GUIContent ConfigPrefixLabel = new("hhmod_");
        private static readonly GUIContent ExportLabel = new("Export");

        private readonly Dictionary<ShoeConfigParameter, NumberInput> inputs = new();

        private Vector2 scrollPos = Vector2.zero;
        private Rect windowRect = new(20f, 20f, 260f, 265f);
        public bool Visible;

        private string configName = string.Empty;

        private GUIStyle? windowStyle;
        private GUIStyle WindowStyle => windowStyle ??= new(GUI.skin.box);

        public event EventHandler? ReloadEvent;
        public event EventHandler<TextInputEventArgs>? ExportEvent;

        public MainWindow()
        {
            var editModeConfig = Plugin.Instance!.EditModeConfig;            
            inputs[ShoeConfigParameter.BodyOffset] = new("Body Offset", editModeConfig.BodyOffset);
            inputs[ShoeConfigParameter.BodyOffset].InputChangeEvent += (_, a) => editModeConfig.BodyOffset = a.Value;
            inputs[ShoeConfigParameter.FootLAngle] = new("Foot L Angle", editModeConfig.FootLAngle);
            inputs[ShoeConfigParameter.FootLAngle].InputChangeEvent += (_, a) => editModeConfig.FootLAngle = a.Value;
            inputs[ShoeConfigParameter.FootLMax] = new("Foot L Max", editModeConfig.FootLMax);
            inputs[ShoeConfigParameter.FootLMax].InputChangeEvent += (_, a) => editModeConfig.FootLMax = a.Value;
            inputs[ShoeConfigParameter.ToeLAngle] = new("Toe L Angle", editModeConfig.ToeLAngle);
            inputs[ShoeConfigParameter.ToeLAngle].InputChangeEvent += (_, a) => editModeConfig.ToeLAngle = a.Value;
            inputs[ShoeConfigParameter.FootRAngle] = new("Foot R Angle", editModeConfig.FootRAngle);
            inputs[ShoeConfigParameter.FootRAngle].InputChangeEvent += (_, a) => editModeConfig.FootRAngle = a.Value;
            inputs[ShoeConfigParameter.FootRMax] = new("Foot R Max", editModeConfig.FootRMax);
            inputs[ShoeConfigParameter.FootRMax].InputChangeEvent += (_, a) => editModeConfig.FootRMax = a.Value;
            inputs[ShoeConfigParameter.ToeRAngle] = new("Toe R Angle", editModeConfig.ToeRAngle);
            inputs[ShoeConfigParameter.ToeRAngle].InputChangeEvent += (_, a) => editModeConfig.ToeRAngle = a.Value;
        }

        public void Update()
        {
            if (!Visible || Input.mouseScrollDelta.y == 0f) return;

            var mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            if (windowRect.Contains(mousePos)) Input.ResetInputAxes();
        }

        public void Draw()
        {
            if (!Visible) return;

            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 20, Screen.width - 20);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 20, Screen.height - 20);

            var pluginEnabled = Plugin.Instance!.Configuration.Enabled.Value;
            var editModeEnabled = Plugin.Instance!.EditMode;
            windowRect.height = pluginEnabled && editModeEnabled ? 265f : 55f;

            windowRect = GUILayout.Window(WindowId, windowRect, GuiFunc, string.Empty, WindowStyle);
        }

        private void GuiFunc(int windowId)
        {
            DrawTitlebar();
            DrawEditMode();

            GUI.enabled = true;
            GUI.DragWindow();
        }

        private void DrawTitlebar()
        {
            var plugin = Plugin.Instance!;

            GUILayout.BeginHorizontal();

            var configPluginEnabled = plugin.Configuration.Enabled.Value;

            var pluginEnabled = GUILayout.Toggle(configPluginEnabled, EnabledLabel);

            if (pluginEnabled != configPluginEnabled)
            {
                if (configPluginEnabled) plugin.EditMode = false;
                plugin.Configuration.Enabled.Value = pluginEnabled;
            }

            GUI.enabled = pluginEnabled;

            if (GUILayout.Button(ReloadLabel)) ReloadEvent?.Invoke(this, EventArgs.Empty);

            GUILayout.FlexibleSpace();

            GUI.enabled = true;

            if (GUILayout.Button(CloseLabel, NoExpand)) Visible = false;

            GUILayout.EndHorizontal();
        }

        private void DrawEditMode()
        {
            var plugin = Plugin.Instance!;

            GUI.enabled = plugin.Configuration.Enabled.Value;

            plugin.EditMode = GUILayout.Toggle(plugin.EditMode, EditModeLabel);

            if (!plugin.EditMode) return;

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (var input in inputs.Values) input.Draw();

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            
            GUILayout.BeginHorizontal();

            GUILayout.Label(ConfigPrefixLabel, NoExpand);

            configName = GUILayout.TextField(configName, ConfigNameLayout);

            if (GUILayout.Button(ExportLabel, NoExpand)) ExportEvent?.Invoke(this, new(configName));
            
            GUILayout.EndHorizontal();
        }
    }
}
