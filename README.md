# Obra Dinn Archipelago

This is a mod to make *Return of the Obra Dinn* [Archipelago](https://archipelago.gg/) compatible.

Note: *Return of the Obra Dinn* will not be merged into the main Archipelago repo due to its 18+ rating; as such, you
will need the host of your Archipelago session to manually generate the required Archipelago zip file instead of using
the website's generator. Instructions to do so can be
found [here](https://archipelago.gg/tutorial/Archipelago/setup/en#on-your-local-installation)

## Setup Instructions

### First Time Setup

1. [Add the latest APWorld](https://github.com/wkcheezy/ObraDinnArchipelagoWorld/releases) to your Archipelago 'worlds' folder.
2. Download a supported mod manager (recommended), or manually set up the mod. **If you need to switch methods, ensure all files and programs from one method are removed before switching**.

#### Mod Manager Setup

3. Download and install either [r2modman](https://github.com/ebkr/r2modmanPlus/releases/latest) or the [Thunderstore Mod Manager](https://www.overwolf.com/app/thunderstore-thunderstore_mod_manager) 
4. Locate "Return of the Obra Dinn" in the list of games, then click on "Select Game" when hovering over it. *Recommended: Star the entry before clicking on "Select Game", you'll need to find it every time you wish to launch the game*
5. On the Profile selection screen, select either "Create new" to create a new profile (if you want to keep this mod separate from other mods) or "Select profile" (if you're fine with keeping your mods in one place)
6. In the sidebar, click on "Online", search for "Archipelago" in the top bar, click on the "Obra Dinn Archipelago" mod, then click "Download".

#### Manual Mod Setup

3. [Install BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html#installing-bepinex). Note: If on Windows, use the x86 version.
4. [Download and unzip the latest release](https://github.com/wkcheezy/ObraDinnArchipelago/releases) to the bepinex plugins folder

### Connecting to Archipelago

1. Create your YAML file. **Highly recommended to use different slot names for each Archipelago session, to help differentiate between saved sessions**
2. Launch the Obra Dinn client in the main Archipelago client (should be a button in the right column that says "Return of the Obra Dinn Client")
3. Connect to your session through the Obra Dinn client
4. Once connected, launch your modded copy of *Return of the Obra Dinn*:
   - **If you followed the [Mod Manager Setup](#mod-manager-setup) process**: 
      1. Launch r2modman/Thunderstore Mod Manager
      2. Find "Return of the Obra Dinn" in the games list, then click "Select Game"
      3. Click "Select profile" on the profile that contains your Archipelago mod
      4. Click on "Start modded"
   - **If you followed the [Manual Mod Setup](#manual-mod-setup) process**, locate your game executable and launch it.
5. Once the game has launched, click "Start".
6. If you're connecting to a session you've connected to before, the slot name and start date should be listed; click on
   it and you should connect automatically
7. If you're connecting to a new sessions, click on "New Connection". Enter the following details, then hit Connect
    1. Do not change the value in the first text box.
    2. Enter your slot name in the second text box
    3. Enter the server password in the third text box; if there's no password, clear the field
8. A warning will appear if the details you've entered match an existing connection's details. If you're certain that you need to create a new session connection, hit 'Connect' again. 
9. You should be connected!

## Development

### Requirements

- [PC Copy of *Return of the Obra Dinn*](https://obradinn.com/)

- [BepInEx x86 v5.4.23.2](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) or higher (but not v6)

- [.NET 8.0](https://dotnet.microsoft.com/en-us/download)

- A C#/.NET compliant IDE
    - [Visual Studio Community Edition](https://visualstudio.microsoft.com/free-developer-offers/),
      with the .NET desktop and Unity development packages
    - [JetBrains Rider](https://www.jetbrains.com/rider/download/)

- [Unity 2017.4.37](https://unity.com/releases/editor/whats-new/2017.4.37) (Optional, for creating assets)

- [AssetStudio v0.16.47](https://github.com/Perfare/AssetStudio/releases/tag/v0.16.47) (Optional, for updating assets)

- [dnSpy v6.5.1](https://github.com/dnSpyEx/dnSpy/releases/tag/v6.5.1) (Optional but recommended)

- [Runtime Unity Editor v5.4](https://github.com/ManlyMarco/RuntimeUnityEditor/releases/tag/v5.4) (Optional but
  recommended)

#### A Note on Unity

The version of Unity used to create *Return of the Obra Dinn* is a bit of a unique version.
It was the last to offer direct support for AssetBundles, it doesn't have direct support for Unity Package Manager, and
it seems to have been scrubbed from Unity's download archive. As such, there's a lot of trial and error to get a
development setup functional for modding this game. These instructions may not provide the best development workflow,
but it was what I was able to get working without any of the previously mentioned features. I'll continue to tweak the
setup, hopefully to improve it over time.

### Code Setup

1. Download your copy of Obra Dinn and locate the game folder.

    - GOG Galaxy: Typically located at `GOG Galaxy\Games\Return of the Obra Dinn`

2. Follow [these instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html) to install and
   setup BepInEx.
3. Once `BepInEx.cfg` is open,
   follow [these instructions](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html)
   to get your development environment setup.

    - Note: The current stable templates for
      BepInEx [don't work with .NET v8](https://github.com/BepInEx/BepInEx/issues/778),
      so you will need to install the bleeding edge templates instead of the stable ones listed in the instructions. You
      can do so by running the following command:

    ```shell
    dotnet new install BepInEx.Templates::2.0.0-be.4 --nuget-source https://nuget.bepinex.dev/v3/index.json
   ```

4. Once your development environment is set up, clone this project. Once cloned, add a `lib` folder to this project and
   copy over the following files from the `ObraDinn_Data/Managed` folder (found within the Obra Dinn game folder) to the
   `lib` folder:

    - `Assembly-CSharp.dll`
    - `UnityEngine.CoreModule.dll`
    - `UnityEngine.dll`
    - `UnityEngine.UI.dll`

5. Open a Developer Command Prompt in Visual Studio, then run

   ```shell
   MSBuild.exe ObraDinnArchipelago.csproj -property:Configuration=Debug
   ```

   This will generate a `bin/Debug/net35` folder with a `obradinn_archipelago.dll` file.
6. Copy the `obradinn_archipelago.dll` file to your `BepInEx/plugins` folder and then run the game. A message should
   appear in your console saying that the plugin was loaded!

### Asset Workflow Setup

1. Create a new Unity project
2. In your assets folder, add a new folder called `Editor`
3. In the `Editor` folder, create a new script called `Bundle`
4. Add the following code to the `Bundle` script:

   ```csharp
    using UnityEditor;
    using System.IO;
    
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                            BuildAssetBundleOptions.None,
                                            BuildTarget.StandaloneWindows);
            
            
            FileUtil.ReplaceFile("Assets/AssetBundles/archiassets", "[YOUR PROJECT'S ARCHIASSETS FILE PATH HERE]");
        }
    }   
   ```

5. If you have not done so already, select the prefabs and assets you want to include in the bundle and, in the Asset
   Labels windows, select the "archiassets" label (create it if not already done so).
6. Right-click your `Assets` folder and click on `Build AssetBundles`.
7. Repeat Steps 5-6 whenever you add a new asset or edit an existing one.

### dnSpy Setup and Use

1. Download [dnSpy v6.5.1](https://github.com/dnSpyEx/dnSpy/releases/tag/v6.5.1) if you have not done so already
2. In the downloaded file, run `dnSpy.exe`
3. In the *Assembly Explorer*, remove any files already there (Select, Right-click, `Del`)
4. Open your *Return of the Obra Dinn* game folder, then open `ObraDinn_Data/Managed`
5. Click and drag `Assembly-CSharp.dll` into the *Assembly Explorer*. The game files will be located under the
   `Assembly-Csharp.dll` dropdown, under `-`

### AssetStudio Setup and Use

1. Download [AssetStudio v0.16.47](https://github.com/Perfare/AssetStudio/releases/tag/v0.16.47) if you have not done so
   already
2. In the downloaded folder, run `AssetStudioGUI.exe`
3. Once the program opens, click `File`, then `Load Folder`. Select your `ObraDinn_Data` folder located in your
   *Return of the Obra Dinn* game folder, then hit `Select Folder`. The assets will be under the `Asset List` tab.

### Runtime Unity Editor Setup and Use

Follow [these instructions](https://github.com/ManlyMarco/RuntimeUnityEditor/tree/9d3d7123c2c831979a376dcc68d3e76f3d164924?tab=readme-ov-file#bepinex)
to get Runtime Unity Editor set up.

https://github.com/dnSpy/dnSpy-Unity-mono
https://github.com/dnSpyEx/dnSpy/wiki/Debugging-Unity-Games#turning-a-release-build-into-a-debug-build