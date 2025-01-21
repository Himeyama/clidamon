using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string text = "", string output = @".\output.wav", int speakerId = 1)
    {
        if(text == string.Empty){
            Console.Error.WriteLine("No text has been entered");
        }
        
        if(!await IsVersionMatchAsync()) {
            Console.Error.WriteLine("VOICEVOX is not running");
            VOICEVOXLaunchErrorMsgbox();
            return;
        }

        string? query = await GetAudioQueryResponseAsync(text);
        if(query == null){
            Console.Error.WriteLine("Failed to retrieve audio query");
            return;
        }

        bool success = await GenerateSpeechAsync(query, output, speakerId);
        if(!success){
            Console.Error.WriteLine("Failed to create audio.");
        }
    }

    static void VOICEVOXLaunchErrorMsgbox(){
        string script = "MsgBox \"VOICEVOX が起動していません\", vbCritical, \"エラー\"";
        // 環境変数から temp ディレクトリのパスを取得
        string tempPath = Path.GetTempPath();
        string filePath = Path.Combine(tempPath, "msgbox.vbs"); // temp\msgbox.vbs のパスを作成

        // ファイルに書き込みます
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using StreamWriter writer = new(filePath, false, Encoding.GetEncoding("shift_jis"));
            writer.WriteLine(script);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error: " + ex.Message);
            return;
        }

        // VBS スクリプトを実行します
        ProcessStartInfo startInfo = new()
        {
            FileName = "wscript.exe", // VBS を実行するための Windows Script Host
            Arguments = filePath, // 実行するスクリプトのパス
            UseShellExecute = true // シェルを使ってプロセスを起動
        };

        Process? process = Process.Start(startInfo);
        if(process == null){
            Console.Error.WriteLine("The process cannot be executed");
            return;
        }
        process.WaitForExit(); // プロセスの終了を待つ
    }

    static async Task<bool> IsVersionMatchAsync()
    {
        using HttpClient httpClient = new(new HttpClientHandler() { UseProxy = false });
        // タイムアウトを1秒に設定
        httpClient.Timeout = TimeSpan.FromSeconds(1);
        try
        {
            // GETリクエストを送信
            string response = await httpClient.GetStringAsync("http://127.0.0.1:50021/version");

            // レスポンスがバージョン番号にマッチするかを判断
            return Regex.IsMatch(response, @"\d+\.\d+\.\d+");
        }
        catch (HttpRequestException e)
        {
            Console.Error.WriteLine($"HTTP request error: {e.Message}");
        }
        catch (TaskCanceledException)
        {
            // タイムアウト
            Console.Error.WriteLine("The request timed out.");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Unexpected error: {e.Message}");
        }

        return false;
    }

    public static async Task<string?> GetAudioQueryResponseAsync(string text)
    {
        // プロキシを無効にした HttpClientHandler を作成
        HttpClient client = new(new HttpClientHandler(){ UseProxy = false });

        // URI を設定
        string url = $"http://127.0.0.1:50021/audio_query?speaker=1&text={Uri.EscapeDataString(text)}";
        
        try
        {
            // POST リクエストを送信
            HttpResponseMessage response = await client.PostAsync(url, null);

            // レスポンスが成功の場合
            if (response.IsSuccessStatusCode)
            {
                // レスポンスの内容を文字列として取得
                return await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
        }

        // レスポンスがない場合は null を返す
        return null;
    }

    // WAVファイルを受信して保存するための関数
    public static async Task<bool> GenerateSpeechAsync(string query, string outputFilePath, int speakerId = 1)
    {
        try
        {
            // HttpClientのインスタンスを作成
            using HttpClient client = new(new HttpClientHandler() { UseProxy = false });
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            StringContent content = new(query, Encoding.UTF8, "application/json");

            // POSTリクエストを送信
            HttpResponseMessage response = await client.PostAsync($"http://127.0.0.1:50021/synthesis?speaker={speakerId}", content);
            
            // レスポンスが成功したか確認
            response.EnsureSuccessStatusCode();

            // WAVファイルを取得し、指定したパスに保存
            using Stream responseStream = await response.Content.ReadAsStreamAsync();
            using FileStream fileStream = new(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await responseStream.CopyToAsync(fileStream);

            // 成功の場合はtrueを返す
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Error: " + ex.Message);
            // 失敗した場合はfalseを返す
            return false;
        }
    }
}