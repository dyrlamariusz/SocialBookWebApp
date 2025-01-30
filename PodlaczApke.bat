powershell -Command "Start-Process 'docker' -ArgumentList 'compose up' -NoNewWindow; Start-Sleep -Seconds 5; Start-Process 'dotnet' -ArgumentList 'run --project SocialBook';
