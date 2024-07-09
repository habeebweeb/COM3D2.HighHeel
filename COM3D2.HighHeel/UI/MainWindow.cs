using UnityEngine;

using ShoeConfigParameter = COM3D2.HighHeel.Core.ShoeConfig.ShoeConfigParameter;

namespace COM3D2.HighHeel.UI;

public class MainWindow
{
    public bool Visible;

    private const int WindowId = 523961;

    private static readonly GUILayoutOption NoExpand = GUILayout.ExpandWidth(false);
    private static readonly GUILayoutOption ConfigNameLayout = GUILayout.Width(140f);

    private static readonly GUIContent EnabledLabel = new(Plugin.PluginString);
    private static readonly GUIContent CloseLabel = new("x");
    private static readonly GUIContent ReloadLabel = new("Reload");
    private static readonly GUIContent EditModeLabel = new("Edit Mode");
    private static readonly GUIContent ConfigPrefixLabel = new("hhmod_");
    private static readonly GUIContent ExportLabel = new("Export");
    private static readonly GUIContent ImportLabel = new("Import");

    private readonly Dictionary<ShoeConfigParameter, NumberInput> inputs = [];

    private Core.ShoeConfig editModeConfig;

    private Vector2 scrollPos = Vector2.zero;
    private Rect windowRect = new(20f, 20f, 260f, 265f);

    private string configName = string.Empty;

    private GUIStyle? windowStyle;

    public MainWindow()
    {
        editModeConfig = Plugin.Instance!.EditModeConfig;
        inputs[ShoeConfigParameter.BodyOffset] = new("Body Offset", editModeConfig.BodyOffset);
        inputs[ShoeConfigParameter.BodyOffset].InputChangeEvent += (_, a) => editModeConfig.BodyOffset = a.Value;

        inputs[ShoeConfigParameter.FootLAngle] = new("Foot L Angle", editModeConfig.FootLAngle);
        inputs[ShoeConfigParameter.FootLAngle].InputChangeEvent += (_, a) => editModeConfig.FootLAngle = a.Value;
        inputs[ShoeConfigParameter.FootLMax] = new("Foot L Max", editModeConfig.FootLMax);
        inputs[ShoeConfigParameter.FootLMax].InputChangeEvent += (_, a) => editModeConfig.FootLMax = a.Value;
        inputs[ShoeConfigParameter.ToeLAngle] = new("Toe L Angle", editModeConfig.ToeLAngle);
        inputs[ShoeConfigParameter.ToeLAngle].InputChangeEvent += (_, a) => editModeConfig.ToeLAngle = a.Value;

        inputs[ShoeConfigParameter.ToeL0Angle] = new("Toe L 0 Angle", editModeConfig.ToeL0Angle);
        inputs[ShoeConfigParameter.ToeL0Angle].InputChangeEvent += (_, a) => editModeConfig.ToeL0Angle = a.Value;
        inputs[ShoeConfigParameter.ToeL01Angle] = new("Toe L 01 Angle", editModeConfig.ToeL01Angle);
        inputs[ShoeConfigParameter.ToeL01Angle].InputChangeEvent += (_, a) => editModeConfig.ToeL01Angle = a.Value;
        inputs[ShoeConfigParameter.ToeL1Angle] = new("Toe L 1 Angle", editModeConfig.ToeL1Angle);
        inputs[ShoeConfigParameter.ToeL1Angle].InputChangeEvent += (_, a) => editModeConfig.ToeL1Angle = a.Value;
        inputs[ShoeConfigParameter.ToeL11Angle] = new("Toe L 11 Angle", editModeConfig.ToeL11Angle);
        inputs[ShoeConfigParameter.ToeL11Angle].InputChangeEvent += (_, a) => editModeConfig.ToeL11Angle = a.Value;
        inputs[ShoeConfigParameter.ToeL2Angle] = new("Toe L 2 Angle", editModeConfig.ToeL2Angle);
        inputs[ShoeConfigParameter.ToeL2Angle].InputChangeEvent += (_, a) => editModeConfig.ToeL2Angle = a.Value;
        inputs[ShoeConfigParameter.ToeL21Angle] = new("Toe L 21 Angle", editModeConfig.ToeL21Angle);
        inputs[ShoeConfigParameter.ToeL21Angle].InputChangeEvent += (_, a) => editModeConfig.ToeL21Angle = a.Value;

        inputs[ShoeConfigParameter.FootRAngle] = new("Foot R Angle", editModeConfig.FootRAngle);
        inputs[ShoeConfigParameter.FootRAngle].InputChangeEvent += (_, a) => editModeConfig.FootRAngle = a.Value;
        inputs[ShoeConfigParameter.FootRMax] = new("Foot R Max", editModeConfig.FootRMax);
        inputs[ShoeConfigParameter.FootRMax].InputChangeEvent += (_, a) => editModeConfig.FootRMax = a.Value;
        inputs[ShoeConfigParameter.ToeRAngle] = new("Toe R Angle", editModeConfig.ToeRAngle);
        inputs[ShoeConfigParameter.ToeRAngle].InputChangeEvent += (_, a) => editModeConfig.ToeRAngle = a.Value;

        inputs[ShoeConfigParameter.ToeR0Angle] = new("Toe R 0 Angle", editModeConfig.ToeR0Angle);
        inputs[ShoeConfigParameter.ToeR0Angle].InputChangeEvent += (_, a) => editModeConfig.ToeR0Angle = a.Value;
        inputs[ShoeConfigParameter.ToeR01Angle] = new("Toe R 01 Angle", editModeConfig.ToeR01Angle);
        inputs[ShoeConfigParameter.ToeR01Angle].InputChangeEvent += (_, a) => editModeConfig.ToeR01Angle = a.Value;
        inputs[ShoeConfigParameter.ToeR1Angle] = new("Toe R 1 Angle", editModeConfig.ToeR1Angle);
        inputs[ShoeConfigParameter.ToeR1Angle].InputChangeEvent += (_, a) => editModeConfig.ToeR1Angle = a.Value;
        inputs[ShoeConfigParameter.ToeR11Angle] = new("Toe R 11 Angle", editModeConfig.ToeR11Angle);
        inputs[ShoeConfigParameter.ToeR11Angle].InputChangeEvent += (_, a) => editModeConfig.ToeR11Angle = a.Value;
        inputs[ShoeConfigParameter.ToeR2Angle] = new("Toe R 2 Angle", editModeConfig.ToeR2Angle);
        inputs[ShoeConfigParameter.ToeR2Angle].InputChangeEvent += (_, a) => editModeConfig.ToeR2Angle = a.Value;
        inputs[ShoeConfigParameter.ToeR21Angle] = new("Toe R 21 Angle", editModeConfig.ToeR21Angle);
        inputs[ShoeConfigParameter.ToeR21Angle].InputChangeEvent += (_, a) => editModeConfig.ToeR21Angle = a.Value;
    }

