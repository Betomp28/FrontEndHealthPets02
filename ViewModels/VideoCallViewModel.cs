using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FrontEndHealthPets.Services;
using System.Timers;

namespace FrontEndHealthPets.ViewModels;

public partial class VideoCallViewModel : ObservableObject
{
    private readonly SignalRService _signalRService;
    private System.Timers.Timer? _durationTimer;
    private DateTime? _callStartTime;

    // Call data
    public int ConversacionId { get; set; }
    public int? CallId { get; set; }
    public bool IsCaller { get; set; }

    [ObservableProperty]
    private string participantName = "";

    [ObservableProperty]
    private string participantImage = "";

    [ObservableProperty]
    private string callStatusText = "Conectando...";

    [ObservableProperty]
    private string callDuration = "00:00";

    [ObservableProperty]
    private bool isConnected;

    [ObservableProperty]
    private bool isIncomingCall;

    [ObservableProperty]
    private string callerName = "";

    [ObservableProperty]
    private string callerInitial = "?";

    [ObservableProperty]
    private bool showControls = true;

    [ObservableProperty]
    private bool canGoBack = true;

    // Media state
    [ObservableProperty]
    private bool isVideoEnabled = true;

    [ObservableProperty]
    private bool isAudioEnabled = true;

    [ObservableProperty]
    private bool isScreenSharing;

    [ObservableProperty]
    private bool isRecording;

    // Button colors
    public Color VideoButtonColor => IsVideoEnabled ? Color.FromArgb("#333") : Color.FromArgb("#FF3B30");
    public Color AudioButtonColor => IsAudioEnabled ? Color.FromArgb("#333") : Color.FromArgb("#FF3B30");
    public Color ScreenShareButtonColor => IsScreenSharing ? Color.FromArgb("#6B4EE6") : Color.FromArgb("#333");
    public Color RecordButtonColor => IsRecording ? Color.FromArgb("#FF3B30") : Color.FromArgb("#333");

    // Button icons
    public string VideoButtonIcon => IsVideoEnabled ? "V" : "/";
    public string AudioButtonIcon => IsAudioEnabled ? "M" : "/";

    // Events to communicate with the page (for WebView control)
    public event Action<string>? OnExecuteJavaScript;
    public event Action? OnCallEnded;

    public VideoCallViewModel(SignalRService signalRService)
    {
        _signalRService = signalRService;
        SetupSignalRHandlers();
    }

    private void SetupSignalRHandlers()
    {
        _signalRService.OnCallAccepted += OnCallAcceptedHandler;
        _signalRService.OnCallRejected += OnCallRejectedHandler;
        _signalRService.OnCallEnded += OnCallEndedHandler;
        _signalRService.OnCallError += OnCallErrorHandler;

        _signalRService.OnOfferReceived += OnOfferReceivedHandler;
        _signalRService.OnAnswerReceived += OnAnswerReceivedHandler;
        _signalRService.OnIceCandidateReceived += OnIceCandidateReceivedHandler;

        _signalRService.OnRemoteVideoToggled += OnRemoteVideoToggledHandler;
        _signalRService.OnRemoteAudioToggled += OnRemoteAudioToggledHandler;
        _signalRService.OnScreenShareStarted += OnScreenShareStartedHandler;
        _signalRService.OnScreenShareStopped += OnScreenShareStoppedHandler;
        _signalRService.OnRecordingStarted += OnRecordingStartedHandler;
        _signalRService.OnRecordingStopped += OnRecordingStoppedHandler;
    }

    public async Task InitializeCallAsync(bool isCaller, int conversacionId, string participantName, int? incomingCallId = null)
    {
        ConversacionId = conversacionId;
        ParticipantName = participantName;
        IsCaller = isCaller;
        CallId = incomingCallId;

        CallerInitial = !string.IsNullOrEmpty(participantName) ? participantName[0].ToString().ToUpper() : "?";

        try
        {
            // Connect to SignalR if not connected
            if (!_signalRService.IsConnected)
            {
                await _signalRService.ConnectAsync();
            }

            if (isCaller)
            {
                // Start outgoing call
                CallStatusText = "Llamando...";
                await _signalRService.StartCallAsync(conversacionId, IsVideoEnabled, IsAudioEnabled);
            }
            else if (incomingCallId.HasValue)
            {
                // Show incoming call UI
                IsIncomingCall = true;
                CallerName = participantName;
                ShowControls = false;
            }
        }
        catch (Exception ex)
        {
            CallStatusText = "Error de conexión";
            System.Diagnostics.Debug.WriteLine($"[VideoCall] Error initializing: {ex.Message}");
        }
    }

    #region SignalR Event Handlers

    private void OnCallAcceptedHandler(CallAcceptedData data)
    {
        CallId = data.CallId;
        CallStatusText = "Conectando...";
        IsIncomingCall = false;
        ShowControls = true;

        // If we're the caller, create and send offer
        if (IsCaller)
        {
            OnExecuteJavaScript?.Invoke("window.WebRTC.createOffer()");
        }
    }

