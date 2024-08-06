using GatherBuddy.AutoGather;
using OtterGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Dalamud.Interface;
using ECommons.ImGuiMethods;
using GatherBuddy.Config;
using GatherBuddy.Enums;
using GatherBuddy.GatherHelper;
using GatherBuddy.Interfaces;
using ImGuiNET;
using OtterGui.Raii;
using OtterGui.Widgets;
using static GatherBuddy.Gui.Interface.PerItemConfigFunctions;

namespace GatherBuddy.Gui
{
    public partial class Interface
    {
        private readonly        PerItemConfig            _perItemConfig = GatherBuddy.Config.AutoGatherConfig.PerItemConfig;
        private readonly        GatherWindowPerItemCache _perItemCache;

        private static readonly IGatherable[] _gatherables = GatherBuddy.GameData.Gatherables.Values
            .Where(gatherable => !IsFisherGatherable(gatherable.GatheringType)).OrderBy(g => g.Name[GatherBuddy.Language]).ToArray();

        private void DrawManualPerItemConfiguration()
        {
            DrawPerItemEnableBox();

            if (!_perItemConfig.Enabled)
                return;

            DrawAddItemButton();
            ImGui.SameLine();
            DrawItemDrop();

            using var box = ImRaii.ListBox("##perItemList", new Vector2(-1.5f * ImGui.GetStyle().ItemSpacing.X, -1));
            if (!box)
                return;

            for (var i = 0; i < _gatherables.Length; ++i)
            {
                using var id    = ImRaii.PushId(i);
                using var group = ImRaii.Group();
                var       item  = _gatherables[i];

                if (ImGui.CollapsingHeader(item.Name.English))
                {

                }
                group.Dispose();
            }
        }
        private void DrawPerItemEnableBox()
        {
            DrawCheckbox("Enable",
                "Enabling this will override any gathering behaviour and specifically use set rotations for configured gatherables.",
                GatherBuddy.Config.AutoGatherConfig.PerItemConfig.Enabled, g => GatherBuddy.Config.AutoGatherConfig.PerItemConfig.Enabled = g);
        }

        public void DrawAddItemButton()
        {
            if (!ImGuiUtil.DrawDisabledButton(FontAwesomeIcon.Plus.ToIconString(), IconButtonSize, "Adds a new item to the list below.",
                    _perItemConfig.PerItemConfigurationCollection.ContainsGatherableWithId(_perItemCache.GatherableIdx), true))
                return;

            var gatherable = _gatherables.Single(x => x.ItemId == _perItemCache.GatherableIdx);
            if (!_perItemConfig.PerItemConfigurationCollection.Contains(gatherable))
                _perItemConfig.PerItemConfigurationCollection.Add(_gatherables.Single(x=> x.ItemId == _perItemCache.GatherableIdx));
        }

        public void DrawItemDrop()
        {
            if (_perItemCache.GatherableSelector.Draw(_perItemCache.GatherableIdx, out var idx))
                _perItemCache.GatherableIdx = idx;
        }

        public static class PerItemConfigFunctions
        {
            public static void DrawCheckbox(string label, string description, bool oldValue, Action<bool> setter)
            {
                if (ImGuiUtil.Checkbox(label, description, oldValue, setter))
                    GatherBuddy.Config.Save();
            }

            public static bool IsFisherGatherable(GatheringType gatheringType)
                => gatheringType == GatheringType.Fisher;
        }


        //private class GatherWindowPerItemDragDropData
        //{
        //    public GatherWindowPerItemPreset Preset;
        //    public IGatherable               Item;
        //    public int                       ItemIdx;

        //    public GatherWindowPerItemDragDropData(GatherWindowPerItemPreset preset, IGatherable item, int idx)
        //    {
        //        Preset  = preset;
        //        Item    = item;
        //        ItemIdx = idx;
        //    }
        //}
        private class GatherWindowPerItemCache
        {
            public readonly ClippedSelectableCombo<IGatherable> GatherableSelector =
                new("AllGatherables", string.Empty, 250, _gatherables, g => g.Name.English);

            public int GatherableIdx;
            
        }
    }

}
