$artifacts = Get-ChildItem -Path ./out_artifacts -Directory

if ($(Test-Path ./out) -eq $false) {
    mkdir out
}

Write-Host "Coping"
foreach ($artifact in $artifacts) {
    Get-ChildItem -Path ./out_artifacts/$($artifact.Name)
    Copy-Item ./out_artifacts/$($artifact.Name)/* -Destination ./out/ -Recurse -Force
}

Write-Host "Copy Result"
Get-ChildItem -Path ./out/
