<#
Converts the Markdown articles in this folder to HTML pages that use the same header and footer as the
application (MainLayout.razor / NavMenu.razor). Output goes to ResourceInformationV2/wwwroot/documentation.
Run from any location with PowerShell 7+:  ./documentation/build-html.ps1
#>
$source = $PSScriptRoot
$output = Join-Path $source '..\ResourceInformationV2\wwwroot\documentation'
$output = [System.IO.Path]::GetFullPath($output)
New-Item -ItemType Directory -Path $output -Force | Out-Null
Get-ChildItem -Path $output -Filter *.html | Remove-Item -Force

$template = @'
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>{{TITLE}} - WIGG Resource Application Documentation</title>
	<link rel="dns-prefetch" href="//cdn.brand.illinois.edu">
	<link rel="dns-prefetch" href="//cdn.toolkit.illinois.edu">
	<link rel="dns-prefetch" href="//cdn.disability.illinois.edu">
	<link rel="shortcut icon" href="https://cdn.brand.illinois.edu/favicon.ico">
	<link rel="apple-touch-icon" href="https://cdn.brand.illinois.edu/touch-icon-40x40.png">
	<link rel="stylesheet" href="//cdn.toolkit.illinois.edu/3/toolkit.css">
	<script type="module" src="//cdn.toolkit.illinois.edu/3/toolkit.js" async></script>
	<script src="https://cdn.disability.illinois.edu/skipto.min.js"></script>
	<script>var SkipToConfig = { 'settings': { 'skipTo': { displayOption: 'static', colorTheme: 'illinois', headings: 'h1, h2, h3', } } };</script>
	<link href="/css/site.css?cache=1" rel="stylesheet" />
</head>
<body>
<ilw-page>
<ilw-header slot="header">
<a slot="primary-unit" href="https://wigg.illinois.edu/">Web Implementation Guidelines Group</a>
<a slot="site-name" href="/">WIGG Resource Application</a>
<div slot="links">
	<a href="/">Back to the Application</a>
</div>
</ilw-header>

<main id="app-main">
<ilw-content width="page">
{{CONTENT}}
</ilw-content>
<ilw-spacer></ilw-spacer>
</main>

<ilw-footer slot="footer">
<a slot="primary-unit" href="https://wigg.illinois.edu/">Web Implementation Guidelines Group</a>
<a slot="site-name" href="/">WIGG Resource Application</a>
</ilw-footer>
</ilw-page>
</body>
</html>
'@

foreach ($file in Get-ChildItem -Path $source -Filter *.md) {
	$markdown = ConvertFrom-Markdown -Path $file.FullName
	$html = $markdown.Html
	$html = [regex]::Replace($html, 'href="README\.md"', 'href="index.html"')
	$html = [regex]::Replace($html, 'href="([^"/:]+)\.md"', 'href="$1.html"')
	$title = [regex]::Match($html, '<h1[^>]*>(.*?)</h1>').Groups[1].Value
	if (-not $title) { $title = 'Documentation' }
	$page = $template.Replace('{{TITLE}}', $title).Replace('{{CONTENT}}', $html)
	$name = if ($file.BaseName -eq 'README') { 'index' } else { $file.BaseName }
	Set-Content -Path (Join-Path $output "$name.html") -Value $page -Encoding utf8
	Write-Host "Created $name.html"
}
