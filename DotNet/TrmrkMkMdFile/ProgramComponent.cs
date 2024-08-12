using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TrmrkMkMdFile
{
    /// <summary>
    /// The program's main component that does the core part of the program's execution.
    /// </summary>
    internal class ProgramComponent
    {
        /// <summary>
        /// The program args retriever component.
        /// </summary>
        private readonly ProgramArgsRetriever pgArgsRetriever;

        /// <summary>
        /// The program config retriever component.
        /// </summary>
        private readonly ProgramConfigRetriever pgCfgRetriever;

        /// <summary>
        /// The only constructor containing the component dependencies.
        /// </summary>
        /// <param name="pgArgsRetriever">The program args retriever component</param>
        /// <exception cref="ArgumentNullException">Gets thrown when the value for <see cref="pgArgsRetriever" />
        /// is <c>null</c></exception>
        public ProgramComponent(
            ProgramArgsRetriever pgArgsRetriever,
            ProgramConfigRetriever pgCfgRetriever)
        {
            this.pgArgsRetriever = pgArgsRetriever ?? throw new ArgumentNullException(
                nameof(pgArgsRetriever));

            this.pgCfgRetriever = pgCfgRetriever ?? throw new ArgumentNullException(
                nameof(pgCfgRetriever));
        }

        /// <summary>
        /// The component's main method that runs the program.
        /// </summary>
        /// <param name="args">The raw command line args</param>
        public void Run(
            string[] args)
        {
            var pgArgs = pgArgsRetriever.GetProgramArgs(args);
            Run(pgArgs);
        }

        /// <summary>
        /// Runs the core part of the program.
        /// </summary>
        /// <param name="pgArgs">The program args parsed from the user provided arguments
        /// and normalized with the config values.</param>
        public void Run(
            ProgramArgs pgArgs)
        {
            var config = pgCfgRetriever.Config.Value;
            string workDirPath = Path.GetFullPath(pgArgs.WorkDir);

            if (pgArgs.UpdateMdFileName != null)
            {
                UpdateMdFile(pgArgs, config, workDirPath);
            }
            else
            {
                CreateMdFile(pgArgs, config, workDirPath);
            }
        }

        /// <summary>
        /// Updates the name of a markdown file.
        /// </summary>
        /// <param name="pgArgs">The program args parsed from the user provided arguments and normalized with the config values.</param>
        /// <param name="config">The config object containing the normalized config values.</param>
        /// <param name="workDirPath">The work dir path that has either been provided by the user or
        /// assigned the value of <see cref="Directory.GetCurrentDirectory()" />.</param>
        private void UpdateMdFile(
            ProgramArgs pgArgs,
            ProgramConfig config,
            string workDirPath)
        {
            var filesArr = Directory.GetFiles(
                workDirPath,
                pgArgs.UpdateMdFileName);

            if (filesArr.Length != 1)
            {
                throw new ArgumentException(
                    $"Found {filesArr.Length} matching the provided pattern");
            }

            var filePath = filesArr.Single();
            string fileName = Path.GetFileName(filePath);

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($"Found matching file: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(fileName);
            Console.ResetColor();

            if (pgArgs.Title == null)
            {
                pgArgs.Title = GetMdTitle(
                    filePath,
                    out string mdTitleStr);

                pgArgs.MdTitleStr ??= mdTitleStr;
            }
            else if (pgArgs.MdTitleStr == null)
            {
                pgArgs.MdTitleStr = HttpUtility.HtmlDecode(pgArgs.Title);
            }
            
            pgArgsRetriever.NormalizeArgs(pgArgs);

            var newFilePath = Path.Combine(
                workDirPath,
                pgArgs.MdFileName);

            if (filePath != newFilePath)
            {
                if (!File.Exists(newFilePath) && !Directory.Exists(newFilePath))
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write($"Renaming file to: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(pgArgs.MdFileName);
                    Console.ResetColor();

                    File.Move(filePath, newFilePath);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"An entry with this name already exists: {pgArgs.MdFileName}");
                }
            }
        }

        /// <summary>
        /// Creates a markdown file.
        /// </summary>
        /// <param name="pgArgs">The program args parsed from the user provided arguments and normalized with the config values.</param>
        /// <param name="config">The config object containing the normalized config values.</param>
        /// <param name="workDirPath">The work dir path that has either been provided by the user or
        /// assigned the value of <see cref="Directory.GetCurrentDirectory()" />.</param>
        private void CreateMdFile(
            ProgramArgs pgArgs,
            ProgramConfig config,
            string workDirPath)
        {
            string mdFileContents = string.Format(
                config.MdFileContentsTemplate,
                pgArgs.MdTitleStr);

            string mdFilePath = Path.Combine(
                workDirPath,
                pgArgs.MdFileName);

            File.WriteAllText(
                mdFilePath,
                mdFileContents);
        }

        /// <summary>
        /// Retrieves the title from the provided markdown file.
        /// </summary>
        /// <param name="mdFilePath">The provided markdown file path</param>
        /// <param name="mdTitleStr">The markdown title string.</param>
        /// <returns>The title extracted from the provided markdown file.</returns>
        private string GetMdTitle(
            string mdFilePath,
            out string mdTitleStr)
        {
            mdTitleStr = File.ReadAllLines(mdFilePath).First(
                line => line.StartsWith("# ")).Substring("# ".Length).Trim();

            string title = HttpUtility.HtmlDecode(mdTitleStr);
            return title;
        }
    }
}
