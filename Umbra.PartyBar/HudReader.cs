using System.Collections.Generic;
using System.Text;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using ExcelAction = Lumina.Excel.Sheets.Action;
using ExcelStatus = Lumina.Excel.Sheets.Status;
using Umbra.Common;
using GameStatus = FFXIVClientStructs.FFXIV.Client.Game.Status;

namespace Umbra.PartyBar;

internal static unsafe class HudReader
{
    internal readonly record struct Slot(
        string Name,
        byte Job,
        uint Hp,
        uint MaxHp,
        uint Mp,
        uint MaxMp,
        uint EntityId,
        byte Shield)
    {
        public bool Dead => MaxHp > 0 && Hp == 0;
    }

    internal readonly record struct StatusIcon(uint IconId, bool Debuff, bool Cleansable, byte Stacks, float Remaining);

    internal readonly record struct CastBar(string Name, float Progress);

    internal readonly record struct Subject(Slot Slot, CastBar? Cast, bool HasCleansable, IReadOnlyList<StatusIcon> Statuses);

    private static IObjectTable Objects { get; } = Framework.Service<IObjectTable>();
    private static ITargetManager Targets { get; } = Framework.Service<ITargetManager>();
    private static IDataManager Data { get; } = Framework.Service<IDataManager>();

    private static readonly Dictionary<uint, (uint Icon, bool Debuff, bool Cleansable)> StatusCache = new();
    private static readonly Dictionary<uint, string> ActionNames = new();
    private static readonly List<StatusIcon> StatusBuffer = new(16);

    internal static Subject? Read(string who)
    {
        if (who == "me") return ReadMe();
        if (who == "target") return FromActor(Targets.Target);
        if (who == "tot") return FromActor(Targets.Target?.TargetObject);
        if (who == "focus") return FromActor(Targets.FocusTarget);
        if (who == "choco") return FromActor(Companion());
        if (who == "pet") return FromActor(Pet());

        if (who.Length >= 2 && who[0] == 'p' && int.TryParse(who[1..], out var party))
            return ReadParty(party - 1);

        if (who.Length >= 2 && who[0] is 'a' or 'b' or 'c' && int.TryParse(who[1..], out var index))
        {
            var group = who[0] switch { 'a' => 0, 'b' => 1, _ => 2 };
            return ReadAlliance(group, index - 1);
        }

        return null;
    }

    internal static Subject Preview(Subject? live)
    {
        var slot = live?.Slot ?? new Slot("Player Name", 24, 18000, 24000, 6400, 10000, 0, 30);
        if (slot.MaxHp == 0 || slot.Job == 0)
        {
            slot = new Slot(
                slot.MaxHp == 0 ? "Player Name" : slot.Name,
                slot.Job == 0 ? (byte)24 : slot.Job,
                slot.MaxHp == 0 ? 18000 : slot.Hp,
                slot.MaxHp == 0 ? 24000 : slot.MaxHp,
                slot.MaxMp == 0 ? 6400 : slot.Mp,
                slot.MaxMp == 0 ? 10000 : slot.MaxMp,
                slot.EntityId,
                slot.Shield == 0 ? (byte)30 : slot.Shield);
        }

        return new Subject(slot, live?.Cast ?? new CastBar("Cure", 0.45f), true, SampleIcons());
    }

    private static IReadOnlyList<StatusIcon> SampleIcons()
    {
        return
        [
            new(62119, false, false, 2, 18),
            new(62121, false, false, 0, 14),
            new(62124, false, false, 0, 9),
            new(62128, false, false, 0, 6),
            new(62132, true, true, 0, 12),
            new(62133, true, false, 0, 8),
            new(62137, true, false, 0, 4),
        ];
    }

    internal static Subject Empty(string label)
    {
        return new Subject(new Slot(label, 0, 0, 0, 0, 0, 0, 0), null, false, []);
    }

    internal static string WhoLabel(string who)
    {
        if (who == "me") return "Me";
        if (who == "target") return "Target";
        if (who == "tot") return "Target of Target";
        if (who == "focus") return "Focus Target";
        if (who == "choco") return "Chocobo";
        if (who == "pet") return "Pet";
        if (who.Length >= 2 && who[0] == 'p' && int.TryParse(who[1..], out var party))
            return $"F{party}";
        if (who.Length >= 2 && who[0] is 'a' or 'b' or 'c' && int.TryParse(who[1..], out var index))
            return $"{char.ToUpperInvariant(who[0])}{index}";

        return who;
    }

