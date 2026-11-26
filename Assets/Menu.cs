using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UIElements;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
using KaijuSolutions.MultiplayerEngine;
#endif
/// <summary>
/// Hosts, joins, or leaves a demo session using the available networking backend.
/// </summary>
[RequireComponent(typeof(PanelRenderer))]
public sealed class Menu : MonoBehaviour
{
    private Button _host;
    private Button _connect;
    private Button _disconnect;
#if !CA_KAIJUSOLUTIONS_MULTIPLAYER
    private Connection _connection;
    private bool _started;
#endif
    private void OnEnable()
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(BindPanel);
#if CA_KAIJUSOLUTIONS_MULTIPLAYER && !DISABLE_KAIJU_VOICE
        KaijuMultiplayerManager.OnStart += EnableVoice;
        KaijuMultiplayerManager.OnShutdown += DisableVoice;
#endif
    }

    private void OnDisable()
    {
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(BindPanel);
        UnbindButtons();
#if CA_KAIJUSOLUTIONS_MULTIPLAYER && !DISABLE_KAIJU_VOICE
        KaijuMultiplayerManager.OnStart -= EnableVoice;
        KaijuMultiplayerManager.OnShutdown -= DisableVoice;
#endif
    }

    private void BindPanel(PanelRenderer renderer, VisualElement root, int version)
    {
        UnbindButtons();
        _host = root.Q<Button>("host-button");
        _connect = root.Q<Button>("connect-button");
        _disconnect = root.Q<Button>("disconnect-button");
        _host.clicked += Host;
        _connect.clicked += Connect;
        _disconnect.clicked += Disconnect;
        Update();
    }

    private void UnbindButtons()
    {
        if (_host != null)
        {
            _host.clicked -= Host;
            _connect.clicked -= Connect;
            _disconnect.clicked -= Disconnect;
        }
    }

    private void Update()
    {
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        bool active = KaijuMultiplayerManager.InLobby;
#else
        if (_started && (!_connection.IsValid() || _connection.GetConnectionState() == ConnectionState.State.Disconnected))
        {
            Disconnect();
        }
        bool active = _started;
#endif
        if (_host != null)
        {
            _host.style.display = _connect.style.display = active ? DisplayStyle.None : DisplayStyle.Flex;
            _disconnect.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void Host()
    {
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        _ = KaijuMultiplayerManager.Host();
#else
        _connection = Netcode.StartAsHost(NetworkEndpoint.AnyIpv4.WithPort(7777));
        _started = _connection.IsValid();
        if (!_started)
        {
            Netcode.Shutdown();
        }
#endif
    }

    private void Connect()
    {
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        _ = KaijuMultiplayerManager.FindLobby();
#else
        _connection = Netcode.Connect(NetworkEndpoint.LoopbackIpv4.WithPort(7777));
        _started = _connection.IsValid();
        if (!_started)
        {
            Netcode.Shutdown();
        }
#endif
    }

    private void Disconnect()
    {
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        KaijuMultiplayerManager.Shutdown();
#else
        _started = false;
        _connection = default;
        Netcode.Shutdown();
#endif
    }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER && !DISABLE_KAIJU_VOICE
    private static void EnableVoice() => KaijuMultiplayerManager.MicEnabled = true;

    private static void DisableVoice() => KaijuMultiplayerManager.MicEnabled = false;
#endif
}