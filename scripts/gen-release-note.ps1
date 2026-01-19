$md5Summary = "
> [!important]
> 下载时请注意核对文件MD5是否正确。

| 文件名 | MD5 |
| --- | --- |
"

$hashes = [ordered]@{}

$hash = Get-FileHash ./out/DutyIsland.cipx -Algorithm MD5
$hashString = $hash.Hash
$md5Summary +=  "| DutyIsland.cipx | ``${hashString}`` |`n"
$hashes.Add("DutyIsland.cipx", $hashString)

$json = ConvertTo-Json $hashes -Compress

$md5Summary +=  "`n<!-- CLASSISLAND_PKG_MD5 ${json} -->"
Write-Host "MD5 Summary:" -ForegroundColor Gray
Write-Host $md5Summary -ForegroundColor Gray
Write-Host "----------" -ForegroundColor Gray

$changelog = Get-Content "./changelog/${env:tagName}.md"
$fullContent = $changelog + $md5Summary

Write-Output $fullContent > "release-note.md"
