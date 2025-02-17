# 🚀 How to start the app

## 1️⃣ Open the Terminal  
Navigate to the **main project folder** (where `docker-compose.yml` is located).

## 2️⃣ Build and Start Containers  
Run the following command:  
```sh
docker-compose up --build
```

## 3️⃣ Once all containers are running, start the application by running:
```sh
dotnet run --project SocialBook
```
## 4️⃣ Alternatively, you can use the script:
Run `PodlaczApkeBuild.bat` to automate the process.

# 🛠 Troubleshooting

🔹 Port conflicts?

If certain ports are unavailable, try stopping local SQL Server in Windows Services.

🔹 Docker issues?

You may need to disable WSL2 in Docker settings.

Ensure that Docker Daemon is exposed (Expose daemon on TCP in Docker settings).

![image](https://github.com/user-attachments/assets/738f4c89-0414-4494-ac1c-89446b44d77b)

