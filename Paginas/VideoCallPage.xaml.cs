using FrontEndHealthPets.ViewModels;
using System.Web;

namespace FrontEndHealthPets.Paginas;

public partial class VideoCallPage : ContentPage
{
    private readonly VideoCallViewModel _viewModel;

    public VideoCallPage(VideoCallViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Subscribe to ViewModel events
        _viewModel.OnExecuteJavaScript += ExecuteJavaScript;
        _viewModel.OnCallEnded += OnCallEnded;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnExecuteJavaScript -= ExecuteJavaScript;
        _viewModel.OnCallEnded -= OnCallEnded;
        _viewModel.Cleanup();
    }

    /// <summary>
    /// Initialize the call when the page appears
    /// </summary>
    public async Task InitializeCallAsync(bool isCaller, int conversacionId, string participantName, int? incomingCallId = null)
    {
        await _viewModel.InitializeCallAsync(isCaller, conversacionId, participantName, incomingCallId);
    }

    /// <summary>
    /// Execute JavaScript in the WebView
    /// </summary>
    private async void ExecuteJavaScript(string script)
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await WebRtcView.EvaluateJavaScriptAsync(script);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[VideoCall] JS execution error: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle call ended - navigate back
    /// </summary>
    private async void OnCallEnded()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Navigation.PopModalAsync();
        });
    }

    /// <summary>
    /// Handle WebView navigation (intercept WebRTC callbacks)
    /// </summary>
    private async void WebRtcView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        // Intercept custom URL scheme for WebRTC callbacks
        if (e.Url.StartsWith("healthpets://webrtc?"))
        {
            e.Cancel = true;

            try
            {
                var encodedPayload = e.Url.Replace("healthpets://webrtc?", "");
                var payload = HttpUtility.UrlDecode(encodedPayload);
                var message = System.Text.Json.JsonSerializer.Deserialize<WebRtcMessage>(payload);

                if (message != null)
                {
                    var dataJson = message.Data != null
                        ? System.Text.Json.JsonSerializer.Serialize(message.Data)
                        : "{}";

                    await _viewModel.HandleWebRtcMessageAsync(message.Event, dataJson);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VideoCall] Error parsing WebRTC message: {ex.Message}");
            }
        }
    }

    private class WebRtcMessage
    {
        public string Event { get; set; } = "";
        public object? Data { get; set; }
    }
}
