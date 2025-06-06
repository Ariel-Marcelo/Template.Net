param(
    [Parameter(Mandatory=$true)]
    [string]$NewProjectName,
    
    [Parameter(Mandatory=$false)]
    [string]$DestinationPath = ".."
)

# Validate project name
if ($NewProjectName -match '[^a-zA-Z0-9]') {
    Write-Error "Project name should only contain letters and numbers"
    exit 1
}

# Validate and create destination path if it doesn't exist
if (-not [System.IO.Path]::IsPathRooted($DestinationPath)) {
    $DestinationPath = [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $DestinationPath))
}

if (-not (Test-Path -Path $DestinationPath)) {
    Write-Host "Creating destination directory: $DestinationPath"
    New-Item -ItemType Directory -Path $DestinationPath -Force
}

# Create new project directory
$newPath = Join-Path $DestinationPath $NewProjectName
Write-Host "Creating new project directory: $newPath"
New-Item -ItemType Directory -Path $newPath -Force

# Define directories and files to exclude
$excludedDirs = @(
    "bin",
    "obj",
    ".git",
    ".vs",
    "node_modules",
    ".github",
    ".vscode",
    "TestResults"
)

$excludedFiles = @(
    ".gitignore",
    ".gitattributes",
    ".env",
    "*.user",
    "*.suo",
    "*.cache",
    "*.log",
    "create-project-from-template.ps1"
)

# Copy files using robocopy with exclusions
Write-Host "Copying project files..."
$excludeDirs = $excludedDirs | ForEach-Object { "/XD `"$_`"" }
$excludeFiles = $excludedFiles | ForEach-Object { "/XF `"$_`"" }

Write-Host "Copying files (excluding unnecessary ones)..."
$robocopyArgs = @(
    ".",
    "`"$newPath`"",
    "/E",
    "/NFL",
    "/NDL",
    "/NJH",
    "/NJS"
) + $excludeDirs + $excludeFiles

& robocopy @robocopyArgs

# Check if the copy was successful
if (-not (Test-Path (Join-Path $newPath "template-net7.csproj"))) {
    Write-Error "Failed to copy project files. The source .csproj file was not found in the destination."
    exit 1
}

# Navigate to new directory
Set-Location $newPath

# Rename .csproj file
Write-Host "Renaming project file..."
Rename-Item "template-net7.csproj" "$NewProjectName.csproj"

# Update .csproj content
Write-Host "Updating project references..."
$csproj = Get-Content "$NewProjectName.csproj" -Raw
$csproj = $csproj -replace '<RootNamespace>Template</RootNamespace>', "<RootNamespace>$NewProjectName</RootNamespace>"
Set-Content "$NewProjectName.csproj" $csproj

# Create/Update solution file
Write-Host "Creating solution file..."
$guid = [guid]::NewGuid().ToString().ToUpper()
$solutionContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$NewProjectName", "$NewProjectName.csproj", "{$guid}"
EndProject
Global
    GlobalSection(SolutionConfigurationPlatforms) = preSolution
        Debug|Any CPU = Debug|Any CPU
        Release|Any CPU = Release|Any CPU
    EndGlobalSection
    GlobalSection(SolutionProperties) = preSolution
        HideSolutionNode = FALSE
    EndGlobalSection
    GlobalSection(ProjectConfigurationPlatforms) = postSolution
        {$guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        {$guid}.Debug|Any CPU.Build.0 = Debug|Any CPU
        {$guid}.Release|Any CPU.ActiveCfg = Release|Any CPU
        {$guid}.Release|Any CPU.Build.0 = Release|Any CPU
    EndGlobalSection
EndGlobal
"@
Set-Content -Path "$NewProjectName.sln" -Value $solutionContent

Write-Host "`nProject creation complete!"
Write-Host "New project created at: $newPath"
Write-Host "You can now open the solution in Visual Studio or run 'dotnet run' to test it."

Write-Host "`nRecommended Extensions for Cursor/VS Code:"
Write-Host "1. C# (ms-dotnettools.csharp) - Essential for C# development"
Write-Host "2. .NET Core Tools (formulahendry.dotnet)"
Write-Host "3. NuGet Package Manager (jmrog.vscode-nuget-package-manager)"
Write-Host "4. C# Extensions (jchannon.csharpextensions)"
Write-Host "5. .NET Core Test Explorer (formulahendry.dotnet-test-explorer)"

Write-Host "`nAfter installing extensions:"
Write-Host "1. Reload Cursor"
Write-Host "2. Wait for OmniSharp to initialize"
Write-Host "3. Use Command Palette (Ctrl+Shift+P) and run 'OmniSharp: Restart OmniSharp' if needed" 