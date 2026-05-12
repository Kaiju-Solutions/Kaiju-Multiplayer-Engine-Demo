# Kaiju Multiplayer Engine Demo

**This provides a simple demo showing [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine") with Unity's [Netcode for GameObjects](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@latest "Netcode for GameObjects"). This was used in the [overview video](https://youtu.be/KJ_WYKuBHsI "Kaiju Multiplayer Engine Overview").**

## Getting Started

1. Install [Git](https://git-scm.com "Git") so [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET "Steamworks.NET") can automatically be installed into the project.
2. Clone or download this project and open it.
3. Install [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine") following the [getting started instructions](https://multiplayer.kaijusolutions.ca/manual/getting-started.html "Kaiju Multiplayer Engine - Getting Started").
4. If prompted to reload your scene, click to do so. This popup is because the demo project will automatically set up [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine") in the scene and on the player prefab by adding the needed GameObjects and components once it is installed.

## Playing

- If you play before installing [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine"), it will use [Netcode for GameObjects](https://docs.unity3d.com/Packages/com.unity.netcode.gameobjects@latest "Netcode for GameObjects") directly, identical to the beginning of the [overview video](https://youtu.be/KJ_WYKuBHsI "Kaiju Multiplayer Engine Overview").
- Once [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine") is installed, playing will behave the same as the end of the [overview video](https://youtu.be/KJ_WYKuBHsI "Kaiju Multiplayer Engine Overview").
- These changes are automatically made due to the [`GameAssembly`](/Assets/GameAssembly.asmdef "GameAssembly") [assembly definition file](https://docs.unity3d.com/Manual/assembly-definition-files.html "Unity - Organizing scripts into assemblies") with its [conditional including of the Kaiju Multiplayer Engine assembly.](https://docs.unity3d.com/Manual/assembly-definition-includes.html "Unity - https://docs.unity3d.com/Manual/assembly-definition-includes.html").

## License

The [MIT license](LICENSE.md "MIT License") applies to this repository's demo. You are free to reuse, modify, or share this demo project, but you cannot include [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine") itself when you do so. See the [Kaiju Multiplayer Engine license](https://multiplayer.kaijusolutions.ca/license "Kaiju Multiplayer Engine - License") for its details.

## Resources

Assets are from the [Platformer Kit](https://kenney.nl/assets/platformer-kit "Platformer Kit - Kenney") kit by [Kenney](https://kenney.nl "Kenney") under the [Creative Commons CC0 license](https://creativecommons.org/publicdomain/zero/1.0 "CC0 1.0 Universal").