mergeInto(LibraryManager.library, {
  InitSDK: function (objectName) {
    if (typeof window.CyborgImpl != 'undefined'
      && window.CyborgImpl.didSdkInit)
      return;
    if (typeof UTF8ToString !== 'undefined') {
      window.unityStringify = UTF8ToString;
    } else if (typeof Pointer_stringify !== 'undefined') {
      window.unityStringify = Pointer_stringify;
    } else {
      window.unityStringify = (str) => str;
    }
    window.CyborgImpl = {
      callbackObjectName: window.unityStringify(objectName),
      scriptElement: HTMLScriptElement = null,
      didSdkInit: false,
      isSdkCoreLoaded: false,
      callbackQueue: [],
      InitSDK() {
        // console.log(`Init------${VERSION}`);
        // load the HTML SDK v2
        this.scriptElement = document.createElement('script');
        this.scriptElement.id = "CyborgCore";
        this.scriptElement.src = 'https://sdk.cyborg.game/v0.0.27/cyborg_core.umd.release.min.js';
        document.head.appendChild(this.scriptElement);
        this.scriptElement.addEventListener('load', async function () {
          const res = await window.CyborgCore.InitSDK();
          if (res)
            window.CyborgImpl.OnSDKLoaded();
          else
            window.CyborgImpl.OnSDKInitFailed();
        });
      },
      OnSDKLoaded() {
        // console.log(`Done init------${VERSION}`);
        window.CyborgImpl.didSdkInit = true;
        this.isSdkCoreLoaded = true;
        this.callbackQueue.forEach(element => {
          element();
        });
        const userProfile = window.CyborgCore.GetUserProfile();
        window.CyborgImpl.SendMessageTo(CyborgImpl.callbackObjectName, "SDKInitCallback", JSON.stringify(userProfile));
      },
      OnSDKInitFailed() {
        document.head.removeChild(this.scriptElement);
      },
      EnsureLoaded(callback) {
        if (this.isSdkCoreLoaded) {
          callback();
        }
        else {
          this.callbackQueue.push(callback);
        }
      },
      SendMessageTo(objectName, messageName, payload) {
        if (typeof SendMessage != 'undefined')
          SendMessage(objectName, messageName, payload);
        else
          window.document.dispatchEvent(
            new CustomEvent(messageName, {
              bubbles: true,
              detail: payload
            })
          )
      }
    }
    window.CyborgImpl.InitSDK();
  },
  StartGame: function () {
    window.CyborgImpl.EnsureLoaded(() => {
      window.CyborgCore.StartGame();
    });
  },
  ShowInterstitialAd: function () {
    window.CyborgImpl.EnsureLoaded(() => {
      window.CyborgCore.ShowInterstitialAd();
    });
  },
  ShowRewardedAd: function () {
    window.CyborgImpl.EnsureLoaded(async () => {
      const result = await window.CyborgCore.ShowRewardedAd();
      window.CyborgImpl.SendMessageTo(
        CyborgImpl.callbackObjectName,
        "ShowRewardedAdCallback",
        JSON.stringify(result)
      );
    });
  },
  StopGame: function () {
    window.CyborgImpl.EnsureLoaded(() => {
      window.CyborgCore.StopGame();
    });
  },
  SendTracking: function (eventID, eventParams) {
    window.CyborgImpl.EnsureLoaded(() => {
      window.CyborgCore.SendTracking(window.unityStringify(eventID), window.unityStringify(eventParams));
    });
  },
  GetUserProfile: function () {
    window.CyborgImpl.EnsureLoaded(() => {
      const userProfile = window.CyborgCore.GetUserProfile();
      window.CyborgImpl.SendMessageTo(CyborgImpl.callbackObjectName, "GetUserProfileCallback", JSON.stringify(userProfile));
    });
  },
  VerifyJWT: function () {
    window.CyborgImpl.EnsureLoaded(async () => {
      const userProfile = await window.CyborgCore.VerifyJWT();
      window.CyborgImpl.SendMessageTo(CyborgImpl.callbackObjectName, "VerifyJWTCallback", JSON.stringify(userProfile));
    });
  },
  Test: function () {
   
  },
  GetBalance: function (tokenAddress, chainID) {
    window.CyborgImpl.EnsureLoaded(async () => {
      const balanceInfoResp = await window.CyborgCore.GetBalance(tokenAddress, chainID);
      window.CyborgImpl.SendMessageTo(CyborgImpl.callbackObjectName, "GetBalanceCallback", JSON.stringify(balanceInfoResp));
    });
  },
  SendTransaction: function (rawTransaction, toAddress) {
    window.CyborgImpl.EnsureLoaded(async () => {
      const rawTransactionEventResp = await window.CyborgCore.GetBalance(rawTransaction, toAddress);
      window.CyborgImpl.SendMessageTo(CyborgImpl.callbackObjectName, "SendTransactionCallback", JSON.stringify(rawTransactionEventResp));
    });
  },
});