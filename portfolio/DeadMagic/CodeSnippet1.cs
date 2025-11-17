// LobbyController.cs
// Handles lobby creation and joining, and manages network events.
private async void HandleCreateLobby(string name, string difficulty)
{
    LobbyLogger.StatusMessage("Starting Networking...");
    string relayJoinCode = await lobbyNetManager.HostNetworkTask();
    LobbyLogger.StatusMessage("Creating Lobby...");
    await serviceManager.HostLobbyTask(name, relayJoinCode, difficulty);
    NetworkManager.OnClientConnectedCallback += _ => ResetReadyStatusRpc();
    NetworkManager.OnClientConnectedCallback += NetworkManagerOnOnClientConnectedCallback;
    CanStartGame(false);
}
// End

// LobbyNetworkManager.cs
public async Task<string> HostNetworkTask()
{
    RelayServerData relayServerData = await StartHostAllocation(4);
    StartNetworkingTask(true, relayServerData);
    return _relayJoinCode;
}
private async Task<RelayServerData> StartHostAllocation(int maxPlayers)
{
    try
    {
        var service = RelayService.Instance;
        Allocation allocation = await service.CreateAllocationAsync(maxPlayers);
        _relayJoinCode = await service.GetJoinCodeAsync(allocation.AllocationId);
        return allocation.ToRelayServerData("wss");
    }
    catch (Exception e)
    {
        Debug.LogError($"Failed to create relay allocation: {e.Message}");
        throw;
    }
}
// End

// LobbyServiceManager.cs
public async Task HostLobbyTask(string lobbyName, string relayJoinCode, string difficulty)
{
    var options = new CreateLobbyOptions
    {
        Data = new Dictionary<string, DataObject>
                {
                    {
                        "RelayJoinCode",
                        new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode)
                    },
                    {
                "HostName",
                new DataObject(
                    DataObject.VisibilityOptions.Public,
                    AuthenticationService.Instance.PlayerName
                )
            },
            {
                "Difficulty",
                new DataObject(DataObject.VisibilityOptions.Public, difficulty)
            }
        },
    };
    Lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, options);
    StartHeartbeat();
}
// End