# Define directories and files to exclude

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
if ($DestinationPath -eq $NewProjectName -or (Split-Path $DestinationPath -Leaf) -eq $NewProjectName) {
    $newPath = $DestinationPath
} else {
    $newPath = Join-Path $DestinationPath $NewProjectName
    if (-not (Test-Path -Path $newPath)) {
        Write-Host "Creating new project directory: $newPath"
        New-Item -ItemType Directory -Path $newPath -Force
    }
}
Write-Host "Using project directory: $newPath"

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

# Copy Template.Net project
Write-Host "Copying Template.Net project files..."
$robocopyArgsNet = @(
    "Template.Net",            # Source directory
    (Join-Path $newPath "Template.Net"), # Destination directory
    "/E", "/NFL", "/NDL", "/NJH", "/NJS"
)
foreach ($dir in $excludedDirs) { $robocopyArgsNet += "/XD"; $robocopyArgsNet += $dir }
foreach ($file in $excludedFiles) { $robocopyArgsNet += "/XF"; $robocopyArgsNet += $file }
& robocopy @robocopyArgsNet

# Copy Template.Net.Tests project
Write-Host "Copying Template.Net.Tests project files..."
$robocopyArgsTests = @(
    "Template.Net.Tests",            # Source directory
    (Join-Path $newPath "Template.Net.Tests"), # Destination directory
    "/E", "/NFL", "/NDL", "/NJH", "/NJS"
)
foreach ($dir in $excludedDirs) { $robocopyArgsTests += "/XD"; $robocopyArgsTests += $dir }
foreach ($file in $excludedFiles) { $robocopyArgsTests += "/XF"; $robocopyArgsTests += $file }
& robocopy @robocopyArgsTests

# Check if the copy was successful
if (-not (Test-Path (Join-Path $newPath "Template.Net/template-net7.csproj"))) {
    Write-Error "Failed to copy Template.Net project files. The source .csproj file was not found in the destination."
    exit 1
}
if (-not (Test-Path (Join-Path $newPath "Template.Net.Tests/Template.Net.Tests.csproj"))) {
    Write-Error "Failed to copy Template.Net.Tests project files. The source .csproj file was not found in the destination."
    exit 1
}

# Rename .csproj files
Write-Host "Renaming Template.Net project file..."
Rename-Item (Join-Path $newPath "Template.Net/template-net7.csproj") (Join-Path $newPath "Template.Net/$NewProjectName.csproj")

Write-Host "Renaming Template.Net.Tests project file..."
Rename-Item (Join-Path $newPath "Template.Net.Tests/Template.Net.Tests.csproj") (Join-Path $newPath "Template.Net.Tests/${NewProjectName}.Tests.csproj")

# Rename Template.Net and Template.Net.Tests folders to match new project name
$mainProjectOldPath = Join-Path $newPath "Template.Net"
$mainProjectNewPath = Join-Path $newPath $NewProjectName
if (Test-Path $mainProjectOldPath) {
    Write-Host "Renaming Template.Net folder to $NewProjectName..."
    Rename-Item $mainProjectOldPath $mainProjectNewPath
}
$testProjectOldPath = Join-Path $newPath "Template.Net.Tests"
$testProjectNewPath = Join-Path $newPath "$NewProjectName.Tests"
if (Test-Path $testProjectOldPath) {
    Write-Host "Renaming Template.Net.Tests folder to $NewProjectName.Tests..."
    Rename-Item $testProjectOldPath $testProjectNewPath
}

# Update .csproj content for Template.Net
Write-Host "Updating Template.Net project references..."
$csproj = Get-Content (Join-Path $mainProjectNewPath "$NewProjectName.csproj") -Raw
$csproj = $csproj -replace '<RootNamespace>Template</RootNamespace>', "<RootNamespace>$NewProjectName</RootNamespace>"
Set-Content (Join-Path $mainProjectNewPath "$NewProjectName.csproj") $csproj

# Update .csproj content for Test Project
Write-Host "Updating $NewProjectName.Tests project references..."
$csprojTestPath = Join-Path $testProjectNewPath "$NewProjectName.Tests.csproj"
$csprojTest = Get-Content $csprojTestPath -Raw
$csprojTest = $csprojTest -replace '<RootNamespace>Template.Net.Tests</RootNamespace>', "<RootNamespace>${NewProjectName}.Tests</RootNamespace>"
$csprojTest = $csprojTest -replace '\.\.\\Template.Net\\template-net7.csproj', "..\\$NewProjectName\\$NewProjectName.csproj"
Set-Content $csprojTestPath $csprojTest

# Create/Update solution file
Write-Host "Creating solution file..."
Set-Location $newPath

dotnet new sln -n $NewProjectName

dotnet sln add (Join-Path $mainProjectNewPath "$NewProjectName.csproj")
dotnet sln add (Join-Path $testProjectNewPath "$NewProjectName.Tests.csproj")