    public event EventHandler? ReloadEvent;

    public event EventHandler<TextInputEventArgs>? ExportEvent;

    public event EventHandler<TextInputEventArgs>? ImportEvent;

    private GUIStyle WindowStyle =>
        windowStyle ??= new(GUI.skin.box);

    public void UpdateEditModeValues()
    {
        editModeConfig = Plugin.Instance!.EditModeConfig;
        inputs[ShoeConfigParameter.BodyOffset].Value = editModeConfig.BodyOffset;

        inputs[ShoeConfigParameter.FootLAngle].Value = editModeConfig.FootLAngle;
        inputs[ShoeConfigParameter.FootLMax].Value = editModeConfig.FootLMax;
        inputs[ShoeConfigParameter.ToeLAngle].Value = editModeConfig.ToeLAngle;

        inputs[ShoeConfigParameter.ToeL0Angle].Value = editModeConfig.ToeL0Angle;
        inputs[ShoeConfigParameter.ToeL01Angle].Value = editModeConfig.ToeL01Angle;
        inputs[ShoeConfigParameter.ToeL1Angle].Value = editModeConfig.ToeL1Angle;
        inputs[ShoeConfigParameter.ToeL11Angle].Value = editModeConfig.ToeL11Angle;
        inputs[ShoeConfigParameter.ToeL2Angle].Value = editModeConfig.ToeL2Angle;
        inputs[ShoeConfigParameter.ToeL21Angle].Value = editModeConfig.ToeL21Angle;

        inputs[ShoeConfigParameter.FootRAngle].Value = editModeConfig.FootRAngle;
        inputs[ShoeConfigParameter.FootRMax].Value = editModeConfig.FootRMax;
        inputs[ShoeConfigParameter.ToeRAngle].Value = editModeConfig.ToeRAngle;

        inputs[ShoeConfigParameter.ToeR0Angle].Value = editModeConfig.ToeR0Angle;
        inputs[ShoeConfigParameter.ToeR01Angle].Value = editModeConfig.ToeR01Angle;
        inputs[ShoeConfigParameter.ToeR1Angle].Value = editModeConfig.ToeR1Angle;
        inputs[ShoeConfigParameter.ToeR11Angle].Value = editModeConfig.ToeR11Angle;
        inputs[ShoeConfigParameter.ToeR2Angle].Value = editModeConfig.ToeR2Angle;
        inputs[ShoeConfigParameter.ToeR21Angle].Value = editModeConfig.ToeR21Angle;
    }

    public void Update()
    {
        if (!Visible || Input.mouseScrollDelta.y == 0f)
            return;

        var mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

        if (windowRect.Contains(mousePos))
            Input.ResetInputAxes();
    }

    public void Draw()
    {
        if (!Visible)
            return;

        windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 20, Screen.width - 20);
        windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 20, Screen.height - 20);

        var pluginEnabled = Plugin.Instance!.Configuration.Enabled.Value;
        var editModeEnabled = Plugin.Instance!.EditMode;
        windowRect.height = pluginEnabled && editModeEnabled ? 280f : 55f;

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
            if (configPluginEnabled)
                plugin.EditMode = false;

            plugin.Configuration.Enabled.Value = pluginEnabled;
        }

        GUI.enabled = pluginEnabled;

        if (GUILayout.Button(ReloadLabel))
            ReloadEvent?.Invoke(this, EventArgs.Empty);

        GUILayout.FlexibleSpace();

        GUI.enabled = true;

        if (GUILayout.Button(CloseLabel, NoExpand))
            Visible = false;

        GUILayout.EndHorizontal();
    }

    private void DrawEditMode()
    {
        var plugin = Plugin.Instance!;

        GUI.enabled = plugin.Configuration.Enabled.Value;

        plugin.EditMode = GUILayout.Toggle(plugin.EditMode, EditModeLabel);

        if (!plugin.EditMode)
            return;

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var input in inputs.Values)
            input.Draw();

        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();

        GUILayout.Label(ConfigPrefixLabel, NoExpand);

        configName = GUILayout.TextField(configName, ConfigNameLayout);

        if (GUILayout.Button(ExportLabel, NoExpand))
            ExportEvent?.Invoke(this, new(configName));

        if (GUILayout.Button(ImportLabel, NoExpand))
            ImportEvent?.Invoke(this, new(configName));

        GUILayout.EndHorizontal();
    }
}
