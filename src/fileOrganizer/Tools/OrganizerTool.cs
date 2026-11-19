using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using fileOrganizer.Models;
using Microsoft.Extensions.AI;

namespace fileOrganizer.Tools
{
    public class OrganizerTool
    {
        private readonly string _sandBoxFolder;

        public OrganizerTool(string sandBoxFolder)
        {
            _sandBoxFolder = Path.GetFullPath(sandBoxFolder);
            Directory.CreateDirectory(_sandBoxFolder);
        }

        public string sandBoxPath => _sandBoxFolder;

        [Description("Lists all the files inside a test folder")]
        public string ListFiles()
        {
            Console.WriteLine("[Tool] ListFiles()");
            var files = Directory.GetFiles(sandBoxPath)
                        .Select(Path.GetFileName)
                        .Where(name => !name.StartsWith('.') && !string.IsNullOrEmpty(name))
                        .ToArray();
            return files.Length == 0 ? "No files found in the directory" : string.Join("\n", files);
        }

        [Description("Move selected files into target folder after user approval")]
        public string OrganizeFiles([Description("List of all file moves mapping FileName to TargetFolder")] List<MovePlan> moves)
        {
            Console.WriteLine("[Tool] OrganizeFiles()");
            if(moves == null || moves.Count == 0)
            {
                return "No files matching your grouping criteria";
            }

            foreach(var group in moves.GroupBy(x => x.TargetSubfolder))
            {
                Console.WriteLine($"Target SubFolder: ${group.Key}");
                foreach(var item in group)
                {
                    Console.WriteLine($"  - {item.FileName}");
                }
            }
                Console.WriteLine("Do you want to proceed with the plan? Y/N");
                var userInput = Console.ReadLine();
                if(!string.Equals(userInput, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting the operation...");
                    return "User aborted the operation.";
                }

                int moveCount = 0;
                foreach(var move in moves)
                {
                    string sourcePath = Path.GetFullPath(Path.Combine(_sandBoxFolder, move.FileName));
                    string destinationFolder = Path.GetFullPath(Path.Combine(_sandBoxFolder, move.TargetSubfolder));
                    string destPath = Path.Combine(destinationFolder, move.FileName);

                    if(!sourcePath.StartsWith(_sandBoxFolder) || !destinationFolder.StartsWith(_sandBoxFolder) || !File.Exists(sourcePath))
                    {
                        continue;
                    }
                    
                    Directory.CreateDirectory(destinationFolder);
                    if (File.Exists(destPath))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(move.FileName);
                        string ext = Path.GetExtension(move.FileName);
                        destPath = Path.Combine(destinationFolder, $"{fileName}_{DateTime.Now:HHmmss}.{ext}" );
                    }
                    File.Move(sourcePath, destPath);
                    moveCount++;
                }
            return $"Successfully moved ${moveCount} into TestFolder";
        }
        
        public IList<AITool> ToAITools() =>
        [
            AIFunctionFactory.Create(ListFiles),
            AIFunctionFactory.Create(OrganizeFiles)
        ];
    }
}