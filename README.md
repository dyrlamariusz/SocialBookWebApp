1. Open terminal in the main folder (with docker-compose.yml)
2. Use 'docker-compose up --build'
3. Once the containers are up and running 'dotnet run --project SocialBook'

or

PodlaczApkeBuild.bat

------- Troubleshooting

If certain ports are not available, try to stop local SQL Server in Services
Also you might need to switch off the WSL2 and expose daemon in Docker

![image](https://github.com/user-attachments/assets/738f4c89-0414-4494-ac1c-89446b44d77b)