    private void OnCallRejectedHandler(CallRejectedData data)
    {
        CallStatusText = "Llamada rechazada";
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(2000);
            OnCallEnded?.Invoke();
        });
    }

    private void OnCallEndedHandler(CallEndedData data)
    {
        StopDurationTimer();
        CallStatusText = "Llamada finalizada";
        OnExecuteJavaScript?.Invoke("window.WebRTC.endCall()");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(1500);
            OnCallEnded?.Invoke();
        });
    }

    private void OnCallErrorHandler(string message)
    {
        CallStatusText = message;
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", message, "OK");
        });
    }

    private void OnOfferReceivedHandler(OfferData data)
    {
        OnExecuteJavaScript?.Invoke($"window.WebRTC.handleOffer('{EscapeJs(data.Sdp)}')");
    }

    private void OnAnswerReceivedHandler(AnswerData data)
    {
        OnExecuteJavaScript?.Invoke($"window.WebRTC.handleAnswer('{EscapeJs(data.Sdp)}')");
        StartDurationTimer();
        IsConnected = true;
        CallStatusText = "En llamada";
    }

    private void OnIceCandidateReceivedHandler(IceCandidateData data)
    {
        OnExecuteJavaScript?.Invoke($"window.WebRTC.addIceCandidate('{EscapeJs(data.Candidate)}')");
    }

    private void OnRemoteVideoToggledHandler(MediaToggleData data)
    {
        var initial = CallerInitial;
        OnExecuteJavaScript?.Invoke($"window.WebRTC.setRemoteVideoEnabled({data.Enabled.ToString().ToLower()}, '{initial}')");
    }

    private void OnRemoteAudioToggledHandler(MediaToggleData data)
    {
        // Could show a visual indicator that remote audio is muted
    }

    private void OnScreenShareStartedHandler(int callId)
    {
        // Remote user started screen sharing
    }

    private void OnScreenShareStoppedHandler(int callId)
    {
        // Remote user stopped screen sharing
    }

    private void OnRecordingStartedHandler(int callId)
    {
        // Notify that call is being recorded
    }

    private void OnRecordingStoppedHandler(int callId)
    {
        // Recording stopped
    }

    #endregion

    #region WebView Callbacks

    public async Task HandleWebRtcMessageAsync(string eventName, string dataJson)
    {
        System.Diagnostics.Debug.WriteLine($"[VideoCall] WebRTC event: {eventName}");

        switch (eventName)
        {
            case "ready":
                // WebRTC is ready, start local media
                OnExecuteJavaScript?.Invoke($"window.WebRTC.startLocalMedia({IsVideoEnabled.ToString().ToLower()}, {IsAudioEnabled.ToString().ToLower()})");
                break;

            case "localMediaStarted":
                // Local media ready, create peer connection
                OnExecuteJavaScript?.Invoke("window.WebRTC.createPeerConnection()");
                break;

            case "offerCreated":
                // Send offer through SignalR
                var offerData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
                if (offerData != null && offerData.TryGetValue("sdp", out var sdp) && CallId.HasValue)
                {
                    await _signalRService.SendOfferAsync(CallId.Value, sdp);
                }
                break;

            case "answerCreated":
                // Send answer through SignalR
                var answerData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
                if (answerData != null && answerData.TryGetValue("sdp", out var answerSdp) && CallId.HasValue)
                {
                    await _signalRService.SendAnswerAsync(CallId.Value, answerSdp);
                    StartDurationTimer();
                    IsConnected = true;
                    CallStatusText = "En llamada";
                }
                break;

            case "iceCandidate":
                // Send ICE candidate through SignalR
                var iceData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
                if (iceData != null && iceData.TryGetValue("candidate", out var candidate) && CallId.HasValue)
                {
                    await _signalRService.SendIceCandidateAsync(CallId.Value, candidate);
                }
                break;

            case "connectionStateChanged":
                var stateData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
                if (stateData != null && stateData.TryGetValue("state", out var state))
                {
                    if (state == "connected")
                    {
                        StartDurationTimer();
                        IsConnected = true;
                        CallStatusText = "En llamada";
                    }
                }
                break;

            case "recordingComplete":
                // Handle completed recording (upload to server)
                // TODO: Implement recording upload
                break;

            case "error":
                var errorData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(dataJson);
                if (errorData != null && errorData.TryGetValue("message", out var errorMessage))
                {
                    OnCallErrorHandler(errorMessage);
                }
                break;
        }
    }

    #endregion

    #region Commands

    [RelayCommand]
    private async Task AcceptCall()
    {
        if (!CallId.HasValue) return;

        IsIncomingCall = false;
        ShowControls = true;
        CallStatusText = "Conectando...";

        await _signalRService.AcceptCallAsync(CallId.Value, IsVideoEnabled, IsAudioEnabled);

        // Start local media
        OnExecuteJavaScript?.Invoke($"window.WebRTC.startLocalMedia({IsVideoEnabled.ToString().ToLower()}, {IsAudioEnabled.ToString().ToLower()})");
    }

    [RelayCommand]
    private async Task RejectCall()
    {
        if (!CallId.HasValue) return;

        await _signalRService.RejectCallAsync(CallId.Value, "Llamada rechazada");
        OnCallEnded?.Invoke();
    }

    [RelayCommand]
    private async Task EndCall()
    {
        if (CallId.HasValue)
        {
            await _signalRService.EndCallAsync(CallId.Value);
        }

        StopDurationTimer();
        OnExecuteJavaScript?.Invoke("window.WebRTC.endCall()");
        OnCallEnded?.Invoke();
    }

    [RelayCommand]
    private async Task ToggleVideo()
    {
        IsVideoEnabled = !IsVideoEnabled;
        OnPropertyChanged(nameof(VideoButtonColor));
        OnPropertyChanged(nameof(VideoButtonIcon));

        OnExecuteJavaScript?.Invoke($"window.WebRTC.toggleVideo({IsVideoEnabled.ToString().ToLower()})");

        if (CallId.HasValue)
        {
            await _signalRService.ToggleVideoAsync(CallId.Value, IsVideoEnabled);
        }
    }

    [RelayCommand]
    private async Task ToggleAudio()
    {
        IsAudioEnabled = !IsAudioEnabled;
        OnPropertyChanged(nameof(AudioButtonColor));
        OnPropertyChanged(nameof(AudioButtonIcon));

        OnExecuteJavaScript?.Invoke($"window.WebRTC.toggleAudio({IsAudioEnabled.ToString().ToLower()})");

        if (CallId.HasValue)
        {
            await _signalRService.ToggleAudioAsync(CallId.Value, IsAudioEnabled);
        }
    }

    [RelayCommand]
    private async Task ToggleScreenShare()
    {
        IsScreenSharing = !IsScreenSharing;
        OnPropertyChanged(nameof(ScreenShareButtonColor));

        if (IsScreenSharing)
        {
            OnExecuteJavaScript?.Invoke("window.WebRTC.startScreenShare()");
            if (CallId.HasValue)
            {
                await _signalRService.StartScreenShareAsync(CallId.Value);
            }
        }
        else
        {
            OnExecuteJavaScript?.Invoke("window.WebRTC.stopScreenShare()");
            if (CallId.HasValue)
            {
                await _signalRService.StopScreenShareAsync(CallId.Value);
            }
        }
    }

    [RelayCommand]
    private async Task ToggleRecording()
    {
        IsRecording = !IsRecording;
        OnPropertyChanged(nameof(RecordButtonColor));

        if (IsRecording)
        {
            OnExecuteJavaScript?.Invoke("window.WebRTC.startRecording()");
            if (CallId.HasValue)
            {
                await _signalRService.StartRecordingAsync(CallId.Value);
            }
        }
        else
        {
            OnExecuteJavaScript?.Invoke("window.WebRTC.stopRecording()");
            if (CallId.HasValue)
            {
                await _signalRService.StopRecordingAsync(CallId.Value);
            }
        }
    }

    [RelayCommand]
    private async Task Back()
    {
        if (CallId.HasValue && IsConnected)
        {
            // Confirm if user wants to end call
            var result = await Application.Current!.MainPage!.DisplayAlert(
                "Finalizar llamada",
                "¿Deseas finalizar la videollamada?",
                "Sí", "No");

            if (result)
            {
                await EndCall();
            }
        }
        else
        {
            OnCallEnded?.Invoke();
        }
    }

    #endregion

    #region Timer

    private void StartDurationTimer()
    {
        _callStartTime = DateTime.Now;
        _durationTimer = new System.Timers.Timer(1000);
        _durationTimer.Elapsed += OnDurationTimerElapsed;
        _durationTimer.Start();
    }

    private void StopDurationTimer()
    {
        _durationTimer?.Stop();
        _durationTimer?.Dispose();
        _durationTimer = null;
    }

    private void OnDurationTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_callStartTime.HasValue)
        {
            var duration = DateTime.Now - _callStartTime.Value;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CallDuration = duration.ToString(@"mm\:ss");
            });
        }
    }

    #endregion

    private static string EscapeJs(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("'", "\\'")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r");
    }

    public void Cleanup()
    {
        StopDurationTimer();

        _signalRService.OnCallAccepted -= OnCallAcceptedHandler;
        _signalRService.OnCallRejected -= OnCallRejectedHandler;
        _signalRService.OnCallEnded -= OnCallEndedHandler;
        _signalRService.OnCallError -= OnCallErrorHandler;
        _signalRService.OnOfferReceived -= OnOfferReceivedHandler;
        _signalRService.OnAnswerReceived -= OnAnswerReceivedHandler;
        _signalRService.OnIceCandidateReceived -= OnIceCandidateReceivedHandler;
        _signalRService.OnRemoteVideoToggled -= OnRemoteVideoToggledHandler;
        _signalRService.OnRemoteAudioToggled -= OnRemoteAudioToggledHandler;
        _signalRService.OnScreenShareStarted -= OnScreenShareStartedHandler;
        _signalRService.OnScreenShareStopped -= OnScreenShareStoppedHandler;
        _signalRService.OnRecordingStarted -= OnRecordingStartedHandler;
        _signalRService.OnRecordingStopped -= OnRecordingStoppedHandler;
    }
}
