using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using ECommons;
using ECommons.ChatMethods;
using ECommons.DalamudServices;
using ECommons.GameFunctions;
using ECommons.GameHelpers;
using ECommons.Hooks.ActionEffectTypes;
using ECommons.ImGuiMethods;
using Splatoon.SplatoonScripting;
using Splatoon.Utility;
using System.Collections.Generic;
using System.Linq;
using UIColor = ECommons.ChatMethods.UIColor;

namespace SplatoonScriptsOfficial.Duties.Dawntrail.Dancing_Mad;

public class P4_Debuff_Reminder_ChatOnly : SplatoonScript<P4_Debuff_Reminder_ChatOnly.Config>
{
    public override Metadata Metadata { get; } = new(1, "Ahmed Alnuaimi (AihmedML), based on NightmareXIV, mirage");
    public override HashSet<uint>? ValidTerritories { get; } = [1363];

    private readonly List<string> VfxLie =
    [
        "vfx/common/eff/z3oy_stlp6_c0c.avfx",
        "vfx/common/eff/z3oy_stlp4_c0c.avfx",
    ];

    private readonly List<string> VfxTruth =
    [
        "vfx/common/eff/z3oy_stlp7_c0c.avfx",
        "vfx/common/eff/z3oy_stlp5_c0c.avfx",
    ];

    private record struct StatusInfo(uint objectId, uint statusId);

    private readonly Dictionary<uint, bool> IsTruth = [];
    private readonly List<StatusInfo> FakeStatuses = [];
    private List<uint>? debuffList;

    public bool IsLie = false;

    public class Debuffs
    {
        public static uint[] DebuffDontMove = [5546, 1072, 1384, 2657, 3793, 3802, 4144];
        public static uint[] DebuffLookAway = [5543, 452];
        public static uint[] DebuffStack = [1023, 5545, 2142];
        public static uint[] DebuffSpread = [587, 3799, 5544];
        public static uint[] DebuffFireSpread = [1600, 5547];
        public static uint[] DebuffDonut = [1601, 5548];
        public static uint DebuffLive = 454;
        public static uint[] DebuffDie = [1382, 5464];
        public static uint[] DebuffWhitewould = [4887, 5541];
        public static uint[] DebuffBlackwound = [4888, 5542];
    }

    public List<uint> DebuffList
    {
        get
        {
            if(debuffList != null)
            {
                return debuffList;
            }

            debuffList = [];
            foreach(var value in typeof(Debuffs).GetFields().Select(x => x.GetValue(null)!))
            {
                if(value is uint debuff)
                {
                    debuffList.Add(debuff);
                }
                else if(value is uint[] debuffs)
                {
                    debuffList.AddRange(debuffs);
                }
            }

            return debuffList;
        }
    }

    public override void OnReset()
    {
        IsTruth.Clear();
        FakeStatuses.Clear();
        IsLie = false;
    }

    public override void OnVFXSpawn(uint target, string vfxPath)
    {
        if(target.GetObject()?.DataId.EqualsAny<uint>(19510, 19507) != true)
        {
            return;
        }

        if(VfxTruth.Contains(vfxPath))
        {
            IsTruth[target] = true;
        }
        else if(VfxLie.Contains(vfxPath))
        {
            IsTruth[target] = false;
        }
    }

    public override void OnActionEffectEvent(ActionEffectSet set)
    {
        if(set.Action != null && set.Source?.ObjectId.EqualsAny(IsTruth.Keys) == true)
        {
            IsLie = !IsTruth[set.Source.ObjectId];
        }
    }

