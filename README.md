# clidamon

VOICEVOX API を呼び出し音声ファイル (*.wav) を作成するコマンドです。

## 例

```ps1
."$env:localappdata\clidamon\clidamon.exe" --text "こんにちは" --speaker-id 1 --output="output.wav"
start output.wav
```

`--text` 以外は省略可能です。

speakerId は `http://127.0.0.1:50021/speakers` の情報と対応しています。

## ノベルゲームでの使用
ノベルゲームのテキスト読み上げ機能設定で使用可能です。

(最近は標準機能で VOICEVOX を呼び出せたりするので、不要かも)

アプリは、`%localappdata%\clidamon\clidamon.exe` にあります。

<img src="https://github.com/user-attachments/assets/67fc5baf-8a7a-4112-93a7-bf932eccdeda" width="600" />

パラメーターの設定は以下を参考にしてください。

<img src="https://github.com/user-attachments/assets/4e96b63c-ace4-44cb-bc84-94a92b417fcd" width="600" />
