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
                string[] directories = Directory.GetDirectories(pathDirectory);

                string newNameDirectory = $"{pathDirectory}\\{o.ProjectName}";

                foreach (string directory in directories)
                {
                    if (directory.Contains("{{maestro_gen.connectorName}}"))
                    {
                        Directory.Move(directory, newNameDirectory);

                        foreach (string connectorDirectoryName in Directory.GetDirectories(newNameDirectory))
                        {
                            if (connectorDirectoryName.Contains("{{maestro_gen.connectorName}}"))
                            {
                                var splitNameConnector = connectorDirectoryName.Split("}.");
                                Directory.Move(connectorDirectoryName, $"{newNameDirectory}\\{o.ProjectName}.{splitNameConnector[splitNameConnector.Length - 1]}");
                            }
                        }
                        
                    }
                }


            });

    }
}