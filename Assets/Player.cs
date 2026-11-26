using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
using KaijuSolutions.MultiplayerEngine;
#endif
/// <summary>
/// Moves an owned player through the level and presents its replicated state.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(Animator))]
public sealed partial class Player : GhostBehaviour
{
    [SerializeField] private VisualTreeAsset nameTemplate;
    private GhostInput<DemoMoveInput> _input;
    private GhostField<float> _speed;
    private CharacterController _controller;
    private Animator _animator;
    private PanelRenderer _panel;
    private Camera _camera;
    private VisualElement _background;
    private Label _name;
    private Image _icon;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
    /// <summary>
    /// The lobby peer assigned by the server to this ghost.
    /// </summary>
    public GhostField<ulong> Peer;
    private KaijuUser _user;
#endif
    public override void Awake()
    {
        base.Awake();
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;
        _panel = FindAnyObjectByType<Menu>().GetComponent<PanelRenderer>();
        _panel.RegisterUIReloadCallback(BindPanel);
    }

    public override void OnDestroy()
    {
        if (_panel != null)
        {
            _panel.UnregisterUIReloadCallback(BindPanel);
        }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        if (_user != null)
        {
            _user.OnUser -= RefreshProfile;
            _user.OnIcon -= RefreshProfile;
        }
#endif
        _background?.RemoveFromHierarchy();
        base.OnDestroy();
    }

    private void BindPanel(PanelRenderer renderer, VisualElement root, int version)
    {
        _background?.RemoveFromHierarchy();
        _background = nameTemplate.Instantiate().Q("background");
        _background.pickingMode = PickingMode.Ignore;
        root.Insert(0, _background);
        _name = _background.Q<Label>("name-label");
        _icon = _background.Q<Image>("icon-image");
        _icon.style.display = DisplayStyle.None;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        RefreshProfile();
#endif
    }

    /// <summary>
    /// Capture local keyboard input for Netcode to send to the server.
    /// </summary>
    public override void PredictionGatherInput(PredictionGatherInputContext context)
    {
        DemoMoveInput input = default;
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            input.Horizontal = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
            input.Vertical = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
        }
        _input.Capture(input);
    }

    /// <summary>
    /// Apply the input for this tick on the server and the predicting client.
    /// </summary>
    public override void PredictionUpdate(PredictionUpdateContext context)
    {
        DemoMoveInput input = _input.GetForCurrentTick();
        Vector3 direction = new Vector3(input.Horizontal, 0f, input.Vertical).normalized;
        _speed.Value = direction.magnitude;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10f * context.TickedDeltaTime);
        }
        _controller.Move((direction * 5f + Physics.gravity) * context.TickedDeltaTime);
    }

    private void LateUpdate()
    {
        if (!Ghost.IsSpawned)
        {
            return;
        }
        _animator.SetFloat("Speed", _speed.Value);
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        if (Peer.Value != 0 && _user == null)
        {
            _user = gameObject.AddComponent<KaijuUser>();
            _user.OnUser += RefreshProfile;
            _user.OnIcon += RefreshProfile;
            _user.PeerId = new KaijuPeerId(Peer.Value);
            RefreshProfile();
        }
#endif
        if (_background?.panel == null || _camera == null)
        {
            return;
        }
        Vector2 position = RuntimePanelUtils.CameraTransformWorldToPanel(_background.panel, transform.position + Vector3.up * 1.1f, _camera);
        _background.style.left = position.x;
        _background.style.top = position.y;
#if !CA_KAIJUSOLUTIONS_MULTIPLAYER
        _name.text = $"Player {Ghost.OwnerNetworkId.Value}";
#endif
    }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
    private void RefreshProfile()
    {
        if (_user != null && _name != null)
        {
            _name.text = _user.Name;
            _icon.image = _user.Icon;
            _icon.style.display = _icon.image == null ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
#endif
}