    internal static Dictionary<string, string> WhoOptions()
    {
        var options = new Dictionary<string, string>
        {
            ["me"] = "Me",
            ["target"] = "Target",
            ["tot"] = "Target of Target",
            ["focus"] = "Focus Target",
            ["choco"] = "Chocobo",
            ["pet"] = "Pet",
        };

        for (var i = 1; i <= 8; i++)
            options[$"p{i}"] = $"F{i}";

        foreach (var group in new[] { 'a', 'b', 'c' })
        {
            for (var i = 1; i <= 8; i++)
                options[$"{group}{i}"] = $"{char.ToUpperInvariant(group)}{i}";
        }

        return options;
    }

    private static IGameObject? Companion()
    {
        return Framework.Service<IBuddyList>().CompanionBuddy?.GameObject;
    }

    private static IGameObject? Pet()
    {
        var buddies = Framework.Service<IBuddyList>();
        var pet = buddies.PetBuddy?.GameObject;
        if (pet != null && pet.IsValid())
            return pet;

        var self = Objects.LocalPlayer;
        if (self == null)
            return null;

        var companionId = buddies.CompanionBuddy?.GameObject?.EntityId ?? 0u;
        foreach (var actor in Objects)
        {
            if (actor is not IBattleNpc npc || !npc.IsValid())
                continue;

            if (npc.OwnerId == self.EntityId && npc.EntityId != companionId)
                return npc;
        }

        return null;
    }

    private static Subject? ReadMe()
    {
        var self = Objects.LocalPlayer;
        if (self == null)
            return null;

        var group = Group();
        if (group != null)
        {
            var member = group->GetPartyMemberByEntityId(self.EntityId);
            if (Occupied(member))
                return FromMember(member);
        }

        return FromActor(self);
    }

    private static Subject? ReadParty(int index)
    {
        if (index is < 0 or > 7)
            return null;

        var list = PartyListNumberArray.Instance();
        if (list != null)
        {
            var count = list->PartyListCount;
            if (count < 0)
                count = 0;
            if (count > 8)
                count = 8;

            if (index < count)
                return FromEntity(list->PartyMembers[index].EntityId) ?? (index == 0 ? ReadMe() : null);

            var trustIndex = index - count;
            var trusts = list->TrustCount;
            if (trusts < 0)
                trusts = 0;
            if (trusts > 7)
                trusts = 7;

            if (trustIndex < trusts)
                return FromEntity(list->TrustMembers[trustIndex].EntityId);

            if (count == 0 && index == 0)
                return ReadMe();

            return null;
        }

        var group = Group();
        if (group == null)
            return index == 0 ? ReadMe() : null;

        return FromMember(group->GetPartyMemberByIndex(index));
    }

    private static Subject? ReadAlliance(int groupIndex, int index)
    {
        if (groupIndex is < 0 or > 2 || index is < 0 or > 7)
            return null;

        var list = AllianceListNumberArray.Instance();
        if (list != null && groupIndex < list->PartyCount)
        {
            var party = list->Groups[groupIndex];
            if (index >= party.MemberCount)
                return null;

            return FromEntity(party.Members[index].EntityId);
        }

        var group = Group();
        if (group == null)
            return null;

        return FromMember(group->GetAllianceMemberByGroupAndIndex(groupIndex, index));
    }

    private static Subject? FromEntity(uint entityId)
    {
        if (entityId is 0 or 0xE0000000)
            return null;

        var group = Group();
        if (group != null)
        {
            var member = group->GetPartyMemberByEntityId(entityId);
            if (Occupied(member))
                return FromMember(member);

            for (var party = 0; party < 3; party++)
            {
                for (var slot = 0; slot < 8; slot++)
                {
                    member = group->GetAllianceMemberByGroupAndIndex(party, slot);
                    if (Occupied(member) && member->EntityId == entityId)
                        return FromMember(member);
                }
            }
        }

        var actor = Objects.SearchById(entityId);
        return actor == null ? null : FromActor(actor);
    }

    private static GroupManager.Group* Group()
    {
        var manager = GroupManager.Instance();
        return manager == null ? null : manager->GetGroup();
    }

    private static Subject? FromActor(IGameObject? actor)
    {
        if (actor is not ICharacter character || !character.IsValid())
            return null;

        var battle = actor as IBattleChara;
        var statuses = ReadStatuses(battle, null);
        return new Subject(
            new Slot(
                character.Name.TextValue,
                (byte)character.ClassJob.RowId,
                character.CurrentHp,
                character.MaxHp,
                character.CurrentMp,
                character.MaxMp,
                character.EntityId,
                character.ShieldPercentage),
            ReadCast(battle),
            HasCleansable(statuses),
            statuses);
    }

