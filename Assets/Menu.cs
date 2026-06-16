using UnityEngine;
using UnityEngine.UIElements;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
using KaijuSolutions.MultiplayerEngine;
#else
using Unity.Netcode;
#endif
/// <summary>
/// Handle the menu buttons.
/// </summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(int.MaxValue)]
[RequireComponent(typeof(PanelRenderer))]
public class Menu : MonoBehaviour
{
    /// <summary>
    /// Button to host a game.
    /// </summary>
    private Button _hostButton;
    
    /// <summary>
    /// Button to connect to a game. When using Steam, this will search for an open game, and host if one is not found.
    /// </summary>
    private Button _connectButton;
    
    /// <summary>
    /// Button to leave a game.
    /// </summary>
    private Button _disconnectButton;
    
    /// <summary>
    /// The renderer for the UI.
    /// </summary>
    private PanelRenderer _renderer;
    
    /// <summary>
    /// Unity calls Awake when loading an instance of a script component.
    /// </summary>
    private void Awake()
    {
        _renderer = GetComponent<PanelRenderer>();
        _renderer.RegisterUIReloadCallback(OnUIReload);
    }
    
    /// <summary>
    /// Called when a GameObject or component is about to be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        _renderer.UnregisterUIReloadCallback(OnUIReload);
        CleanupButtons();
    }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
    /// <summary>
    /// Called when the UI is rendered.
    /// </summary>
    /// <param name="renderer">The renderer for the UI.</param>
    /// <param name="root">The root element of the UI.</param>
    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        // Unbind any previous button calls.
        CleanupButtons();
        
        // Get the buttons.
        _hostButton = root.Q<Button>("host-button");
        _connectButton = root.Q<Button>("connect-button");
        _disconnectButton = root.Q<Button>("disconnect-button");
        
        // Bind all buttons. The "Bindable" variations return nothing, making them easy to bind like this.
        // Use the non-"Bindable" variations (i.e. "Host" and "FindLobby") if you want a return Boolean value to check.
        _hostButton.clicked += KaijuMultiplayerManager.HostBindable;
        _connectButton.clicked += KaijuMultiplayerManager.FindLobbyBindable;
        _disconnectButton.clicked += KaijuMultiplayerManager.Shutdown;
    }
    
    /// <summary>
    /// Method to clean up buttons.
    /// </summary>
    private void CleanupButtons()
    {
        if (_hostButton != null) _hostButton.clicked -= KaijuMultiplayerManager.HostBindable;
        if (_connectButton != null) _connectButton.clicked -= KaijuMultiplayerManager.FindLobbyBindable;
        if (_disconnectButton != null) _disconnectButton.clicked -= KaijuMultiplayerManager.Shutdown;
    }
    
    /// <summary>
    /// Called when a component of an active GameObject is first enabled.
    /// </summary>
    private void OnEnable()
    {
        // Bind callbacks so the UI displays properly based on if we are in a game or not.
        KaijuMultiplayerManager.OnStart += OnConnect;
        KaijuMultiplayerManager.OnShutdown += OnDisconnect;
        
        // Ensure the UI is initialized into the proper state by running the correct callback once manually.
        if (KaijuMultiplayerManager.InLobby)
        {
            OnConnect();
        }
        else
        {
            OnDisconnect();
        }
    }
    
    /// <summary>
    /// Called when a component itself is disabled or its parent GameObject is deactivated.
    /// </summary>
    private void OnDisable()
    {
        // Unbind callbacks.
        KaijuMultiplayerManager.OnStart -= OnConnect;
        KaijuMultiplayerManager.OnShutdown -= OnDisconnect;
    }
    
    /// <summary>
    /// Callback for when we are connected to a game.
    /// </summary>
    private void OnConnect()
    {
        // In this demo, when in a game, voice chat is enabled.
        KaijuMultiplayerManager.MicEnabled = true;
        
        // Sync the UI buttons.
        _hostButton.style.display = _connectButton.style.display = DisplayStyle.None;
        _disconnectButton.style.display = DisplayStyle.Flex;
    }
    
    /// <summary>
    /// Callback for when we leave a game.
    /// </summary>
    private void OnDisconnect()
    {
        // Turn off our mic in the menu.
        KaijuMultiplayerManager.MicEnabled = false;
        
        // Sync the UI buttons.
        _disconnectButton.style.display = DisplayStyle.None;
        _hostButton.style.display = _connectButton.style.display = DisplayStyle.Flex;
    }
#else
    /// <summary>
    /// Called when the UI is rendered.
    /// </summary>
    /// <param name="renderer">The renderer for the UI.</param>
    /// <param name="root">The root element of the UI.</param>
    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        // Unbind any previous button calls.
        CleanupButtons();
        
        // Get the buttons.
        _hostButton = root.Q<Button>("host-button");
        _connectButton = root.Q<Button>("connect-button");
        _disconnectButton = root.Q<Button>("disconnect-button");
        
        // Bind the button calls.
        _hostButton.clicked += Host;
        _connectButton.clicked += Connect;
        _disconnectButton.clicked += Disconnect;
    }
    
    /// <summary>
    /// Method to clean up buttons.
    /// </summary>
    private void CleanupButtons()
    {
        if (_hostButton != null) _hostButton.clicked -= Host;
        if (_connectButton != null) _connectButton.clicked -= Connect;
        if (_disconnectButton != null) _disconnectButton.clicked -= Disconnect;
    }
    
    /// <summary>
    /// Called when a component of an active GameObject is first enabled.
    /// </summary>
    private void OnEnable()
    {
        if (NetworkManager.Singleton == null) throw new("NetworkManager not found");
        NetworkManager.Singleton.OnClientStarted += OnConnect;
        NetworkManager.Singleton.OnClientStopped += OnDisconnect;
        if (NetworkManager.Singleton.IsConnectedClient) OnConnect(); else OnDisconnect();
    }
    
    /// <summary>
    /// Called when a component itself is disabled or its parent GameObject is deactivated.
    /// </summary>
    private void OnDisable()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientStarted -= OnConnect;
        NetworkManager.Singleton.OnClientStopped -= OnDisconnect;
    }
    
    /// <summary>
    /// Since Netcode for GameObjects' "StartHost" method returns a Boolean, we can't bind to it directly and need this helper method.
    /// </summary>
    private static void Host()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    /// <summary>
    /// Since Netcode for GameObjects' "StartClient" method returns a Boolean, we can't bind to it directly and need this helper method.
    /// </summary>
    private static void Connect()
    {
        NetworkManager.Singleton.StartClient();
    }
    
    /// <summary>
    /// Since Netcode for GameObjects' "Shutdown" method taks a Boolean input, we can't bind to it directly and need this helper method.
    /// </summary>
    private static void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
    }
    
    /// <summary>
    /// Callback for when we are connected to a game.
    /// </summary>
    private void OnConnect()
    {
        // Sync the UI buttons.
        _hostButton.style.display = _connectButton.style.display = DisplayStyle.None;
        _disconnectButton.style.display = DisplayStyle.Flex;
    }
    
    /// <summary>
    /// Callback for when we leave a game.
    /// <param name="_">Netcode for GameObjects has a parameter for whether this was a disconnect as a server or a client which this can ignore.</param>
    /// </summary>
    private void OnDisconnect(bool _ = false)
    {
        // Sync the UI buttons.
        _disconnectButton.style.display = DisplayStyle.None;
        _hostButton.style.display = _connectButton.style.display = DisplayStyle.Flex;
    }
#endif
}