$ErrorActionPreference = "Stop"

$scriptPath = Join-Path $PSScriptRoot "P4_Debuff_Reminder_ChatOnly.cs"
$content = Get-Content -Raw -LiteralPath $scriptPath

$requiredPatterns = @(
    "class P4_Debuff_Reminder_ChatOnly",
    "OutputInChat",
    "OnGainBuffEffect",
    "OnVFXSpawn",
    "OnActionEffectEvent",
    "DebuffLookAway",
    "DebuffDontMove",
    "DebuffSpread",
    "DebuffStack"
)

$forbiddenPatterns = @(
    "RegisterElement",
    "RegisterLayout",
    "RegisterElements",
    "GetElementByName",
    "TryGetElementByName",
    "GetRegisteredLayouts",
    "OnUpdate",
    "Controller.Hide",
    "UseSelfmark",
    "/marking",
    "InternationalString"
)

foreach ($pattern in $requiredPatterns) {
    if ($content -notmatch [regex]::Escape($pattern)) {
        throw "Missing required pattern: $pattern"
    }
}

foreach ($pattern in $forbiddenPatterns) {
    if ($content -match [regex]::Escape($pattern)) {
        throw "Forbidden visual/command pattern found: $pattern"
    }
}

Write-Host "P4 chat-only script verification passed."