    public override void OnGainBuffEffect(uint sourceId, FFXIVClientStructs.FFXIV.Client.Game.Status status)
    {
        if(!DebuffList.Contains(status.StatusId) || !sourceId.TryGetPlayer(out var pc))
        {
            return;
        }

        if(IsLie)
        {
            FakeStatuses.Add(new(sourceId, status.StatusId));
        }

        if(!pc.AddressEquals(BasePlayer) || !C.OutputInChat)
        {
            return;
        }

        if((Debuffs.DebuffSpread.Contains(status.StatusId) && !IsLie) ||
           (Debuffs.DebuffStack.Contains(status.StatusId) && IsLie))
        {
            Print(UIColor.Orange, status.RemainingTime > 60f ? C.LongSpread.Get() : C.ShortSpread.Get());
        }

        if(Debuffs.DebuffLookAway.Contains(status.StatusId))
        {
            if(status.RemainingTime > 65f)
            {
                Print(UIColor.Red, IsLie ? C.LongGazeInv.Get() : C.LongGaze.Get());
            }
            else
            {
                Print(UIColor.Red, IsLie ? C.ShortGazeInv.Get() : C.ShortGaze.Get());
            }
        }

        if(Debuffs.DebuffDontMove.Contains(status.StatusId))
        {
            Print(UIColor.Yellow, IsLie ? C.AccelerationBombInv.Get() : C.AccelerationBomb.Get());
        }
    }

    private void Print(UIColor color, string msg)
    {
        var entry = new XivChatEntry
        {
            Message = new SeStringBuilder().AddUiForeground(msg, (ushort)color).Build(),
        };

        if(C.OverrideChatType != XivChatType.None)
        {
            entry.Type = C.OverrideChatType;
        }

        Svc.Chat.Print(entry);
    }

    public override void OnSettingsDraw()
    {
        ImGui.Checkbox("Output your debuffs into local chat (for you only)", ref C.OutputInChat);
        if(C.OutputInChat)
        {
            ImGui.Indent();
            ImGui.SetNextItemWidth(240f);
            ImGuiEx.EnumCombo("Override chat channel (local only; visual chat tab type)", ref C.OverrideChatType);
            ImGui.Unindent();
        }

        ImGui.Separator();
        ImGui.SetNextItemWidth(260f);
        C.AccelerationBomb.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Acceleration bomb, normal");

        ImGui.SetNextItemWidth(260f);
        C.AccelerationBombInv.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Acceleration bomb, inverted");

        ImGui.SetNextItemWidth(260f);
        C.LongGaze.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Long gaze (away)");

        ImGui.SetNextItemWidth(260f);
        C.LongGazeInv.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Long gaze (at)");

        ImGui.SetNextItemWidth(260f);
        C.ShortGaze.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Short gaze (away)");

        ImGui.SetNextItemWidth(260f);
        C.ShortGazeInv.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Short gaze (at)");

        ImGui.SetNextItemWidth(260f);
        C.LongSpread.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Long spread");

        ImGui.SetNextItemWidth(260f);
        C.ShortSpread.ImGuiEditNoDefault();
        ImGui.SameLine();
        ImGuiEx.Text("Short spread");

        if(ImGui.CollapsingHeader("Debug"))
        {
            ImGui.Checkbox(nameof(IsLie), ref IsLie);
            ImGuiEx.Text($"Tracked debuffs: {DebuffList.Print()}");
            ImGuiEx.Text($"Casters: {IsTruth.Select(x => $"{x.Key}: {x.Value}").Print("\n")}");
            ImGuiEx.Text($"Fake statuses: {FakeStatuses.Select(x => $"{x.objectId} / {x.statusId}").Print("\n")}");
        }
    }

    public class Config
    {
        public bool OutputInChat = true;
        public XivChatType OverrideChatType = XivChatType.None;
        public InternationalString AccelerationBomb = new() { En = "Acceleration bomb on YOU (DON'T MOVE)" };
        public InternationalString AccelerationBombInv = new() { En = "Inverted acceleration bomb on YOU (MOVE)" };
        public InternationalString LongGaze = new() { En = "LONG GAZE on YOU (Look Away)" };
        public InternationalString LongGazeInv = new() { En = "LONG GAZE on YOU (Look At)" };
        public InternationalString ShortGaze = new() { En = "SHORT GAZE on YOU (Look Away)" };
        public InternationalString ShortGazeInv = new() { En = "SHORT GAZE on YOU (Look At)" };
        public InternationalString LongSpread = new() { En = "LONG SPREAD on YOU" };
        public InternationalString ShortSpread = new() { En = "SHORT SPREAD on YOU" };
    }
}
