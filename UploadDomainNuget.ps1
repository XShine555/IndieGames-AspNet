Get-Content .env | ForEach-Object {
    if ($_ -match "^\s*([^#][^=]*)=(.*)$") {
        [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
    }
}

$projectPath = ".\Domain\Domain.csproj"
$outputPath = ".\Domain\bin\Release"
$source = "github"
$apiKey = $env:GITHUB_TOKEN

if (-not $apiKey) {
    Write-Error "GITHUB_TOKEN no está definido en el .env"
    exit 1
}

dotnet pack $projectPath -c Release

$package = Get-ChildItem $outputPath -Filter *.nupkg |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if (-not $package) {
    Write-Error "No se encontró ningún paquete .nupkg"
    exit 1
}

Write-Host "Subiendo paquete: $($package.FullName)"

dotnet nuget push $package.FullName `
    --source $source `
    --api-key $apiKey `
    --skip-duplicate