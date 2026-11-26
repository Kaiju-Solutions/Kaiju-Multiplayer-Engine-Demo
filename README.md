# Kaiju Multiplayer Engine Demo

Move an animated character around a shared level with WASD. The player prefab is a Unity [Netcode](https://docs.unity3d.com/Packages/com.unity.netcode@latest "Netcode") ghost. The same prefab and movement code run with or without [Kaiju Multiplayer Engine](https://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine").

## Getting Started

1. Install Unity 6.7 and [Git](https://git-scm.com "Git"), which Unity uses to download [Steamworks.NET](https://github.com/rlabrecque/Steamworks.NET "Steamworks.NET").
2. Open the project and load **Assets/Level.unity**. Enter Play mode and select **Host**. Use **Connect** in a second instance and **Disconnect** to leave.
3. To use Steam lobbies, install your licensed copy of Kaiju Multiplayer Engine through its [getting started guide](https://multiplayer.kaijusolutions.ca/manual/getting-started.html "Getting Started"). The demo also detects an embedded copy at `Packages/ca.kaijusolutions.multiplayer`. The demo supplies its own transport at play time, so you can skip the Netcode setup prompt. Use **Project Settings > Kaiju Multiplayer Engine** to configure the Steam app ID.

## Playing With and Without the Package

- **With Kaiju Multiplayer Engine**, Host creates a lobby and Connect searches for one. The demo displays each participant’s name and avatar and enables voice chat when the feature is available. You can test local sessions using [Multiplayer Play Mode](https://docs.unity3d.com/Packages/com.unity.multiplayer.playmode@latest "Multiplayer Play Mode").
- **With Netcode alone**, Host listens on UDP port 7777 and Connect joins the host on the same machine. Characters display their Netcode player numbers. A fresh clone runs this mode until you install Kaiju Multiplayer Engine. To switch back, remove the package or move the embedded copy outside the project with Unity closed.
- The [game assembly definition](Assets/GameAssembly.asmdef "GameAssembly") enables `CA_KAIJUSOLUTIONS_MULTIPLAYER` when the package is installed. [Menu.cs](Assets/Menu.cs "Menu") shows both session workflows. [DemoNetworking.cs](Assets/DemoNetworking.cs "DemoNetworking") spawns one owned ghost per connection and [Player.cs](Assets/Player.cs "Player") handles movement, animation, and player labels.

## License

The [MIT license](LICENSE.md "MIT License") applies to this repository's demo. You are free to reuse, modify, or share this demo project, but you cannot include [Kaiju Multiplayer Engine](http://multiplayer.kaijusolutions.ca "Kaiju Multiplayer Engine") itself when you do so. See the [Kaiju Multiplayer Engine license](https://multiplayer.kaijusolutions.ca/license "Kaiju Multiplayer Engine - License") for its details.

## Resources

The [overview video](https://youtu.be/KJ_WYKuBHsI "Kaiju Multiplayer Engine Overview") demonstrates the original edition of this demo.

Assets are from the [Platformer Kit](https://kenney.nl/assets/platformer-kit "Platformer Kit - Kenney") kit by [Kenney](https://kenney.nl "Kenney") under the [Creative Commons CC0 license](https://creativecommons.org/publicdomain/zero/1.0 "CC0 1.0 Universal").
