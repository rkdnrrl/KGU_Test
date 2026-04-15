mergeInto(LibraryManager.library, {
  readTextAloud: function(textPtr, rate, pitch, langPtr, objPtr, methodPtr) {
    var text = UTF8ToString(textPtr);
    var lang = UTF8ToString(langPtr);
    var objName = UTF8ToString(objPtr);
    var methodName = UTF8ToString(methodPtr);

    var utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = lang;
    utterance.pitch = pitch;
    utterance.rate = rate;

    utterance.onend = function() {
      SendMessage(objName, methodName, "done");
    };

    utterance.onerror = function() {
      SendMessage(objName, methodName, "error");
    };

    window.speechSynthesis.speak(utterance);
  },

  GetVoices: function() {
    var voices = window.speechSynthesis.getVoices();
    var voicesList = voices.map(function(voice) {
      return voice.name + "," + voice.lang;
    }).join(";");

    var bufferSize = lengthBytesUTF8(voicesList) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(voicesList, buffer, bufferSize);
    return buffer;
  },

  stopTextAloud: function() {
    window.speechSynthesis.cancel();
  }
});