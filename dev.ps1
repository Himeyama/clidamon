$csproj = ".\clidamon.csproj"
$publishDir = ".\publish"
$appName = "clidamon"

$firstArgument = $args[0]
$version = (Get-Date).ToString("yy.M.d")
$date = (Get-Date).ToString("yyyyMMdd")
$size = [Math]::Round((Get-ChildItem $publishDir -Force -Recurse -ErrorAction SilentlyContinue | Measure-Object Length -Sum).Sum / 1KB, 0, [MidpointRounding]::AwayFromZero)

function Publish() {
    dotnet publish $csproj -o $publishDir -c Release -p:Version=$version
}

function Pack() {
    Publish
    .'C:\Program Files (x86)\NSIS\makensis.exe' /DVERSION="$version" /DDATE="$date" /DSIZE="$size" /DPUBLISH_DIR="$publishDir" /DPRODUCT_NAME="$appName" /DEXEC_FILE="$execFile" /DPUBLISHER="$publisher" install.nsh
}

if ($firstArgument -eq 'run') {
    dotnet run $csproj
} elseif ($firstArgument -eq 'publish') {
    Publish
} elseif ($firstArgument -eq 'pack') {
    Pack
}

