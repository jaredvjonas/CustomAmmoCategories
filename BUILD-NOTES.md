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

## 2026-10-10 — CustomAmmoCategoriesPrivate.dll: use OUR build, not RogueTech's
RogueTech's `CustomAmmoCategoriesPrivate.dll` reads `LanceMechEquipmentListItem.componentRef` directly. That field only exists when RogueTech's
`ComponentRefInjector.dll` is installed, and we don't install it because it also changes the BaseComponentRef save format. Without the field,
its `SetTooltipData` prefix throws MissingFieldException at JIT time, which breaks the mech bay, the lance configurator (missions) and the shop.
Our fork's source reaches the field through the reflection shim (`GetComponentRef`), so it's safe. Build it with:
`dotnet msbuild CustomAmmoCategoriesPrivate\CustomAmmoCategoriesPrivate.csproj -restore -p:Configuration=Release -p:LangVersion=latest -p:BattleTechGameDir=... -p:RepoLibs=...\libs`
(`-p:LangVersion=latest` is needed for GlobalUsings.cs.) Copy `bin\Release\CustomAmmoCategoriesPrivate.dll` → `Mods\CustomAmmoCategories\`.
RogueTech's original is backed up in `C:\BattleTech\backups\2026-10-10-CACPrivate-RogueTech\`.

## 2026-10-10 — Added the missing gear for CAC's demo mechs
The old CAC 0.1.49 demo mechs (`mechdef_shadowhawk_SHD-KM`, `mechdef_urbanmech_UM-KM`, `mechdef_kingcrab_KGC-A`, used by the
tournament contracts) reference `Gear_SpeedBooster`, `Gear_Cockpit_CPU` and `Gear_Sensor_Prototype_EWE3`. None of these shipped with
CAC or RogueTech. They come from KMiSSioN's `CustomBundle.zip` (CustomActivatableEquipment/upgrades). CustomActivatableEquipment is not
installed, so `convert_gear.py` moves their effects into plain passive statusEffects and rewrites the text in English, with BonusValueA/B
matching the real effects. The results go in `Mods\CustomAmmoCategories\upgrades\`. The converted files, the originals and the script are
in `C:\BattleTech\backups\2026-10-10-CAC-demo-gear\`.
- Machine Spirit (CPU): already fully passive. +2 Initiative, Ace Pilot, Bulwark, +120m spotting, +4 Morale.
- X-3 EW Equipment (EWE3): its ECM was a toggle, now always on: a 100m field (same size as IrTech's EWE), +4 missile defence,
  indirect immunity, stealth charges. The Active Probe ping is unchanged.
- Ferrum fibres (SpeedBooster): was a toggled x3 speed, now a permanent x1.75 walk/run speed (+75%, same as IrTech MASC Mk3; set in convert_gear.py).

## 2026-10-10: IRTweaks removed and CBTBehaviorsEnhanced disabled (user wants a near-vanilla game)
Both came from RogueTech `Core/` on 2026-10-08, only to satisfy CustomShops' `DependsOn: ["IRTweaks"]` (IRTweaks in turn depends on CBTBE).
Problems they caused:
- **No head called shots.** Setting IRTweaks `Combat.CalledShot.DisableHeadshots: false` was not enough. IRTweaks 2.2.8
  `HUDMechArmorReadout_SetHoveredAmor` (on whenever `Fixes.CalledShotTweaks` is) ignores that setting and clears the head hover unless the
  target is prone or shut down, so the head could never be clicked.
- **Vanilla menus replaced**: StreamlinedMainMenu (the Argo nav menu), MechbayLayout, difficulty labels.

Current state:
- `Mods\IRTweaks` is gone. The whole folder was moved to `C:\BattleTech\backups\removed-mods-2026-10-10\IRTweaks\`.
- `Mods\CBTBehaviorsEnhanced` is still installed but disabled: `modstate.json` = `{"Enabled":false}`. A few IrianTech `rogueTech\` items
  carry CBTBE_* stats, and those are now inert.
- `Mods\CustomShops\mod.json`: `"DependsOn": []` plus `"OptionallyDependsOn": ["IRTweaks"]`. CustomShops.dll probes for IRTweaks at
  runtime ("No IRTweaks found, usual shop button"), so it is not a hard dependency. Re-apply this after any CustomShops reinstall.
- **Gotcha:** ModTek 4.5.1 ignores `"Enabled"` in mod.json once the mod folder has a `modstate.json`. Always disable a mod through
  `<mod>\modstate.json`.
- CBTBehaviors (v0.2.0, the small 2019 mod) is deliberately kept: the user likes its fire-after-sprinting rule. With CBTBE gone, its
  `AlwaysInterleaved` turn mode is in effect again (turn-based from the start of every mission).
Original manifests and modstate files: `C:\BattleTech\backups\removed-mods-2026-10-10\`. The earlier IRTweaks headshot setting backup is in
`C:\BattleTech\backups\2026-10-10-IRTweaks-headshots\`.

## 2026-10-10: spawn protection turned off (units couldn't fire for the first rounds)
CustomUnits gives spawn protection on deploy ("SpawnProtection add till round 3 ... reason:manual deploy" in CAC_combat_log.txt). CAC's
`SpawnProtectionAffectsCanFire` (default true; also ToHit/Damage/AOE) then blocks protected units from firing. Vanilla has no such thing.
`Mods\CustomUnits\mod.json`: `DeployManualSpawnProtection` and `DeployAutoSpawnProtection` set to false, and `OnUnitSpawnProtection: false`
added (it defaults to true and covers reinforcements). Backup: `C:\BattleTech\backups\2026-10-10-spawn-protection\`.

## 2026-10-10: cFixes Javelin JVN-10A mechdef JSON fixed
`Mods\cFixes-1.9.1.1\mech\mechdef_javelin_JVN-10A.json` (cFixes 1.9.1.1 ships it like this) is missing a comma after `"unit_armor_low"` in MechTags,
so it failed to load ("After parsing a value an unexpected character ... MechTags.items[9]"). Comma added. Backup: `C:\BattleTech\backups\2026-10-10-cfixes-javelin\`.

## 2026-10-11: CustomComponents manifest updated (mech lab showed no hardpoints)
The deployed `Mods\CustomComponents\mod.json` was the old Denadan release manifest; our fork DLL needs `CCHardpoints` definitions, but the old
manifest neither registered that resource type nor shipped `data/hardpoints`. Result: "Missing weapon category info" for every weapon
(even Ballistic/Energy/Missile), ModTek "Type of entry unknown ... (CCHardpoints)" for IrianTech's omni file, and no hardpoints in the mech lab.
Fix: copied the fork's `data\` (hardpoints + unittypes) into `Mods\CustomComponents\`, added `CustomSVGIcon, CCHardpoints, CCCEquipLocationTag,
CCUnitTypes` to `CustomResourceTypes`, and added Manifest entries for `data/unittypes` (CCUnitTypes) and `data/hardpoints` (CCHardpoints).
Other settings left as they were. Backup: `C:\BattleTech\backups\CustomComponents-2026-10-11\`.

## 2026-10-12: CAC message text missing (showed CAC.MESSAGE_ID keys)
In-game CAC messages showed raw `CAC.<id>` keys instead of text. CAC writes `__/<key>/__` placeholders and CustomLocalization
fills them from any `Localization.json` it finds under `Mods\`, but the old deployed `Mods\CustomAmmoCategories` layout had no
`Localization\` folder. Fix: copied `AdditionalFiles\CAC\Localization\` (AIM + CAC, with DE/RU subfolders) to
`Mods\CustomAmmoCategories\Localization\`. Keep the DE subfolder: `CAC.Damaged` and `CAC.Destroyed` (jam results) are defined only there,
with their English text. This is part of a CAC deploy, alongside the companion DLLs.
