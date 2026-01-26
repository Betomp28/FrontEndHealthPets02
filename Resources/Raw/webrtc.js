/**
 * HealthPets WebRTC Video Call Implementation
 * This script handles WebRTC peer connections, media streams, recording, and screen sharing.
 */

// Configuration
const config = {
    iceServers: [
        { urls: 'stun:stun.l.google.com:19302' },
        { urls: 'stun:stun1.l.google.com:19302' }
    ]
};

// State
let peerConnection = null;
let localStream = null;
let remoteStream = null;
let screenStream = null;
let mediaRecorder = null;
let recordedChunks = [];
let isRecording = false;
let isScreenSharing = false;

// DOM Elements
const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');
const statusOverlay = document.getElementById('statusOverlay');
const statusText = document.getElementById('statusText');
const remoteVideoOff = document.getElementById('remoteVideoOff');
const remoteInitial = document.getElementById('remoteInitial');
const recordingIndicator = document.getElementById('recordingIndicator');

/**
 * Initialize WebRTC with ICE servers configuration
 */
function initializeWebRTC(iceServersJson) {
    try {
        const iceServers = JSON.parse(iceServersJson);
        config.iceServers = iceServers;
        log('WebRTC initialized with ICE servers');
        notifyMaui('initialized', { success: true });
    } catch (error) {
        log('Error initializing WebRTC: ' + error.message);
        notifyMaui('error', { message: error.message });
    }
}

/**
 * Start local media (camera and microphone)
 */
async function startLocalMedia(withVideo, withAudio) {
    try {
        setStatus('Accediendo a cámara y micrófono...');

        const constraints = {
            video: withVideo ? {
                width: { ideal: 1280 },
                height: { ideal: 720 },
                facingMode: 'user'
            } : false,
            audio: withAudio ? {
                echoCancellation: true,
                noiseSuppression: true,
                autoGainControl: true
            } : false
        };

        localStream = await navigator.mediaDevices.getUserMedia(constraints);
        localVideo.srcObject = localStream;

        log('Local media started');
        notifyMaui('localMediaStarted', {
            hasVideo: withVideo,
            hasAudio: withAudio
        });

        return true;
    } catch (error) {
        log('Error starting local media: ' + error.message);
        notifyMaui('error', { message: 'No se pudo acceder a la cámara o micrófono: ' + error.message });
        return false;
    }
}

/**
 * Create peer connection and set up event handlers
 */
function createPeerConnection() {
    try {
        peerConnection = new RTCPeerConnection(config);

        // Add local tracks to peer connection
        if (localStream) {
            localStream.getTracks().forEach(track => {
                peerConnection.addTrack(track, localStream);
            });
        }

        // Handle incoming tracks
        peerConnection.ontrack = (event) => {
            log('Remote track received: ' + event.track.kind);
            if (!remoteStream) {
                remoteStream = new MediaStream();
                remoteVideo.srcObject = remoteStream;
            }
            remoteStream.addTrack(event.track);

            // Hide status overlay when receiving video
            if (event.track.kind === 'video') {
                hideStatus();
            }
        };

        // Handle ICE candidates
        peerConnection.onicecandidate = (event) => {
            if (event.candidate) {
                const candidateJson = JSON.stringify(event.candidate);
                notifyMaui('iceCandidate', { candidate: candidateJson });
            }
        };

        // Handle connection state changes
        peerConnection.onconnectionstatechange = () => {
            log('Connection state: ' + peerConnection.connectionState);
            notifyMaui('connectionStateChanged', { state: peerConnection.connectionState });

            switch (peerConnection.connectionState) {
                case 'connecting':
                    setStatus('Conectando...');
                    break;
                case 'connected':
                    hideStatus();
                    break;
                case 'disconnected':
                    setStatus('Conexión perdida...');
                    break;
                case 'failed':
                    setStatus('Error de conexión');
                    notifyMaui('connectionFailed', {});
                    break;
            }
        };

        // Handle ICE connection state
        peerConnection.oniceconnectionstatechange = () => {
            log('ICE connection state: ' + peerConnection.iceConnectionState);
        };

        log('Peer connection created');
        return true;
    } catch (error) {
        log('Error creating peer connection: ' + error.message);
        notifyMaui('error', { message: error.message });
        return false;
    }
}

/**
 * Create and send offer (caller side)
 */
async function createOffer() {
    try {
        setStatus('Iniciando llamada...');

        if (!peerConnection) {
            createPeerConnection();
        }

        const offer = await peerConnection.createOffer({
            offerToReceiveAudio: true,
            offerToReceiveVideo: true
        });

        await peerConnection.setLocalDescription(offer);

        const sdp = JSON.stringify(peerConnection.localDescription);
        notifyMaui('offerCreated', { sdp: sdp });

        log('Offer created');
        return sdp;
    } catch (error) {
        log('Error creating offer: ' + error.message);
        notifyMaui('error', { message: error.message });
        return null;
    }
}

