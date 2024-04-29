using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class CreateAndJoinRelay : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinRoomInput;

    public delegate void RelayHandler(bool isHost, string joinCode);
    public static RelayHandler OnRelayJoined;

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code: " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            OnRelayJoined(true, joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async void JoinRelay()
    {
        try
        {
            Debug.Log("Joining Relay with code: " + joinRoomInput.text);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinRoomInput.text);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            OnRelayJoined(false, joinRoomInput.text);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
