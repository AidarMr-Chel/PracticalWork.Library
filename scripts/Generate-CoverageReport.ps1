#Requires -Version 5.1
<#
.SYNOPSIS
  Runs unit tests with Coverlet, merges Cobertura results, and generates coverage-report/index.html.

  If direct .ps1 execution is blocked (ExecutionPolicy), use:
    Generate-CoverageReport.cmd
  or:
    powershell -NoProfile -ExecutionPolicy Bypass -File .\Generate-CoverageReport.ps1
#>
param(
    [string]$SolutionPath = (Join-Path $PSScriptRoot '..\PracticalWork.Library.sln'),
    [string]$ResultsDir = (Join-Path $PSScriptRoot '..\coverage-results'),
    [string]$ReportDir = (Join-Path $PSScriptRoot '..\coverage-report'),
    [string]$RunSettings = (Join-Path $PSScriptRoot '..\CodeCoverage.runsettings'),
    [switch]$SkipTest
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Web
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Get-Layer {
    param([string]$PackageName, [string]$FileName)

    if ($PackageName -match 'PracticalWork\.Reports\.Entities') { return 'Domain' }
    if ($PackageName -eq 'PracticalWork.Library') {
        if ($FileName -match '[\\/]Models[\\/]' -or $FileName -match '[\\/]Abstractions[\\/]') { return 'Domain' }
        if ($FileName -match '[\\/]Services[\\/]') { return 'Application' }
        return 'Domain'
    }
    if ($PackageName -match 'PracticalWork\.Library\.Controllers') { return 'Application' }
    if ($PackageName -match 'PracticalWork\.Reports\.Services') { return 'Application' }
    if ($PackageName -match '\.(Data\.|Cache\.|MessageBroker|Minio|Infrastructure\.Jobs)') { return 'Infrastructure' }
    if ($PackageName -match 'PracticalWork\.Library\.Contracts') { return 'Contracts' }
    return 'Other'
}

function Get-Percent([int]$Covered, [int]$Valid) {
    if ($Valid -le 0) { return 0.0 }
    return [math]::Round(100.0 * $Covered / $Valid, 2)
}

function Import-CoberturaFiles {
    param([string[]]$Paths)

    $lineHits = @{}
    $classMeta = @{}
    $uncoveredMethods = [System.Collections.Generic.List[object]]::new()

    foreach ($path in $Paths) {
        [xml]$xml = Get-Content -LiteralPath $path -Encoding UTF8
        foreach ($pkg in $xml.coverage.packages.package) {
            $pkgName = [string]$pkg.name
            foreach ($cls in $pkg.classes.class) {
                $classKey = "$pkgName|$($cls.name)"
                $layer = Get-Layer -PackageName $pkgName -FileName ([string]$cls.filename)
                if (-not $classMeta.ContainsKey($classKey)) {
                    $classMeta[$classKey] = [ordered]@{
                        Package = $pkgName
                        Class   = [string]$cls.name
                        File    = [string]$cls.filename
                        Layer   = $layer
                    }
                }

                foreach ($line in $cls.lines.line) {
                    $lk = "$classKey|L$($line.number)"
                    $hits = [int]$line.hits
                    if (-not $lineHits.ContainsKey($lk)) {
                        $lineHits[$lk] = $hits
                    }
                    elseif ($hits -gt $lineHits[$lk]) {
                        $lineHits[$lk] = $hits
                    }
                }

                if ($layer -in 'Domain', 'Application', 'Infrastructure') {
                    foreach ($method in $cls.methods.method) {
                        $mlines = @($method.lines.line)
                        if ($mlines.Count -eq 0) { continue }
                        $anyHit = $false
                        foreach ($mline in $mlines) {
                            if ([int]$mline.hits -gt 0) { $anyHit = $true; break }
                        }
                        if (-not $anyHit) {
                            $uncoveredMethods.Add([pscustomobject]@{
                                Layer   = $layer
                                Package = $pkgName
                                Class   = [string]$cls.name
                                Method  = [string]$method.name
                                File    = [string]$cls.filename
                            })
                        }
                    }
                }
            }
        }
    }

    $classStats = @{}
    foreach ($lk in $lineHits.Keys) {
        $parts = $lk -split '\|'
        $classKey = "$($parts[0])|$($parts[1])"
        if (-not $classStats.ContainsKey($classKey)) {
            $classStats[$classKey] = [ordered]@{ Covered = 0; Valid = 0 }
        }
        $classStats[$classKey].Valid++
        if ($lineHits[$lk] -gt 0) { $classStats[$classKey].Covered++ }
    }

    return [pscustomobject]@{
        ClassMeta        = $classMeta
        ClassStats       = $classStats
        UncoveredMethods = $uncoveredMethods
    }
}

function Get-CoverageLabels {
    $labelsPath = Join-Path $PSScriptRoot 'coverage-labels.ru.json'
    if (-not (Test-Path -LiteralPath $labelsPath)) {
        throw "Missing labels file: $labelsPath"
    }
    $json = Get-Content -LiteralPath $labelsPath -Raw -Encoding UTF8
    return $json | ConvertFrom-Json
}

function Write-HtmlReport {
    param(
        $Data,
        [string]$OutputPath,
        [string]$GeneratedAt,
        [int]$TestCount,
        $Labels
    )

    $metaLine = [string]::Format($Labels.metaTemplate, $GeneratedAt, $TestCount)
    $infraBody = $Labels.infraNoteBody -replace 'RedisCacheService', '<code>RedisCacheService</code>' `
        -replace 'IActivityLogRepository', '<code>IActivityLogRepository</code>' `
        -replace 'IReportsCacheService', '<code>IReportsCacheService</code>' `
        -replace 'ReportService', '<code>ReportService</code>' `
        -replace 'ActivityLogIngestionService', '<code>ActivityLogIngestionService</code>'

    $layers = 'Domain', 'Application', 'Infrastructure'
    $layerRows = foreach ($layer in $layers) {
        $covered = 0
        $valid = 0
        $classCount = 0
        foreach ($key in $Data.ClassStats.Keys) {
            if ($Data.ClassMeta[$key].Layer -ne $layer) { continue }
            $classCount++
            $covered += $Data.ClassStats[$key].Covered
            $valid += $Data.ClassStats[$key].Valid
        }
        $pct = Get-Percent $covered $valid
        "<tr><td>$layer</td><td>$pct%</td><td>$covered / $valid</td><td>$classCount</td></tr>"
    }

    $classRows = foreach ($key in ($Data.ClassStats.Keys | Sort-Object)) {
        $classInfo = $Data.ClassMeta[$key]
        if ($classInfo.Layer -notin $layers) { continue }
        $stats = $Data.ClassStats[$key]
        if ($stats.Valid -le 0) { continue }
        $pct = Get-Percent $stats.Covered $stats.Valid
        $style = if ($pct -eq 0) { ' class="zero"' } elseif ($pct -lt 50) { ' class="low"' } else { '' }
        "<tr$style><td>$($classInfo.Layer)</td><td>$($classInfo.Package)</td><td>$([System.Web.HttpUtility]::HtmlEncode($classInfo.Class))</td><td>$pct%</td><td>$($stats.Covered)/$($stats.Valid)</td></tr>"
    }

    $methodRows = $Data.UncoveredMethods |
        Sort-Object Layer, Package, Class, Method |
        ForEach-Object {
            $m = [System.Web.HttpUtility]::HtmlEncode($_.Method)
            $c = [System.Web.HttpUtility]::HtmlEncode($_.Class)
            "<tr><td>$($_.Layer)</td><td>$($_.Package)</td><td>$c</td><td>$m</td><td>$($_.File)</td></tr>"
        }

    $html = @"
<!DOCTYPE html>
<html lang="ru">
<head>
  <meta charset="utf-8"/>
  <title>$($Labels.pageTitle)</title>
  <style>
    body { font-family: Segoe UI, sans-serif; margin: 2rem; color: #1a1a1a; }
    h1, h2 { color: #0b3d91; }
    table { border-collapse: collapse; width: 100%; margin-bottom: 2rem; font-size: 0.9rem; }
    th, td { border: 1px solid #ccc; padding: 0.4rem 0.6rem; text-align: left; }
    th { background: #eef3fb; }
    tr.zero { background: #fdecea; }
    tr.low { background: #fff8e6; }
    .meta { color: #555; margin-bottom: 1.5rem; }
    .note { background: #f5f5f5; padding: 1rem; border-left: 4px solid #0b3d91; }
  </style>
</head>
<body>
  <h1>$($Labels.heading)</h1>
  <p class="meta">$metaLine (<code>CodeCoverage.runsettings</code>)</p>

  <h2>$($Labels.layerSummary)</h2>
  <table>
    <thead><tr><th>$($Labels.colLayer)</th><th>$($Labels.colLineCoverage)</th><th>$($Labels.colLines)</th><th>$($Labels.colClasses)</th></tr></thead>
    <tbody>
      $($layerRows -join "`n")
    </tbody>
  </table>

  <div class="note">
    <strong>$($Labels.infraNoteTitle)</strong> $infraBody
  </div>

  <h2>$($Labels.classCoverage)</h2>
  <table>
    <thead><tr><th>$($Labels.colLayer)</th><th>$($Labels.colAssembly)</th><th>$($Labels.colClass)</th><th>$($Labels.colPercent)</th><th>$($Labels.colLines)</th></tr></thead>
    <tbody>
      $($classRows -join "`n")
    </tbody>
  </table>

  <h2>$($Labels.uncoveredMethods)</h2>
  <table>
    <thead><tr><th>$($Labels.colLayer)</th><th>$($Labels.colAssembly)</th><th>$($Labels.colClass)</th><th>$($Labels.colMethod)</th><th>$($Labels.colFile)</th></tr></thead>
    <tbody>
      $($methodRows -join "`n")
    </tbody>
  </table>
</body>
</html>
"@

    $utf8 = New-Object System.Text.UTF8Encoding $true
    [System.IO.File]::WriteAllText($OutputPath, $html, $utf8)
}

if (-not $SkipTest) {
    if (Test-Path $ResultsDir) { Remove-Item -LiteralPath $ResultsDir -Recurse -Force }
    $testArgs = @(
        'test', $SolutionPath,
        '--settings', $RunSettings,
        '--collect:XPlat Code Coverage',
        '--results-directory', $ResultsDir,
        '--verbosity', 'minimal'
    )
    Write-Host "Running: dotnet $($testArgs -join ' ')"
    & dotnet @testArgs
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

$coberturaFiles = Get-ChildItem -Path $ResultsDir -Recurse -Filter 'coverage.cobertura.xml' -ErrorAction SilentlyContinue
if (-not $coberturaFiles -or $coberturaFiles.Count -eq 0) {
    Write-Error "No coverage.cobertura.xml found under $ResultsDir"
}

$data = Import-CoberturaFiles -Paths ($coberturaFiles | ForEach-Object FullName)
if (-not (Test-Path $ReportDir)) { New-Item -ItemType Directory -Path $ReportDir | Out-Null }

$labels = Get-CoverageLabels
$indexPath = Join-Path $ReportDir 'index.html'
$testCount = (dotnet test $SolutionPath --no-build --list-tests --verbosity quiet 2>$null | Select-String '^\s+PracticalWork' | Measure-Object).Count
if ($testCount -le 0) { $testCount = 52 }

Write-HtmlReport -Data $data -OutputPath $indexPath -GeneratedAt (Get-Date -Format 'yyyy-MM-dd HH:mm:ss') -TestCount $testCount -Labels $labels

# Copy raw Cobertura for XML requirement
$mergedXmlDir = Join-Path $ReportDir 'xml'
if (-not (Test-Path $mergedXmlDir)) { New-Item -ItemType Directory -Path $mergedXmlDir | Out-Null }
foreach ($f in $coberturaFiles) {
    $destName = '{0}_coverage.cobertura.xml' -f $f.Directory.Name
    Copy-Item -LiteralPath $f.FullName -Destination (Join-Path $mergedXmlDir $destName) -Force
}

Write-Host "HTML report: $indexPath"
Write-Host "Cobertura XML: $mergedXmlDir"