/**
 * Handle received offer and create answer (callee side)
 */
async function handleOffer(sdpJson) {
    try {
        setStatus('Aceptando llamada...');

        if (!peerConnection) {
            createPeerConnection();
        }

        const offer = JSON.parse(sdpJson);
        await peerConnection.setRemoteDescription(new RTCSessionDescription(offer));

        const answer = await peerConnection.createAnswer();
        await peerConnection.setLocalDescription(answer);

        const sdp = JSON.stringify(peerConnection.localDescription);
        notifyMaui('answerCreated', { sdp: sdp });

        log('Answer created');
        return sdp;
    } catch (error) {
        log('Error handling offer: ' + error.message);
        notifyMaui('error', { message: error.message });
        return null;
    }
}

/**
 * Handle received answer (caller side)
 */
async function handleAnswer(sdpJson) {
    try {
        const answer = JSON.parse(sdpJson);
        await peerConnection.setRemoteDescription(new RTCSessionDescription(answer));
        log('Answer handled');
    } catch (error) {
        log('Error handling answer: ' + error.message);
        notifyMaui('error', { message: error.message });
    }
}

/**
 * Add ICE candidate from remote peer
 */
async function addIceCandidate(candidateJson) {
    try {
        const candidate = JSON.parse(candidateJson);
        await peerConnection.addIceCandidate(new RTCIceCandidate(candidate));
        log('ICE candidate added');
    } catch (error) {
        log('Error adding ICE candidate: ' + error.message);
    }
}

/**
 * Toggle local video
 */
function toggleVideo(enabled) {
    if (localStream) {
        localStream.getVideoTracks().forEach(track => {
            track.enabled = enabled;
        });
        localVideo.style.display = enabled ? 'block' : 'none';
        log('Local video ' + (enabled ? 'enabled' : 'disabled'));
    }
}

/**
 * Toggle local audio
 */
function toggleAudio(enabled) {
    if (localStream) {
        localStream.getAudioTracks().forEach(track => {
            track.enabled = enabled;
        });
        log('Local audio ' + (enabled ? 'enabled' : 'disabled'));
    }
}

/**
 * Handle remote video toggle
 */
function setRemoteVideoEnabled(enabled, initial) {
    if (enabled) {
        remoteVideoOff.classList.remove('visible');
    } else {
        remoteInitial.textContent = initial || '?';
        remoteVideoOff.classList.add('visible');
    }
}

/**
 * Switch camera (front/back)
 */
async function switchCamera() {
    try {
        if (!localStream) return;

        const videoTrack = localStream.getVideoTracks()[0];
        if (!videoTrack) return;

        const constraints = videoTrack.getConstraints();
        const facingMode = constraints.facingMode === 'user' ? 'environment' : 'user';

        const newStream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: facingMode },
            audio: false
        });

        const newVideoTrack = newStream.getVideoTracks()[0];

        // Replace track in peer connection
        const sender = peerConnection.getSenders().find(s => s.track?.kind === 'video');
        if (sender) {
            await sender.replaceTrack(newVideoTrack);
        }

        // Replace track in local stream
        localStream.removeTrack(videoTrack);
        localStream.addTrack(newVideoTrack);
        localVideo.srcObject = localStream;

        videoTrack.stop();

        log('Camera switched');
        notifyMaui('cameraSwitched', { facingMode: facingMode });
    } catch (error) {
        log('Error switching camera: ' + error.message);
        notifyMaui('error', { message: error.message });
    }
}

/**
 * Start screen sharing
 */
async function startScreenShare() {
    try {
        screenStream = await navigator.mediaDevices.getDisplayMedia({
            video: {
                cursor: 'always'
            },
            audio: false
        });

        const screenTrack = screenStream.getVideoTracks()[0];

        // Replace video track in peer connection
        const sender = peerConnection.getSenders().find(s => s.track?.kind === 'video');
        if (sender) {
            await sender.replaceTrack(screenTrack);
        }

        // Update local video preview
        localVideo.srcObject = screenStream;

        // Handle when user stops sharing via browser UI
        screenTrack.onended = () => {
            stopScreenShare();
        };

        isScreenSharing = true;
        log('Screen sharing started');
        notifyMaui('screenShareStarted', {});

        return true;
    } catch (error) {
        log('Error starting screen share: ' + error.message);
        notifyMaui('error', { message: error.message });
        return false;
    }
}

/**
 * Stop screen sharing and restore camera
 */
