# Navigate into WebFrontend
Set-Location -Path ".\SalesTrack.WebFrontend"

Write-Host "`nInstalling Tailwind CSS via npm..." -ForegroundColor Cyan
npm install -D tailwindcss postcss autoprefixer

if ($LASTEXITCODE -ne 0) {
    Write-Host "`n❌ npm install failed." -ForegroundColor Red
    exit 1
}

# Skip broken npx init, write config manually
Write-Host "`nManually creating tailwind.config.js..." -ForegroundColor Cyan

@"
module.exports = {
  content: [
    "./Pages/**/*.{cshtml,html}",
    "./Views/**/*.{cshtml,html}",
    "./Shared/**/*.{cshtml,html}"
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
"@ | Set-Content -Encoding UTF8 -Path "tailwind.config.js"

if (-Not (Test-Path "tailwind.config.js")) {
    Write-Host "❌ tailwind.config.js was not created." -ForegroundColor Red
    exit 1
}

# Create input.css with Tailwind directives
if (-Not (Test-Path "wwwroot/css")) {
    New-Item -ItemType Directory -Path "wwwroot/css"
}
Set-Content -Path "wwwroot/css/input.css" -Value "@tailwind base;`n@tailwind components;`n@tailwind utilities;"

Write-Host "`n✅ Tailwind configured. Run the following to build styles:" -ForegroundColor Green
Write-Host "npx tailwindcss -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch" -ForegroundColor Yellow
