using Unity.Netcode;

/// <summary>
/// The movement axes captured for one simulation tick.
/// </summary>
public struct DemoMoveInput : IInputComponentData
{
    public int Horizontal;
    public int Vertical;
}