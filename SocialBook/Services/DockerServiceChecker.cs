using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DockerServiceChecker
{
    private readonly DockerClient _dockerClient;

    public DockerServiceChecker()
    {
        var dockerApiUri = new Uri("http://localhost:2375");

        _dockerClient = new DockerClientConfiguration(dockerApiUri)
            .CreateClient();
    }

    public async Task<bool> IsContainerRunning(string containerName)
    {
        try
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = false
            });

            return containers.Any(c => c.Names.Any(n => n.Contains(containerName)));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Docker API Error: {ex.Message}");
            return false;
        }
    }
}
