# P4 Debuff Reminder Chat Only

Chat-only fork of `P4 Debuff Reminder` for Splatoon scripts in FFXIV.

Created by Ahmed Alnuaimi (`AihmedML`). Based on the original `P4_Debuff_Reminder` by NightmareXIV and mirage from the PunishXIV Splatoon scripts repository.

## What this does

- Prints your P4 debuff reminders into local chat only.
- Keeps the original truth/lie inversion tracking for the supported debuff messages.
- Keeps configurable local-chat text for spread, gaze, and acceleration bomb.
- Does not register Splatoon elements.
- Does not register Splatoon layouts.
- Does not draw anything on the ground or screen.
- Does not use self-marking or `/marking` commands.

## Supported local-chat reminders

- Long spread on you
- Short spread on you
- Long gaze on you, look away
- Long gaze on you, look at
- Short gaze on you, look away
- Short gaze on you, look at
- Acceleration bomb, do not move
- Inverted acceleration bomb, move

## Install

1. Open Splatoon script settings.
2. Add a new custom script.
3. Paste the contents of `P4_Debuff_Reminder_ChatOnly.cs`.
4. Enable the script.
5. In the script configuration, keep `Output your debuffs into local chat (for you only)` enabled.

## Notes

This fork intentionally removes visual-only behavior from the original script. If you also want black/white, donut/AOE, or stack partner reminders printed in chat, those should be added as explicit chat features instead of reusing the removed ground display path.

## Verify

Run this from the repository folder:

```powershell
./verify.ps1
```

The check confirms that the script keeps the chat/debuff hooks and contains no Splatoon element/layout registration, `OnUpdate` drawing path, self-marking option, or `/marking` command.

## License

This is a derivative of AGPL-3.0 licensed Splatoon script code. Keep this fork under AGPL-3.0 when sharing or modifying it.

Original source:

- https://github.com/PunishXIV/Splatoon
- https://github.com/PunishXIV/Splatoon/blob/main/SplatoonScripts/Duties/Dawntrail/Dancing%20Mad/P4_Debuff_Reminder.cs
