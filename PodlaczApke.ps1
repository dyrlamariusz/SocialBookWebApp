Start-Process "docker" -ArgumentList "compose up --build" -NoNewWindow
Start-Sleep -Seconds 5  # Wait for Docker to start
Start-Process "dotnet" -ArgumentList "run --project SocialBook"
Start-Sleep -Seconds 2  # Wait for the frontend to start
Start-Process "http://localhost:5174/"
