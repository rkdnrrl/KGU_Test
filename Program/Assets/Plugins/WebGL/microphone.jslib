mergeInto(LibraryManager.library, {
  JS_Microphone_Start: function(gameObjectNamePtr, onReadyMethodPtr, onErrorMethodPtr) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var onReadyMethod = UTF8ToString(onReadyMethodPtr);
    var onErrorMethod = UTF8ToString(onErrorMethodPtr);

    if (!window.__unityMicState) {
      window.__unityMicState = {};
    }

    var state = window.__unityMicState;

    var sendError = function(msg) {
      SendMessage(gameObjectName, onErrorMethod, String(msg || "Microphone start failed"));
    };

    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      sendError("getUserMedia is not supported in this browser.");
      return;
    }

    navigator.mediaDevices.getUserMedia({ audio: true }).then(function(stream) {
      state.stream = stream;
      state.chunks = [];
      state.gameObjectName = gameObjectName;
      state.onErrorMethod = onErrorMethod;

      var recorderOptions = {};
      if (typeof MediaRecorder !== "undefined" &&
          MediaRecorder.isTypeSupported &&
          MediaRecorder.isTypeSupported("audio/webm;codecs=opus")) {
        recorderOptions.mimeType = "audio/webm;codecs=opus";
      }

      try {
        state.recorder = new MediaRecorder(stream, recorderOptions);
      } catch (e) {
        sendError("Failed to create MediaRecorder: " + (e && e.message ? e.message : e));
        return;
      }

      state.recorder.ondataavailable = function(event) {
        if (event.data && event.data.size > 0) {
          state.chunks.push(event.data);
        }
      };

      state.recorder.onerror = function(event) {
        var err = event && event.error && event.error.message ? event.error.message : "MediaRecorder error";
        sendError(err);
      };

      try {
        state.recorder.start();
      } catch (e) {
        sendError("Failed to start recording: " + (e && e.message ? e.message : e));
        return;
      }

      SendMessage(gameObjectName, onReadyMethod, "ok");
    }).catch(function(err) {
      sendError(err && err.message ? err.message : "Microphone permission denied");
    });
  },

  JS_Microphone_StopAndTranscribe: function(
    gameObjectNamePtr,
    onResultMethodPtr,
    onErrorMethodPtr,
    endpointPtr,
    bearerTokenPtr,
    modelPtr,
    languagePtr
  ) {
    var gameObjectName = UTF8ToString(gameObjectNamePtr);
    var onResultMethod = UTF8ToString(onResultMethodPtr);
    var onErrorMethod = UTF8ToString(onErrorMethodPtr);
    var endpoint = UTF8ToString(endpointPtr);
    var bearerToken = UTF8ToString(bearerTokenPtr);
    var model = UTF8ToString(modelPtr);
    var language = UTF8ToString(languagePtr);

    var state = window.__unityMicState || {};
    var recorder = state.recorder;

    var sendError = function(msg) {
      SendMessage(gameObjectName, onErrorMethod, String(msg || "Microphone stop/transcribe failed"));
    };

    if (!recorder) {
      sendError("Recorder is not initialized.");
      return;
    }

    recorder.onstop = async function() {
      try {
        var chunks = state.chunks || [];
        var mimeType = recorder.mimeType || "audio/webm";
        var blob = new Blob(chunks, { type: mimeType });

        if (!blob || blob.size <= 0) {
          sendError("Recorded audio is empty.");
          return;
        }

        var form = new FormData();
        form.append("model", model || "gpt-4o-mini-transcribe");
        if (language && language.trim().length > 0) {
          form.append("language", language.trim());
        }
        form.append("file", blob, "speech.webm");

        var headers = {};
        if (bearerToken && bearerToken.trim().length > 0) {
          headers["Authorization"] = "Bearer " + bearerToken.trim();
        }

        var response = await fetch(endpoint, {
          method: "POST",
          headers: headers,
          body: form
        });

        if (!response.ok) {
          var errorText = await response.text();
          sendError("HTTP " + response.status + ": " + errorText);
          return;
        }

        var json = await response.json();
        var text = json && json.text ? json.text : "";
        SendMessage(gameObjectName, onResultMethod, text);
      } catch (err) {
        sendError(err && err.message ? err.message : err);
      } finally {
        if (state.stream && state.stream.getTracks) {
          state.stream.getTracks().forEach(function(track) { track.stop(); });
        }
        state.stream = null;
        state.recorder = null;
        state.chunks = [];
      }
    };

    try {
      if (recorder.state === "recording") {
        recorder.stop();
      } else {
        sendError("Recorder is not currently recording.");
      }
    } catch (e) {
      sendError("Failed to stop recorder: " + (e && e.message ? e.message : e));
    }
  },

  JS_Microphone_Stop: function() {
    var state = window.__unityMicState || {};
    if (state.recorder && state.recorder.state === "recording") {
      try {
        state.recorder.stop();
      } catch (e) {
      }
    }

    if (state.stream && state.stream.getTracks) {
      state.stream.getTracks().forEach(function(track) { track.stop(); });
    }

    state.stream = null;
    state.recorder = null;
    state.chunks = [];
    window.__unityMicState = state;
  }
});
