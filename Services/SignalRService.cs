using Microsoft.AspNetCore.SignalR.Client;
using FrontEndHealthPets.Helpers;

namespace FrontEndHealthPets.Services;

/// <summary>
/// Service for managing SignalR connection for video calls
/// </summary>
public class SignalRService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    private bool _isConnecting;

    // Events for video call signaling
    public event Action<IncomingCallData>? OnIncomingCall;
    public event Action<CallStartedData>? OnCallStarted;
    public event Action<CallAcceptedData>? OnCallAccepted;
    public event Action<CallRejectedData>? OnCallRejected;
    public event Action<CallEndedData>? OnCallEnded;
    public event Action<string>? OnCallError;

    // WebRTC signaling events
    public event Action<OfferData>? OnOfferReceived;
    public event Action<AnswerData>? OnAnswerReceived;
    public event Action<IceCandidateData>? OnIceCandidateReceived;

    // Media control events
    public event Action<MediaToggleData>? OnRemoteVideoToggled;
    public event Action<MediaToggleData>? OnRemoteAudioToggled;
    public event Action<int>? OnScreenShareStarted;
    public event Action<int>? OnScreenShareStopped;
    public event Action<int>? OnRecordingStarted;
    public event Action<int>? OnRecordingStopped;

    // Connection state
    public event Action<HubConnectionState>? OnConnectionStateChanged;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;

    public async Task ConnectAsync()
    {
        if (_isConnecting || IsConnected)
            return;

        _isConnecting = true;

        try
        {
            var token = Settings.AuthToken;
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No authentication token available");
            }

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(Constants.VideoCallHubUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                })
                .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                .Build();

            RegisterHandlers();

            _hubConnection.Closed += async (error) =>
            {
                OnConnectionStateChanged?.Invoke(HubConnectionState.Disconnected);
                System.Diagnostics.Debug.WriteLine($"[SignalR] Connection closed: {error?.Message}");
                await Task.CompletedTask;
            };

            _hubConnection.Reconnecting += (error) =>
            {
                OnConnectionStateChanged?.Invoke(HubConnectionState.Reconnecting);
                System.Diagnostics.Debug.WriteLine($"[SignalR] Reconnecting: {error?.Message}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                OnConnectionStateChanged?.Invoke(HubConnectionState.Connected);
                System.Diagnostics.Debug.WriteLine($"[SignalR] Reconnected: {connectionId}");
                return Task.CompletedTask;
            };

            await _hubConnection.StartAsync();
            OnConnectionStateChanged?.Invoke(HubConnectionState.Connected);
            System.Diagnostics.Debug.WriteLine("[SignalR] Connected to VideoCallHub");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SignalR] Connection error: {ex.Message}");
            throw;
        }
        finally
        {
            _isConnecting = false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
            _hubConnection = null;
        }
    }

    private void RegisterHandlers()
    {
        if (_hubConnection == null) return;

        // Incoming call notification
        _hubConnection.On<IncomingCallData>("IncomingCall", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnIncomingCall?.Invoke(data));
        });

        // Call lifecycle events
        _hubConnection.On<CallStartedData>("CallStarted", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnCallStarted?.Invoke(data));
        });

        _hubConnection.On<CallAcceptedData>("CallAccepted", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnCallAccepted?.Invoke(data));
        });

        _hubConnection.On<CallRejectedData>("CallRejected", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnCallRejected?.Invoke(data));
        });

        _hubConnection.On<CallEndedData>("CallEnded", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnCallEnded?.Invoke(data));
        });

        _hubConnection.On<string>("CallError", (message) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnCallError?.Invoke(message));
        });

        // WebRTC signaling
        _hubConnection.On<OfferData>("ReceiveOffer", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnOfferReceived?.Invoke(data));
        });

        _hubConnection.On<AnswerData>("ReceiveAnswer", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnAnswerReceived?.Invoke(data));
        });

        _hubConnection.On<IceCandidateData>("ReceiveIceCandidate", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnIceCandidateReceived?.Invoke(data));
        });

        // Media control
        _hubConnection.On<MediaToggleData>("RemoteVideoToggled", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnRemoteVideoToggled?.Invoke(data));
        });

        _hubConnection.On<MediaToggleData>("RemoteAudioToggled", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnRemoteAudioToggled?.Invoke(data));
        });

        // Screen sharing
        _hubConnection.On<ScreenShareData>("ScreenShareStarted", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnScreenShareStarted?.Invoke(data.CallId));
        });

        _hubConnection.On<ScreenShareData>("ScreenShareStopped", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnScreenShareStopped?.Invoke(data.CallId));
        });

        // Recording
        _hubConnection.On<RecordingData>("RecordingStarted", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnRecordingStarted?.Invoke(data.CallId));
        });

        _hubConnection.On<RecordingData>("RecordingStopped", (data) =>
        {
            MainThread.BeginInvokeOnMainThread(() => OnRecordingStopped?.Invoke(data.CallId));
        });
    }

    #region Hub Methods

    public async Task StartCallAsync(int conversacionId, bool withVideo, bool withAudio)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("StartCall", conversacionId, withVideo, withAudio);
    }

    public async Task AcceptCallAsync(int callId, bool withVideo, bool withAudio)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("AcceptCall", callId, withVideo, withAudio);
    }

    public async Task RejectCallAsync(int callId, string? reason = null)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("RejectCall", callId, reason);
    }

    public async Task EndCallAsync(int callId, string? reason = null)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("EndCall", callId, reason);
    }

    public async Task SendOfferAsync(int callId, string sdpOffer)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("SendOffer", callId, sdpOffer);
    }

    public async Task SendAnswerAsync(int callId, string sdpAnswer)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("SendAnswer", callId, sdpAnswer);
    }

    public async Task SendIceCandidateAsync(int callId, string candidate)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("SendIceCandidate", callId, candidate);
    }

    public async Task ToggleVideoAsync(int callId, bool enabled)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("ToggleVideo", callId, enabled);
    }

    public async Task ToggleAudioAsync(int callId, bool enabled)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("ToggleAudio", callId, enabled);
    }

    public async Task StartScreenShareAsync(int callId)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("StartScreenShare", callId);
    }

    public async Task StopScreenShareAsync(int callId)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("StopScreenShare", callId);
    }

    public async Task StartRecordingAsync(int callId)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("StartRecording", callId);
    }

    public async Task StopRecordingAsync(int callId)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("Not connected to SignalR hub");

        await _hubConnection.InvokeAsync("StopRecording", callId);
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}

#region Data Classes

public record IncomingCallData(
    int CallId,
    int ConversacionId,
    string CallerType,
    int CallerId,
    string CallerName,
    string? CallerImage,
    bool WithVideo,
    bool WithAudio,
    DateTime StartDate
);

public record CallStartedData(
    int CallId,
    int ConversacionId,
    string CalleeType,
    int CalleeId,
    string CalleeName,
    string? CalleeImage,
    bool WithVideo,
    bool WithAudio
);

public record CallAcceptedData(
    int CallId,
    bool WithVideo,
    bool WithAudio
);

public record CallRejectedData(
    int CallId,
    string Reason
);

public record CallEndedData(
    int CallId,
    string Reason
);

public record OfferData(
    int CallId,
    string Sdp
);

public record AnswerData(
    int CallId,
    string Sdp
);

public record IceCandidateData(
    int CallId,
    string Candidate
);

public record MediaToggleData(
    int CallId,
    bool Enabled
);

public record ScreenShareData(
    int CallId
);

public record RecordingData(
    int CallId
);

#endregion
