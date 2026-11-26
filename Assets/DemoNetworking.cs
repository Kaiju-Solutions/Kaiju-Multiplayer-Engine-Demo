using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
using KaijuSolutions.MultiplayerEngine.Netcode;
#endif
/// <summary>
/// Spawns an owned ghost for each connection in the host world.
/// </summary>
public sealed class DemoNetworking : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    private readonly List<(Connection Connection, Player Player)> _players = new();
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
    private KaijuTransport _transport;

    private void Awake()
    {
        _transport = FindAnyObjectByType<KaijuTransport>();
        if (_transport == null)
        {
            _transport = gameObject.AddComponent<KaijuTransport>();
        }
    }
#endif
    private void Update()
    {
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        NetcodeWorld server = _transport.ServerWorld;
#else
        NetcodeWorld server = ClientServerBootstrap.ServerWorld;
#endif
        for (int i = _players.Count - 1; i >= 0; i--)
        {
            (Connection connection, Player player) = _players[i];
            if (connection.World != server || !connection.IsValid() || connection.GetConnectionState() != ConnectionState.State.Connected)
            {
                if (player != null)
                {
                    Destroy(player.gameObject);
                }
                _players.RemoveAt(i);
            }
        }
        if (server == null || !server.IsCreated || !server.Listening())
        {
            return;
        }
        Spawn(server.LocalConnection);
        foreach (Connection connection in server.AllConnections)
        {
            Spawn(connection);
        }
    }

    private void Spawn(Connection connection)
    {
        if (!connection.IsValid() || connection.GetConnectionState() != ConnectionState.State.Connected)
        {
            return;
        }
        foreach ((Connection existing, Player _) in _players)
        {
            if (existing.World == connection.World && existing.ConnectionEntity == connection.ConnectionEntity)
            {
                return;
            }
        }
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        // The transport associates the verified lobby peer with its Netcode connection.
        if (!connection.World.EntityManager.HasComponent<KaijuPeer>(connection.ConnectionEntity))
        {
            return;
        }
        ulong peer = connection.World.EntityManager.GetComponentData<KaijuPeer>(connection.ConnectionEntity).Value;
#endif
        Player player = Instantiate(playerPrefab, transform.position, playerPrefab.transform.rotation);
        player.Ghost.OwnerNetworkId = connection.NetworkId;
#if CA_KAIJUSOLUTIONS_MULTIPLAYER
        player.Peer.Value = peer;
#endif
        _players.Add((connection, player));
    }
}