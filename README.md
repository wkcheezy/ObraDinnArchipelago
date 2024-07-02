# Obra Dinn Archipelago

## Dev Setup

### Requirements

- Copy of *Return of the Obra Dinn*

- BepInEx x86 v5

- .NET v8

- Visual Studio Community Edition

1. Download your copy of Obra Dinn and locate the game folder.

    - GOG Galaxy: Typically located at `GOG Galaxy\Games\Return of the Obra Dinn`

2. Follow [these instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html) to install and setup BepInEx.
3. Once `BepInEx.cfg` is open, follow [these instructions](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html) to get your development environment setup.

    - Note: The current stable templates for BepInEx [don't work with .NET v8](https://github.com/BepInEx/BepInEx/issues/778), so you will need to install the bleeding edge templates instead of the stable ones listed in the instructions. You can do so by running the following command:

    ```cmd
    dotnet new install BepInEx.Templates::2.0.0-be.4 --nuget-source https://nuget.bepinex.dev/v3/index.json
    ```

4. Once your development environment is set up, clone this project. Once cloned, add a `lib` folder to this project and copy over the following files from the `ObraDinn_Data/Managed` folder (found within the Obra Dinn game folder) to the `lib` folder:

    - `Assembly-CSharp.dll`
    - `UnityEngine.CoreModule.dll`
    - `UnityEngine.dll`

5. Run `dotnet build` in a local terminal. This will generate a `bin/Debug/net35` folder with a `obradinn_archipelago.dll` file.
6. Copy the `obradinn_archipelago.dll` file to your `BepInEx/plugins` folder and then run the game. A message should appear in your console saying that the plugin was loaded!
