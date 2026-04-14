param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$applicationRoot = Join-Path $RepoRoot 'Application'
$docsRoot = Join-Path $RepoRoot 'docs\ai'
$catalogPath = Join-Path $docsRoot 'commands-queries.md'

function Get-GenericTypeArgument {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text,
        [Parameter(Mandatory = $true)]
        [int]$StartIndex
    )

    $depth = 1
    for ($index = $StartIndex; $index -lt $Text.Length; $index++) {
        switch ($Text[$index]) {
            '<' { $depth++ }
            '>' {
                $depth--
                if ($depth -eq 0) {
                    return $Text.Substring($StartIndex, $index - $StartIndex).Trim()
                }
            }
        }
    }

    return $null
}

function Get-RequestMetadata {
    param(
        [Parameter(Mandatory = $true)]
        [System.IO.FileInfo]$File
    )

    $content = Get-Content -LiteralPath $File.FullName -Raw

    $namespaceMatch = [regex]::Match($content, 'namespace\s+([A-Za-z0-9_.]+)')
    $nameMatch = [regex]::Match($content, 'record\s+(\w+)\s*\(')
    $interfaceMatch = [regex]::Match($content, '(ICommand|IQuery)\s*<')

    if (-not $namespaceMatch.Success -or -not $nameMatch.Success -or -not $interfaceMatch.Success) {
        return $null
    }

    $name = $nameMatch.Groups[1].Value
    $kind = if ($interfaceMatch.Groups[1].Value -eq 'ICommand') { 'Command' } else { 'Query' }
    $returnTypeStart = $interfaceMatch.Index + $interfaceMatch.Length
    $returnType = Get-GenericTypeArgument -Text $content -StartIndex $returnTypeStart
    $relativePath = $File.FullName.Substring($RepoRoot.Length + 1).Replace([System.IO.Path]::DirectorySeparatorChar, '/')
    $handlerRelativePath = (($relativePath -replace '/Commands/', '/Handlers/') -replace '/Queries/', '/Handlers/') -replace '\.cs$', 'Handler.cs'

    $handlerExists = Test-Path (Join-Path $RepoRoot ($handlerRelativePath -replace '/', [System.IO.Path]::DirectorySeparatorChar))

    [pscustomobject]@{
        Name = $name
        Kind = $kind
        Namespace = $namespaceMatch.Groups[1].Value
        ReturnType = $returnType
        File = $relativePath
        Handler = if ($handlerExists) { $handlerRelativePath } else { 'N/A' }
    }
}

$requestFiles = @()
$requestFiles += Get-ChildItem -Path $applicationRoot -Recurse -File -Filter '*Command.cs'
$requestFiles += Get-ChildItem -Path $applicationRoot -Recurse -File -Filter '*Query.cs'

$items = $requestFiles |
    ForEach-Object { Get-RequestMetadata -File $_ } |
    Where-Object { $null -ne $_ } |
    Sort-Object Namespace, Kind, Name

$sections = $items | Group-Object { $_.Namespace.Split('.')[1] }

$builder = [System.Text.StringBuilder]::new()
[void]$builder.AppendLine('# Commands & Queries Catalog')
[void]$builder.AppendLine('')
[void]$builder.AppendLine('Este archivo se genera automaticamente desde `Application` para que GitHub Copilot u otra IA pueda descubrir rapidamente los `Commands` y `Queries` disponibles.')
[void]$builder.AppendLine('')
[void]$builder.AppendLine('> Para actualizarlo: `powershell -ExecutionPolicy Bypass -File tools\\Update-CommandsQueriesCatalog.ps1`')
[void]$builder.AppendLine('')

foreach ($section in $sections | Sort-Object Name) {
    [void]$builder.AppendLine("## $($section.Name)")
    [void]$builder.AppendLine('')
    [void]$builder.AppendLine('| Tipo | Nombre | Namespace | Devuelve | Handler | Archivo |')
    [void]$builder.AppendLine('| --- | --- | --- | --- | --- | --- |')

    foreach ($item in $section.Group | Sort-Object Kind, Name) {
        $row = '| ' + $item.Kind + ' | `' + $item.Name + '` | `' + $item.Namespace + '` | `' + $item.ReturnType + '` | `' + $item.Handler + '` | `' + $item.File + '` |'
        [void]$builder.AppendLine($row)
    }

    [void]$builder.AppendLine('')
}

New-Item -ItemType Directory -Force -Path $docsRoot | Out-Null
Set-Content -LiteralPath $catalogPath -Value $builder.ToString() -Encoding utf8
Write-Host "Catalog updated: $catalogPath"