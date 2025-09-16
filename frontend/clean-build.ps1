# Clean Angular Build Script for Windows 10 Pro
Write-Host "🚀 Starting Angular full clean build process..."

# Step 1 - Delete node_modules if exists
if (Test-Path "node_modules") {
    Write-Host "Deleting node_modules..."
    Remove-Item -Recurse -Force "node_modules"
}

# Step 2 - Delete package-lock.json if exists
if (Test-Path "package-lock.json") {
    Write-Host "Deleting package-lock.json..."
    Remove-Item -Force "package-lock.json"
}

# Step 3 - Delete dist folder if exists
if (Test-Path "dist") {
    Write-Host "Deleting dist folder..."
    Remove-Item -Recurse -Force "dist"
}

# Step 4 - Delete Angular cache if exists
if (Test-Path ".angular\cache") {
    Write-Host "Deleting .angular\cache folder..."
    Remove-Item -Recurse -Force ".angular\cache"
}

# Step 5 - Clean Angular internal cache
Write-Host "Running Angular cache clean..."
npx ng cache clean

# Step 6 - Install dependencies
Write-Host "Installing npm packages..."
npm install

# Step 7 - Run production build with verbose output
Write-Host "Building project..."
npx ng build --configuration production --verbose

Write-Host "✅ Angular Clean Build Completed Successfully!"