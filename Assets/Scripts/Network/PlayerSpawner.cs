using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefabA;
    [SerializeField] private GameObject playerPrefabB;

    public delegate void SpawnPlayerHandler(ulong clientId);
    public static SpawnPlayerHandler OnPlayerSpawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 0);
        }
        else
        {
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, 1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId, int prefabId)
    {
        GameObject newPlayer;
        if (prefabId == 0)
            newPlayer = Instantiate(playerPrefabA);
        else
            newPlayer = Instantiate(playerPrefabB);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);
        OnPlayerSpawned(clientId);
    }
}
