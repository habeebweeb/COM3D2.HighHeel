using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace COM3D2.HighHeel
{
    public class UI
    {
        private const int WindowId = 523961;

        private static readonly GUILayoutOption NoExpand = GUILayout.ExpandWidth(false);
        private static readonly GUILayoutOption TextFieldLayout = GUILayout.Width(70f);

        private static readonly GUIContent EnabledLabel = new(Plugin.PluginString);
        private static readonly GUIContent CloseLabel = new("x");
        private static readonly GUIContent RefreshLabel = new("Refresh List");
        private static readonly GUIContent ReloadLabel = new("Reload");
        private static readonly GUIContent SaveLabel = new("Save");
        private static readonly GUIContent BodyOffsetLabel = new("Body Offset:");
        private static readonly GUIContent FootLAngleLabel = new("Foot L Angle:");
        private static readonly GUIContent FootRAngleLabel = new("Foot R Angle:");

        private List<Maid> maidList = new();
        private Vector2 scrollPos = Vector2.zero;
        private Dictionary<Maid, TempText> tempTexts = new();
        public bool Visible;
        private Rect windowRect = new(20f, 20f, 240f, 450f);

        private GUIStyle? windowStyle;
        private GUIStyle WindowStyle => windowStyle ??= new(GUI.skin.box);

        public event EventHandler? OnSaveEvent;
        public event EventHandler? OnReloadEvent;

        public void Draw()
        {
            if (!Visible) return;

            windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + 20, Screen.width - 20);
            windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + 20, Screen.height - 20);
            windowRect = GUILayout.Window(WindowId, windowRect, GuiFunc, string.Empty, WindowStyle);
        }

        private void GuiFunc(int windowId)
        {
            DrawTitlebar();

            if (Plugin.Instance!.Configuration.Enabled.Value) DrawMaidList();

            GUILayout.FlexibleSpace();

            DrawBottomButtons();

            GUI.enabled = true;
            GUI.DragWindow();
        }

        public void Update()
        {
            if (Input.mouseScrollDelta.y == 0f || !Visible) return;

            var mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            if (windowRect.Contains(mousePos)) Input.ResetInputAxes();
        }

        private void DrawTitlebar()
        {
            var plugin = Plugin.Instance!;

            GUILayout.BeginHorizontal();

            var configPluginEnabled = plugin.Configuration.Enabled.Value;

            var pluginEnabled = GUILayout.Toggle(configPluginEnabled, EnabledLabel);

            if (pluginEnabled != configPluginEnabled) plugin.Configuration.Enabled.Value = pluginEnabled;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(CloseLabel, NoExpand)) Visible = false;

            GUILayout.EndHorizontal();
        }

        private void DrawMaidList()
        {
            var plugin = Plugin.Instance!;

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (var maid in maidList)
            {
                var (guid, firstName, lastName) = maid.status;
                var tempStrings = tempTexts[maid];

                var (body, footL, footR) = tempStrings;

                GUILayout.Label($"{firstName} {lastName}");

                var tempBody = DrawSetting(BodyOffsetLabel, body);
                var tempFootL = DrawSetting(FootLAngleLabel, footL);
                var tempFootR = DrawSetting(FootRAngleLabel, footR);

                GuiUtil.DrawLine();

                var update = tempBody != body || tempFootL != footL || tempFootR != footR;

                if (!update) continue;

                if (!plugin.Database.TryGetValue(guid, out var data)) continue;

                tempStrings.Body = tempBody;
                tempStrings.FootL = tempFootL;
                tempStrings.FootR = tempFootR;

                // Try parse all text fields and update if they're all valid
                var updateDb = Utility.TryParseDefault(tempBody, data.BodyOffset, out var newBody)
                    & Utility.TryParseDefault(tempFootL, data.FootLAngle, out var newFootL)
                    & Utility.TryParseDefault(tempFootR, data.FootRAngle, out var newFootR);

                if (updateDb)
                    plugin.Database[guid] = data with
                    {
                        BodyOffset = newBody, FootLAngle = newFootL, FootRAngle = newFootR,
                    };
            }

            GUILayout.EndScrollView();

            static string DrawSetting(GUIContent label, string value)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(label);
                var newValue = GUILayout.TextField(value, TextFieldLayout);
                GUILayout.EndHorizontal();

                return newValue;
            }
        }

        private void DrawBottomButtons()
        {
            GUI.enabled = Plugin.Instance!.Configuration.Enabled.Value;

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(RefreshLabel, NoExpand)) RefreshMaidList();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(ReloadLabel, NoExpand))
            {
                OnReloadEvent?.Invoke(null, EventArgs.Empty);
                RefreshMaidList();
            }

            if (GUILayout.Button(SaveLabel, NoExpand)) OnSaveEvent?.Invoke(null, EventArgs.Empty);

            GUILayout.EndHorizontal();
        }

        public void RefreshMaidList()
        {
            maidList = GameMain.Instance.CharacterMgr.GetStockMaidList()
                .Where(maid => maid != null && maid.Visible && maid.isActiveAndEnabled).ToList();

            tempTexts = maidList.Select(maid => new { maid, config = Plugin.Instance!.Database[maid.status.guid] })
                .ToDictionary(x => x.maid, x => new TempText(x.config));
        }

        private static string FormatValue(float value) => value.ToString("0.####", CultureInfo.InvariantCulture);

        private class TempText
        {
            public string Body;
            public string FootL;
            public string FootR;

            public TempText(MaidConfig config)
            {
                var (body, footL, footR) = config;
                Body = FormatValue(body);
                FootL = FormatValue(footL);
                FootR = FormatValue(footR);
            }

            public void Deconstruct(out string body, out string footL, out string footR)
            {
                body = Body;
                footL = FootL;
                footR = FootR;
            }
        }

        private static class GuiUtil
        {
            private static readonly GUIStyle LineStyleWhite;
            private static readonly GUILayoutOption LineHeight = GUILayout.Height(1);

            static GuiUtil()
            {
                LineStyleWhite = new(GUI.skin.box)
                {
                    margin = new(0, 0, 8, 8),
                    normal = { background = MakeTex(2, 2, new(1f, 1f, 1f, 0.2f)) },
                };

                LineStyleWhite.padding = LineStyleWhite.border = new(0, 0, 1, 1);
            }

            public static void DrawLine() => GUILayout.Box(GUIContent.none, LineStyleWhite, LineHeight);

            private static Texture2D MakeTex(int width, int height, Color color)
            {
                var colors = new Color[width * height];
                for (var i = 0; i < colors.Length; i++) colors[i] = color;
                var texture2D = new Texture2D(width, height);
                texture2D.SetPixels(colors);
                texture2D.Apply();

                return texture2D;
            }
        }
    }
}
