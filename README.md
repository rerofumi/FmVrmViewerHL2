# FmVrmViewerHL2
VRM viewer for Hololens2

# 開発環境

- Unity
    - Unity 2019.4.2f1
- Visual Studio
    - Visual Studio 2019 community

# 前準備

## 利用アセットの準備

以下の Asset の unitypackage を用意して、取り込みます。

- UniVRM
    - UniVRM-0.56.3_3b68.unitypackage
- MRTK
    - Microsoft.MixedReality.Toolkit.Unity.Foundation.2.4.0.unitypackage

TextMeshPro Essential のリソースを取り込みます。

- Unity のメニューからリソース取り込みを実行
    - Window > TextMeshPro > Import TMP Essential Resources

## VRM アバターモデルの準備

HoloLens2 のユーザースペシャルディレクトリの `3D オブジェクト` の下に `vrm` というフォルダを作成して、その下に VRM model files を置きます。

PC から取り込むか、OneDrive に置いてそこからダウンロードするかして下さい。

# ビルド方法

HoloLens2 開発チュートリアルを参照。
https://docs.microsoft.com/ja-jp/windows/mixed-reality/mr-learning-base-02


