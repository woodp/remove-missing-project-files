using System;
using System.IO;
using System.Collections.Generic;

namespace CleanProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var newFile = new List<string>();
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify the project full path as an argument");
                return;
            }

            var projFile = args[0];
            if (!File.Exists(projFile))
            {
                Console.WriteLine("The specified project file does not exist: {0}", projFile);
                return;
            }

            if (!projFile.ToLowerInvariant().EndsWith(".csproj"))
            {
                Console.WriteLine("The specified does not seem to be a project file: {0}", projFile);
                return;
            }

            Console.WriteLine("Started removing missing files from project:", projFile);

            var newProjFile = Path.Combine(Path.GetDirectoryName(projFile), Path.GetFileNameWithoutExtension(projFile) + ".Clean.csproj");
            var lines = File.ReadAllLines(projFile);
            var projectPath = Path.GetDirectoryName(projFile);
            for(var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (!line.Contains("<Content Include=\"") && !line.Contains("<None Include=\""))
                {
                    newFile.Add(line);
                }
                else
                {
                    var start = line.IndexOf("Include=\"") + "Include=\"".Length;
                    var end = line.LastIndexOf("\"");
                    var path = line.Substring(start, end - start);
                    if (File.Exists(Path.Combine(projectPath, path)))
                    {
                        newFile.Add(line);
                    }
                    else
                    {
                        if (!line.EndsWith("/>")) // I'm assuming it's only one line inside the tag
                            i += 2;
                    }
                }
            }
            File.WriteAllLines(newProjFile, newFile);

            Console.WriteLine("Finished removing missing files from project.");
            Console.WriteLine("Cleaned project file: {0}", newProjFile);
        }
    }
}
