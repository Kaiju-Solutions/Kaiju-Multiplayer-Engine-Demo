using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
using KaijuSolutions.MultiplayerEngine;
using KaijuSolutions.MultiplayerEngine.NetcodeForGameObjects;
#endif
/// <summary>
/// A basic player which can move with WASD and displays the user's name and icon above them.
/// </summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(int.MaxValue)]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NetworkAnimator))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(PanelRenderer))]
public class Player : NetworkBehaviour
{
    /// <summary>
    /// Cache the <see cref="_animator"/>'s speed key for efficiency.
    /// </summary>
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    
    /// <summary>
    /// The speed the player moves at.
    /// </summary>
    private const float Speed = 5f;
    
    /// <summary>
    /// The rotation speed of the player.
    /// </summary>
    private const float RotationSpeed = 10f;
    
    /// <summary>
    /// The controller to move the player.
    /// </summary>
    private CharacterController _controller;
    
    /// <summary>
    /// Handle player animations.
    /// </summary>
    private Animator _animator;
    
    /// <summary>
    /// The main background element of the user's name and icon UI for positioning it as the player moves.
    /// </summary>
    private VisualElement _background;
    
    /// <summary>
    /// The label to update with the player's name.
    /// </summary>
    private Label _nameLabel;
    
    /// <summary>
    /// The main camera in the world.
    /// </summary>
    private Camera _camera;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
    /// <summary>
    /// The image to set with the user's icon.
    /// </summary>
    private Image _icon;
    
    /// <summary>
    /// The Steam user controlling this.
    /// </summary>
    private KaijuUser _user;
#endif
    /// <summary>
    /// The renderer for the UI.
    /// </summary>
    private PanelRenderer _renderer;
    
    /// <summary>
    /// Unity calls Awake when loading an instance of a script component.
    /// </summary>
    private void Awake()
    {
        // Get all components.
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera = FindAnyObjectByType<Camera>();
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        // Get the components to sync the Steam player with the Netcode for GameObjects player.
        // Note that normally, you would likely have these required through a "RequireComponent" attribute.
        // However, since this demo automatically sets up the scene and player prefab once Kaiju Multiplayer Engine is
        // installed, it can cause an annoying warning pop up saying the components were added every time the prefab is
        // opened. This way avoids that.
        _user = GetComponent<KaijuUser>();
        if (_user == null)
        {
            _user = gameObject.AddComponent<KaijuUser>();
        }
        
        if (gameObject.GetComponent<KaijuUserLink>() == null)
        {
            gameObject.AddComponent<KaijuUserLink>();
        }
#endif
        _renderer = GetComponent<PanelRenderer>();
        _renderer.RegisterUIReloadCallback(OnUIReload);
    }
    
    /// <summary>
    /// Called when a GameObject or component is about to be destroyed.
    /// </summary>
    public override void OnDestroy()
    {
        _renderer.UnregisterUIReloadCallback(OnUIReload);
        base.OnDestroy();
    }
    
    /// <summary>
    /// Called when the UI is rendered.
    /// </summary>
    /// <param name="renderer">The renderer for the UI.</param>
    /// <param name="root">The root element of the UI.</param>
    private void OnUIReload(PanelRenderer renderer, VisualElement root)
    {
        // Query UI elements.
        _background = root.Q("background");
        _nameLabel = _background.Q<Label>("name-label");
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        _icon = _background.Q<Image>("icon-image");
#endif
        // Manually run the callbacks once in case we already have the name or icon.
        SetName();
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        SetIcon();
#endif
    }
    
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        // Only move the player we control.
        if (!IsOwner)
        {
            return;
        }
        
        // Inline WASD movement using the input system.
        Vector3 move = new Vector3(Keyboard.current.dKey.isPressed ? Keyboard.current.aKey.isPressed ? 0f : 1f : Keyboard.current.aKey.isPressed ? -1f : 0f, 0f, Keyboard.current.wKey.isPressed ? Keyboard.current.sKey.isPressed ? 0f : 1f : Keyboard.current.sKey.isPressed ? -1f : 0f).normalized;
        
        // Look in the direction we are moving.
        if (!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.z, 0f))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), RotationSpeed * Time.deltaTime);
        }
        
        // Sync the animation and move the controller.
        _animator.SetFloat(SpeedHash, new Vector2(move.x, move.z).magnitude);
        float delta = Time.deltaTime;
        float speed = Speed * delta;
        move.x *= speed;
        move.z *= speed;
        move.y = Physics.gravity.y * delta;
        _controller.Move(move);
    }
    
    /// <summary>
    /// LateUpdate is called every frame, if the Behaviour is enabled.
    /// </summary>
    private void LateUpdate()
    {
        if (_background == null)
        {
            return;
        }
        
        // Ensure the UI is positioned above the player.
        Vector2 panelPos = RuntimePanelUtils.CameraTransformWorldToPanel(_background.panel, transform.position + new Vector3(0f, 1.1f, 0f), _camera);
        _background.style.left = panelPos.x;
        _background.style.top = panelPos.y;
    }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
    /// <summary>
    /// Called when a component of an active GameObject is first enabled.
    /// </summary>
    private void OnEnable()
    {
        // Bind callbacks.
        _user.OnUser += SetName;
        _user.OnIcon += SetIcon;
    }
    
    /// <summary>
    /// Called when a component itself is disabled or its parent GameObject is deactivated.
    /// </summary>
    private void OnDisable()
    {
        // Unbind the callbacks.
        _user.OnUser -= SetName;
        _user.OnIcon -= SetIcon;
    }
    
    /// <summary>
    /// Callback to set the UI's image to the Steam user's icon.
    /// </summary>
    private void SetIcon()
    {
        if (_icon == null) return;
        
        // Hide the image if it is NULL.
        _icon.image = _user.Icon;
        _icon.style.display = _icon.image != null ? DisplayStyle.Flex : DisplayStyle.None;
    }
#endif
    /// <summary>
    /// Callback to set the UI's text to the Steam user's name.
    /// </summary>
    private void SetName()
    {
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        if (_nameLabel != null) _nameLabel.text = _user.Name;
#else
        // When Kaiju Multiplayer Engine is not installed, there is no icon, and user the Netcode for GameObjects player ID as a placeholder name.
        if (_nameLabel != null) _nameLabel.text = $"Player {OwnerClientId + 1}";
#endif
    }
}