async function stopScreenShare() {
    try {
        if (!isScreenSharing) return;

        // Stop screen stream
        if (screenStream) {
            screenStream.getTracks().forEach(track => track.stop());
            screenStream = null;
        }

        // Restore camera
        if (localStream) {
            const videoTrack = localStream.getVideoTracks()[0];
            const sender = peerConnection.getSenders().find(s => s.track?.kind === 'video');
            if (sender && videoTrack) {
                await sender.replaceTrack(videoTrack);
            }
            localVideo.srcObject = localStream;
        }

        isScreenSharing = false;
        log('Screen sharing stopped');
        notifyMaui('screenShareStopped', {});
    } catch (error) {
        log('Error stopping screen share: ' + error.message);
    }
}

/**
 * Start recording the call
 */
function startRecording() {
    try {
        if (isRecording) return;

        recordedChunks = [];

        // Combine local and remote streams for recording
        const combinedStream = new MediaStream();

        if (localStream) {
            localStream.getTracks().forEach(track => combinedStream.addTrack(track.clone()));
        }
        if (remoteStream) {
            remoteStream.getTracks().forEach(track => combinedStream.addTrack(track.clone()));
        }

        const options = { mimeType: 'video/webm;codecs=vp9,opus' };
        if (!MediaRecorder.isTypeSupported(options.mimeType)) {
            options.mimeType = 'video/webm;codecs=vp8,opus';
        }
        if (!MediaRecorder.isTypeSupported(options.mimeType)) {
            options.mimeType = 'video/webm';
        }

        mediaRecorder = new MediaRecorder(combinedStream, options);

        mediaRecorder.ondataavailable = (event) => {
            if (event.data && event.data.size > 0) {
                recordedChunks.push(event.data);
            }
        };

        mediaRecorder.onstop = () => {
            const blob = new Blob(recordedChunks, { type: 'video/webm' });
            const url = URL.createObjectURL(blob);

            // Convert to base64 for MAUI
            const reader = new FileReader();
            reader.onloadend = () => {
                const base64 = reader.result.split(',')[1];
                notifyMaui('recordingComplete', {
                    base64: base64,
                    mimeType: 'video/webm',
                    size: blob.size
                });
            };
            reader.readAsDataURL(blob);
        };

        mediaRecorder.start(1000); // Collect data every second
        isRecording = true;
        recordingIndicator.classList.remove('hidden');

        log('Recording started');
        notifyMaui('recordingStarted', {});
    } catch (error) {
        log('Error starting recording: ' + error.message);
        notifyMaui('error', { message: error.message });
    }
}

/**
 * Stop recording
 */
function stopRecording() {
    try {
        if (!isRecording || !mediaRecorder) return;

        mediaRecorder.stop();
        isRecording = false;
        recordingIndicator.classList.add('hidden');

        log('Recording stopped');
    } catch (error) {
        log('Error stopping recording: ' + error.message);
    }
}

/**
 * End the call and clean up
 */
function endCall() {
    try {
        // Stop recording if active
        if (isRecording) {
            stopRecording();
        }

        // Stop screen sharing
        if (isScreenSharing) {
            stopScreenShare();
        }

        // Stop all local media tracks
        if (localStream) {
            localStream.getTracks().forEach(track => track.stop());
            localStream = null;
        }

        // Stop remote stream
        if (remoteStream) {
            remoteStream.getTracks().forEach(track => track.stop());
            remoteStream = null;
        }

        // Close peer connection
        if (peerConnection) {
            peerConnection.close();
            peerConnection = null;
        }

        // Clear video elements
        localVideo.srcObject = null;
        remoteVideo.srcObject = null;

        log('Call ended, resources cleaned up');
        notifyMaui('callCleanedUp', {});
    } catch (error) {
        log('Error ending call: ' + error.message);
    }
}

// Helper functions
function setStatus(message) {
    statusText.textContent = message;
    statusOverlay.classList.remove('hidden');
}

function hideStatus() {
    statusOverlay.classList.add('hidden');
}

function log(message) {
    console.log('[WebRTC] ' + message);
}

/**
 * Notify MAUI app of events via URL scheme
 */
function notifyMaui(event, data) {
    const payload = JSON.stringify({ event: event, data: data });
    // Use custom URL scheme for MAUI communication
    window.location.href = 'healthpets://webrtc?' + encodeURIComponent(payload);
}

// Expose functions to MAUI WebView
window.WebRTC = {
    initializeWebRTC,
    startLocalMedia,
    createPeerConnection,
    createOffer,
    handleOffer,
    handleAnswer,
    addIceCandidate,
    toggleVideo,
    toggleAudio,
    setRemoteVideoEnabled,
    switchCamera,
    startScreenShare,
    stopScreenShare,
    startRecording,
    stopRecording,
    endCall
};

// Initialize on load
document.addEventListener('DOMContentLoaded', () => {
    log('WebRTC module loaded');
    notifyMaui('ready', {});
});
