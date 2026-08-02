# Running this server locally

This branch is HardLight (`fenndragon/HardLight`) plus the `Drone` physical trait.

## Requirements

* The **.NET 10 SDK** (`global.json` pins `10.0.100`). Get it from
  <https://dotnet.microsoft.com/download/dotnet/10.0>, or on Linux/macOS:

  ```sh
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 10.0
  export PATH="$HOME/.dotnet:$PATH"
  ```

* Git with submodule support. RobustToolbox is a submodule, so a plain download of the zip will not
  build.

## Clone and build

```sh
git clone -b hardlight-drone https://github.com/Jayty07/Rubber-Drone-Trait.git hardlight
cd hardlight
git submodule update --init --recursive

dotnet build Content.Server/Content.Server.csproj
dotnet build Content.Client/Content.Client.csproj
```

The first build pulls a lot of NuGet packages and takes several minutes.

## Run

Two terminals, from the repo root.

Server:

```sh
dotnet run --project Content.Server -- --cvar net.port=1212 --cvar auth.mode=0
```

`auth.mode=0` disables the central login server so you can connect without an SS14 account. Never use
it on a public server.

Client:

```sh
dotnet run --project Content.Client
```

The development client connects to `localhost:1212` on its own. If it does not, use the
"Direct Connect" field in the launcher UI with `localhost:1212`.

## Give yourself admin

In the **server** console (the terminal running Content.Server):

```
promotehost localhost@<your username>
```

The full session name is required — `promotehost <username>` alone fails with "Unable to find a
player by that name". After that you have the admin menu (`F5`) and the spawn panel in the client.

## Trying the Drone trait

1. In the lobby, open **Customize** → **Traits** → **Physical** and tick **Drone** (costs 6 of your
   10 physical points).
2. Ready up and join the round.
3. The trait's internals are innate: with no mask and no tank equipped, the **Toggle Internals**
   action in the hotbar switches the internals alert to `On`.
4. Spawn any food or drink from the admin spawn panel and try to consume it — it is blocked.
5. Walk around for a while and you will eventually be knocked off balance; after each fall there is a
   45 second grace period before the roll can trigger again.

## Troubleshooting

* **Client crashes at startup on `/Audio/UserInterface/hover.ogg`** — OpenAL cannot open an audio
  device (common on headless Linux/VMs/WSL). Run the client with audio disabled:

  ```sh
  ALSOFT_DRIVERS=null dotnet run --project Content.Client
  ```

* **Rendering fails or the window is black** — force the compatibility renderer:

  ```sh
  dotnet run --project Content.Client -- --cvar display.compat=true
  ```

* **`The specified SDK version 10.0.100 was not found`** — the .NET 10 SDK is not on your `PATH`; see
  Requirements above.
