# clidamon

VOICEVOX API を呼び出し音声ファイル (*.wav) を作成するコマンドです。

## 例

```ps1
clidamon.exe --text "こんにちは" --speaker-id 1 --output="output.wav"
start output.wav
```

`--text` 以外は省略可能です。

speakerId は `http://127.0.0.1:50021/speakers` の情報と対応しています。

## ノベルゲームでの使用

