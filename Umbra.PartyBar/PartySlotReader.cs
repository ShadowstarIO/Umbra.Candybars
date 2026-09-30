using System.Text;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using Umbra.Common;

namespace Umbra.PartyBar;

internal static unsafe class PartySlotReader
{
    internal readonly record struct Slot(
        string Name,
        byte Job,
        uint Hp,
        uint MaxHp,
        uint Mp,
        uint MaxMp,
        uint EntityId)
    {
        public bool Dead => MaxHp > 0 && Hp == 0;
    }

    private static IObjectTable Objects { get; } = Framework.Service<IObjectTable>();

    internal static Slot? Read(string who)
    {
        if (who == "me")
            return ReadMe();

        if (who.Length >= 2 && who[0] == 'p' && int.TryParse(who[1..], out var party))
            return ReadParty(party - 1);

        if (who.Length >= 2 && who[0] is 'a' or 'b' or 'c' && int.TryParse(who[1..], out var index))
        {
            var group = who[0] switch
            {
                'a' => 0,
                'b' => 1,
                _ => 2,
            };

            return ReadAlliance(group, index - 1);
        }

        return null;
    }

    internal static Slot Empty(string label)
    {
        return new Slot(label, 0, 0, 0, 0, 0, 0);
    }

    private static Slot? ReadMe()
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

        if (self is not ICharacter character)
            return null;

        return new Slot(
            character.Name.TextValue,
            JobOf(character),
            character.CurrentHp,
            character.MaxHp,
            character.CurrentMp,
            character.MaxMp,
            self.EntityId);
    }

    private static Slot? ReadParty(int index)
    {
        var group = Group();
        if (group == null || index < 0)
            return null;

        return FromMember(group->GetPartyMemberByIndex(index));
    }

    private static Slot? ReadAlliance(int groupIndex, int index)
    {
        var group = Group();
        if (group == null || index < 0)
            return null;

        return FromMember(group->GetAllianceMemberByGroupAndIndex(groupIndex, index));
    }

    private static GroupManager.Group* Group()
    {
        var manager = GroupManager.Instance();
        return manager == null ? null : manager->GetGroup();
    }

    private static Slot? FromMember(PartyMember* member)
    {
        if (!Occupied(member))
            return null;

        return new Slot(
            ReadName(member),
            member->ClassJob,
            member->CurrentHP,
            member->MaxHP,
            member->CurrentMP,
            member->MaxMP,
            member->EntityId);
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

    private static byte JobOf(ICharacter character)
    {
        return (byte)character.ClassJob.RowId;
    }
}
