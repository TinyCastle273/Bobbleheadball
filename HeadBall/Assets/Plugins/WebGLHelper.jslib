mergeInto(LibraryManager.library, {

  QuitWebGL: function () {
    console.log("CloseGameBubbleHeadBall");
  },
  
  registerVisibilityChangeEvent: function () {
    document.addEventListener("visibilitychange", function () {
      SendMessage("DetectPause", "OnVisibilityChange", document.visibilityState);
    });
    if (document.visibilityState != "visible")
      SendMessage("DetectPause", "OnVisibilityChange", document.visibilityState);
  },
});