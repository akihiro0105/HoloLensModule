# HoloLensModule
- Windows10 Creators Update
- Unity 5.6.1
- Visual Studio 2015
----------
## 概要
HoloLensでハンドジェスチャー及び各種機能を利用するためのツールキットです．
ハンドジェスチャー，ネットワーク，ユーティリティで必要な機能を追加していきます．

## 内容
### Input : ハンドジェスチャーに関するモジュール
- Prefabs
    + HandObject.prefab
        * InputModule.prefab内で利用する認識した手に追従するオブジェクト
    + InputModule.prefab
        * Scene内に一つは位置することで手のトラッキングとジェスチャーの認識を行う
- Scripts
    + HandGestureLog.cs
        * 認識したジェスチャーをConsoleに表示する
    + HandObjectControl.cs
        * トラッキングした手の位置とPress状態を表示
    + HandsGestureManager.cs
        * ジェスチャーの認識
    + HandsInteractionManager.cs
        * 手のトラッキング
    + ObjectMoveControl.cs
        * オブジェクトの移動(Colliderがついているオブジェクトにアタッチ)
    + ObjectRotationControl.cs
        * オブジェクトの回転(Colliderがついているオブジェクトにアタッチ)
    + ObjectScaleControl.cs
        * オブジェクトのサイズ(Colliderがついているオブジェクトにアタッチ)
    + RayCastControl.cs
        * 視線上のオブジェクトを検出する
    + StabilizationCamera.cs
        * HoloLensの表示安定化
### Network : TCP,UDP,RestAPIでの通信を行うためのモジュール
- RestAPI/Scripts
    + RestAPIJsonManager.cs
        * RestAPIのGETとPOSTを行う
- TCP/Scripts
    + TcpNetworkClientManager.cs
        * Tcpネットワークのクライアント制御
    + TcpNetworkServerManager.cs
        * Tcpネットワークのサーバー制御(HoloLens側は未実装)
- UDP/Scripts
    + UdpNetworkClientManager.cs
        * Udpネットワークの送信制御
    + UdpNetworkListenManager.cs
        * Udpネットワークの受信制御
### Secenes : ハンドジェスチャーを確認するためのサンプルScene
- sample.unity
### Utilities
- Scripts
    + BaseSceneFunction.cs
        * シーン変更時に指定のオブジェクトを削除しないようにする
    + DebugConsole.cs
        * コンソール出力を取得してUIのTextに渡す
    + DeviceActiveControl.cs
        * 特定のデバイス(HoloLens or Desktop)のみでオブジェクトを有効にさせる
    + HoloLensModuleSingleton.cs
        * シングルトン
    + WorldAnchorControl.cs
        * ワールドアンカー制御

## 今後の予定
- Unity，VisualStudioのバージョンアップに伴う改修
