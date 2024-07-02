# Obra Dinn Archipelago

## Dev Requirements

- [PC Copy of *Return of the Obra Dinn*](https://obradinn.com/)

- [BepInEx x86 v5.4.23.2](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2)

- [.NET 8.0](https://dotnet.microsoft.com/en-us/download)

- [Visual Studio Community Edition](https://visualstudio.microsoft.com/free-developer-offers/),
with the .NET desktop and Unity development packages

- [Unity 2017.4.37](https://unity.com/releases/editor/whats-new/2017.4.37) (Optional, for creating assets)

- [AssetStudio v0.16.47](https://github.com/Perfare/AssetStudio/releases/tag/v0.16.47) (Optional, for updating assets)

- [dnSpy v6.5.1](https://github.com/dnSpyEx/dnSpy/releases/tag/v6.5.1) (Optional but recommended)

- [Runtime Unity Editor v5.4](https://github.com/ManlyMarco/RuntimeUnityEditor/releases/tag/v5.4) (Optional but 
recommended)

## A Note on Unity

The version of Unity used to create *Return of the Obra Dinn* is a bit of a unique version.
It was the last to offer direct support for AssetBundles, it doesn't have direct support for Unity Package Manager, and
it seems to have been scrubbed from Unity's download archive. As such, there's a lot of trial and error to get a
development setup functional for modding this game. These instructions may not provide the best development workflow,
but it was what I was able to get working without any of the previously mentioned features. I'll continue to tweak the
setup, hopefully to improve it over time.

## Development Setup

1. Download your copy of Obra Dinn and locate the game folder.

    - GOG Galaxy: Typically located at `GOG Galaxy\Games\Return of the Obra Dinn`

2. Follow [these instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html) to install and 
setup BepInEx.
3. Once `BepInEx.cfg` is open, follow [these instructions](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html) 
to get your development environment setup.

    - Note: The current stable templates for BepInEx [don't work with .NET v8](https://github.com/BepInEx/BepInEx/issues/778), 
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

5. Open a Developer Command Prompt in Visual Studio, then run `MSBuild.exe MyProj.proj -property:Configuration=Debug`.
This will generate a `bin/Debug/net35` folder with a `obradinn_archipelago.dll` file.
6. Copy the `obradinn_archipelago.dll` file to your `BepInEx/plugins` folder and then run the game. A message should
appear in your console saying that the plugin was loaded!

## Asset Workflow Setup

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
        }
    }   
   ```

5. Right-click your `Assets` folder and click on `Build AssetBundles`.
6. In the new `AssetBundles` folder, copy the `archiassets` file into your `assets` folder.
7. Repeat Steps 5-6 whenever you add a new asset or edit an existing one.

## dnSpy Setup and Use

1. Download [dnSpy v6.5.1](https://github.com/dnSpyEx/dnSpy/releases/tag/v6.5.1) if you have not done so already
2. In the downloaded file, run `dnSpy.exe`
3. In the *Assembly Explorer*, remove any files already there (Select, Right-click, `Del`)
4. Open your *Return of the Obra Dinn* game folder, then open `ObraDinn_Data/Managed`
5. Click and drag `Assembly-CSharp.dll` into the *Assembly Explorer*. The game files will be located under the
`Assembly-Csharp.dll` dropdown, under `-`

## AssetStudio Setup and Use

1. Download [AssetStudio v0.16.47](https://github.com/Perfare/AssetStudio/releases/tag/v0.16.47) if you have not done so
already
2. In the downloaded folder, run `AssetStudioGUI.exe`
3. Once the program opens, click `File`, then `Load Folder`. Select your `ObraDinn_Data` folder located in your
*Return of the Obra Dinn* game folder, then hit `Select Folder`. The assets will be under the `Asset List` tab.

## Runtime Unity Editor Setup and Use

Follow [these instructions](https://github.com/ManlyMarco/RuntimeUnityEditor/tree/9d3d7123c2c831979a376dcc68d3e76f3d164924?tab=readme-ov-file#bepinex)
to get Runtime Unity Editor set up.
