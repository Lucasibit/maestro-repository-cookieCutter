using CommandLine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Options
{
    [Option('n', "new", Required = true, HelpText = "Create new project or solution.")]
    public string CreateNew { get; set; }

    [Option('t', "type", Required = true, HelpText = "Type of project or solution to create (project or solution).")]
    public string Type { get; set; }

    [Option('p', "projectname", Required = true, HelpText = "Name of the project or solution to create.")]
    public string ProjectName { get; set; }
}
partial class Program
{
    static void Main(string[] args)
    {
        string urlRepository = "https://github.com/Lucasibit/maestro-repository-cookieCutter.git";
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(o =>
            {
                if (o.CreateNew != "new")
                {
                    Console.WriteLine("Error: Invalid option for create new (use 'new').");
                    return;
                }
                if (o.Type != "connector" && o.Type != "command" && o.Type != "repository")
                {
                    Console.WriteLine("Error: Invalid type (use 'project' or 'solution').");
                    return;
                }

                // Do something with the input and output file paths
                Console.WriteLine("Creating new " + o.Type + ": " + o.ProjectName);
                // Clonar repositorio Git. Processar todos os arquivos e diretorios fazendo replace de {{nome de parametro}} pelos valores finais.

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "git",
                        Arguments = $"clone {urlRepository}",
                    }
                };
                process.Start();
                process.WaitForExit();

                string pathDirectory = "maestro-repository-cookieCutter";
                string newPathDirectory = $"{pathDirectory}\\{o.ProjectName}";
                string[] directories = Directory.GetDirectories(pathDirectory);

                foreach (string directory in directories)
                {
                    if (directory.Contains("{{maestro_gen.connectorName}}"))
                    {
                        Directory.Move(directory, newPathDirectory);

                        foreach (string connectorDirectoryName in Directory.GetDirectories(newPathDirectory))
                        {
                            var splitNameConnector = connectorDirectoryName.Split("}.");

                            if (connectorDirectoryName.Contains("{{maestro_gen.connectorName}}"))
                            {
                                var lastSplitNameConnector = splitNameConnector[splitNameConnector.Length - 1];

                                Directory.Move(connectorDirectoryName, $"{newPathDirectory}\\{o.ProjectName}.{lastSplitNameConnector}");
                                
                            }
                        }
                        
                    }
                }

                string[] files = Directory.GetFiles(newPathDirectory);
                foreach (string file in files)
                {
                    if (file.Contains("{{maestro_gen.connectorName}}"))
                    {
                        File.Move(file, $"{newPathDirectory}\\{o.ProjectName}.sln");
                    }
                }
            });
    }
}