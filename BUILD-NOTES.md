# IrianTech — changes made to the installed mods (not in any repo)

These files ship in the installed mod packages under
`C:\Program Files (x86)\Steam\steamapps\common\BATTLETECH\Mods\` and are not tracked by
this fork or any other repo. A reinstall or mod update will overwrite them, so the fixes
are recorded here to reapply. Originals are backed up in `C:\BattleTech\backups\`.

## 2026-10-08 — CustomAmmoCategories: FGAUSS ammo box failed to load

`Mods/CustomAmmoCategories/ammunitionBox/Ammo_AmmunitionBox_Generic_FGAUSS.json`
(shipped 2019-06-11) was missing a comma after `"CriticalComponent" : false` (line 26),
so the game dropped the item and reported a resource load failure:

    DataLoader [ERROR] LoadResource() - Caught exception while loading [...Ammo_AmmunitionBox_Generic_FGAUSS.json]
    System.Exception: AmmunitionBoxDef parse error:Ammo_AmmunitionBox_Generic_FGAUSS
    Newtonsoft.Json.JsonReaderException: After parsing a value an unexpected character was encountered: ". Path 'CriticalComponent', line 27, position 4.

**Fix:** add the comma. The file is UTF-8 with non-ASCII bytes, so validate it as UTF-8.

Backup: `backups\Ammo_AmmunitionBox_Generic_FGAUSS.json-20261008.bak`

## 2026-10-08 — IRTweaks: restrictive defaults

`Mods/IRTweaks/mod.json` ships with three restrictions on in its `Misc` block. All three
were set to `false`:

| Setting                 | Shipped | Now     | Effect when `true`                         |
|-------------------------|---------|---------|--------------------------------------------|
| `DisableCampaign`       | `true`  | `false` | Hides the story Campaign main-menu button  |
| `DisableCombatRestarts` | `true`  | `false` | No restarting a mission                    |
| `DisableCombatSaves`    | `true`  | `false` | No saving during a mission                 |

Backup: `backups\IRTweaks-mod.json-20261008.bak`

## Known, left alone

`Mods/CustomUnits/ability/AbilityDefCU_SpawnUnit.json` and
`Mods/CustomUnits/drop_decorations/decoration_slot_hotdrop.json` reference the icon
`landing-safe-svgrepo-com`, which is not installed. It logs a
`ContainedLoadRequest [ERROR] ManifestEntry is null` line and at worst shows a blank icon.