    private static Subject? FromMember(PartyMember* member)
    {
        if (!Occupied(member))
            return null;

        var actor = Objects.SearchById(member->EntityId) as IBattleChara;
        var statuses = ReadStatuses(actor, member);
        var shield = actor != null ? ((ICharacter)actor).ShieldPercentage : (byte)0;

        return new Subject(
            new Slot(
                ReadName(member),
                member->ClassJob,
                member->CurrentHP,
                member->MaxHP,
                member->CurrentMP,
                member->MaxMP,
                member->EntityId,
                shield),
            ReadCast(actor),
            HasCleansable(statuses),
            statuses);
    }

    private static List<StatusIcon> ReadStatuses(IBattleChara? actor, PartyMember* member)
    {
        StatusBuffer.Clear();

        if (actor != null)
        {
            foreach (var status in actor.StatusList)
            {
                if (StatusBuffer.Count >= 32 || status.StatusId == 0)
                    continue;

                if (!TryDescribe(status.StatusId, out var icon, out var debuff, out var cleansable))
                    continue;

                StatusBuffer.Add(new StatusIcon(icon, debuff, cleansable, (byte)System.Math.Min(status.Param, (ushort)99), status.RemainingTime));
            }
        }
        else if (member != null)
        {
            var count = System.Math.Min((int)member->StatusManager.NumValidStatuses, 30);
            var first = (GameStatus*)((byte*)member + 0x8);
            for (var i = 0; i < count && StatusBuffer.Count < 32; i++)
            {
                var status = first[i];
                if (status.StatusId == 0)
                    continue;

                if (!TryDescribe(status.StatusId, out var icon, out var debuff, out var cleansable))
                    continue;

                StatusBuffer.Add(new StatusIcon(icon, debuff, cleansable, (byte)System.Math.Min(status.Param, (ushort)99), status.RemainingTime));
            }
        }

        return [..StatusBuffer];
    }

    private static bool TryDescribe(uint statusId, out uint icon, out bool debuff, out bool cleansable)
    {
        if (StatusCache.TryGetValue(statusId, out var cached))
        {
            icon = cached.Icon;
            debuff = cached.Debuff;
            cleansable = cached.Cleansable;
            return icon != 0;
        }

        icon = 0;
        debuff = false;
        cleansable = false;

        var row = Data.GetExcelSheet<ExcelStatus>().GetRowOrDefault(statusId);
        if (row == null)
        {
            StatusCache[statusId] = (0, false, false);
            return false;
        }

        icon = row.Value.Icon;
        cleansable = row.Value.CanDispel;
        debuff = row.Value.StatusCategory == 2;
        StatusCache[statusId] = (icon, debuff, cleansable);
        return icon != 0;
    }

    private static CastBar? ReadCast(IBattleChara? actor)
    {
        if (actor is not { IsCasting: true } || actor.TotalCastTime <= 0)
            return null;

        var progress = System.Math.Clamp(actor.CurrentCastTime / actor.TotalCastTime, 0f, 1f);
        return new CastBar(CastName(actor), progress);
    }

    private static string CastName(IBattleChara actor)
    {
        if ((int)actor.CastActionType != 1)
            return "Casting";

        if (ActionNames.TryGetValue(actor.CastActionId, out var cached))
            return cached;

        var row = Data.GetExcelSheet<ExcelAction>().GetRowOrDefault(actor.CastActionId);
        var name = row?.Name.ToString();
        if (string.IsNullOrWhiteSpace(name))
            name = "Casting";

        ActionNames[actor.CastActionId] = name;
        return name;
    }

    private static bool HasCleansable(List<StatusIcon> statuses)
    {
        foreach (var status in statuses)
        {
            if (status.Debuff && status.Cleansable)
                return true;
        }

        return false;
    }

    private static bool Occupied(PartyMember* member)
    {
        if (member == null)
            return false;

        if (member->ContentId != 0)
            return true;

        if (member->EntityId is not (0 or 0xE0000000))
            return true;

        return ReadName(member).Length > 0;
    }

    private static string ReadName(PartyMember* member)
    {
        if (member->NameOverride != null)
        {
            var over = member->NameOverride->ToString();
            if (!string.IsNullOrWhiteSpace(over))
                return over;
        }

        var start = (byte*)member + 0x41C;
        var length = 0;
        while (length < 64 && start[length] != 0)
            length++;

        return length == 0 ? string.Empty : Encoding.UTF8.GetString(start, length);
    }
}
