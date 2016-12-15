# WinRealCapture
アクティブなウィンドウの画像キャプチャを取って png 形式で自動保存するもの。

## 環境前提
Windows 用です。
高解像度モニタで表示倍率を変更している場合に既存キャプチャソフトがうまく動かない（キャプチャ領域が大きくずれる）ことがあるので自作しました。

## 実行ファイルダウンロード
- [WinRealCapture_ver0.8.0.0.zip](https://github.com/kobake/WinRealCapture/raw/master/archives/WinRealCapture_ver0.8.0.0.zip)

## Screenshot
<img src="https://raw.githubusercontent.com/kobake/WinRealCapture/master/screenshots/screenshot.png" width="600" />

## Usage
アプリを起動した状態で Ctrl + F2 キーを押すと、現在アクティブなウィンドウのスクリーンショットが保存先ディレクトリ (SavingDirectory) に自動保存されます。

保存先ディレクトリは任意指定できますが、ファイル名は ```capt_YYYYMMDD_hhmmss_nn.png``` という固定の名前で保存されます。

## 細かい挙動
- リストボックス上の項目をダブルクリックするとファイルが直接開きます。
- リストボックス上の項目を選択して DELETE キーを押すとファイルが消えます。
