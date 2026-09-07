# Vertical-slice minimal APIs + Azure App Configuration

Sample **.NET 10** ASP.NET Core minimal API that shows a maintainable **vertical slice** endpoint pattern, with **Azure App Configuration feature flags** that change live (no process restart).

Locally, [Aspire](https://learn.microsoft.com/dotnet/aspire/get-started/aspire-overview) runs the official **App Configuration emulator** in a container. When you publish, the same AppHost resource is a real Azure App Configuration store.

## What you get

| Path | Behavior |
| --- | --- |
| `GET /hello` | `Hello World` when feature flag `HelloNewWorld` is **off**; `Hello New World` when **on** |
| `GET /ping` | Second slice so you can see how a new folder/class is all it takes to add routes |
| `GET /health` | Aspire / local readiness (Development) |

Flags are evaluated through `IFeatureManager`. Requests do **not** call App Configuration on every hit. `UseAzureAppConfiguration` plus feature-flag cache intervals only contact the store after the refresh window (5 seconds in this demo). Key-values use a **sentinel** (`Demo:Sentinel`) so thousands of keys are not polled individually.

## Prerequisites

- .NET 10 SDK
- Docker or another OCI runtime (required for the emulator)
- Optional: Azure subscription when you want a real store instead of the emulator

## Run

```bash
dotnet run --project src/AppHost
```

Open the Aspire dashboard, then the **api** resource. Call:

```bash
curl http://localhost:<api-port>/hello
```

Open the **appconfiguration** resource URL (emulator UI). You should see:

- `.appconfig.featureflag/HelloNewWorld` — JSON with `"enabled": false|true`
- `Demo:Sentinel` — bump this (any new value) when you change non-flag keys and want `Register(sentinel, refreshAll: true)` to reload them

Set the feature flag value to `{"id":"HelloNewWorld","enabled":true}` (keep the feature-flag content type). Wait about five seconds, call `/hello` again — no API restart.

The emulator volume is persistent, so flag edits survive AppHost restarts. Seeding only **creates missing** keys.

## Add a new endpoint slice

1. Create a folder under `src/Api/Slices/<Name>/`.
2. Implement `IEndpoint` and map routes there (see `HelloEndpoints` or `PingEndpoints`).
3. Nothing else: `AddVerticalSliceEndpoints` scans the assembly.

Keep flag names as constants next to the slice (`HelloFeatures.NewWorld`) so App Configuration keys stay obvious.

## Tests

```bash
dotnet test
```

API tests flip `FeatureManagement:HelloNewWorld` in memory. They do not need Docker or Azure.

## Real Azure App Configuration

In **publish** / non-run mode, AppHost does not call `RunAsEmulator()`. Provision or attach an Azure App Configuration store, create the same feature flag in Feature manager, and assign the app **App Configuration Data Reader** (or Data Owner). The API uses the Aspire client integration (`builder.AddAzureAppConfiguration`) — connection string if present, otherwise the store endpoint plus `DefaultAzureCredential`.
