# Candybars

Build a party and target HUD out of Umbra bars. Each widget is one person. Stack copies on a vertical aux bar and use Umbra's Separator between raid groups.

## Widgets

| Widget | What it shows |
|---|---|
| Candybar | Name, job, HP, MP, shield, cast, buffs, debuffs |
| Candy Name | Name only, larger text |
| Candy Job | Job icon only, larger |
| Candy Vitals | HP, MP, and shield, taller bars |
| Candy Cast | Name and cast bar |
| Candy Buffs | Buff icons |
| Candy Debuffs | Debuff icons, green border if cleansable |
| Candy Status | Buffs and debuffs, buffs smaller and on top |

On Candybar, each row has its own size and a position number. Lower numbers sit higher. Job icon, name, and numbers can be reordered the same way. Buff and debuff counts go up to 16. Candy Status starts with buffs on top and slightly larger debuffs underneath.

Every widget can follow:

- Me
- Target, Target of Target, Focus Target
- Party 1–8
- Alliance A1–A8, B1–B8, C1–C8

Width, bar height, text size, and icon size are settings. Width applies on a vertical aux bar. Turn **Background** off for a bare bar. Click a bar to target that person. Empty slots hide, so the stack closes up.

A dead person goes grey and reads DEAD. A cleansable debuff turns the border green. Casts, shields, and status icons only update while that character is loaded. There is no stamina stat on the party list, so the third vitals bar is shield.

Party 1 and your own alliance slot are the same person. Don't place both.

## Install

Umbra → Settings → Plugins:

- Owner: `ShadowstarIO`
- Repository: `Umbra.Candybars`

Repository: https://github.com/ShadowstarIO/Umbra.Candybars
