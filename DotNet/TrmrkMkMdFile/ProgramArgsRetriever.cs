using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrmrkMkMdFile;

namespace TrmrkMkMdFile
{
    /// <summary>
    /// Component that parses the raw string arguments passed in by the user and retrieves an object
    /// containing the normalized argument values used by the program component.
    /// </summary>
    internal class ProgramArgsRetriever
    {
        /// <summary>
        /// The program config retriever (the component that retrieves the normalized config values).
        /// </summary>
        private readonly ProgramConfigRetriever cfgRetriever;

        /// <summary>
        /// An object containing the normalized config values.
        /// </summary>
        private readonly ProgramConfig config;

        /// <summary>
        /// The only constructor of the component which initializes the config object.
        /// </summary>
        public ProgramArgsRetriever(
            ProgramConfigRetriever cfgRetriever)
        {
            this.cfgRetriever = cfgRetriever ?? throw new ArgumentNullException(
                nameof(cfgRetriever));

            config = cfgRetriever.Config.Value;
        }

        /// <summary>
        /// The main method of the component that parses the raw string arguments passed in by the user and retrieves an object
        /// containing the normalized argument values used by the program component
        /// </summary>
        /// <param name="args">The raw string arguments passed in by the user</param>
        /// <returns>An object containing the normalized argument values used by the program component</returns>
        /// <exception cref="ArgumentNullException">Gets thrown when either the file name prefix or the full file name part
        /// is not provided by the user or is an empty or al white spaces string.</exception>
        public ProgramArgs GetProgramArgs(
            string[] args)
        {
            var pgArgs = new ProgramArgs();
            var nextArgs = args.ToList();

            SeekFlag(nextArgs, config.WorkDirCmdArgName, (flagValue, idx) =>
            {
                pgArgs.WorkDir = flagValue;

                if (!Path.IsPathRooted(pgArgs.WorkDir))
                {
                    pgArgs.WorkDir = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        pgArgs.WorkDir);
                }
            }, () =>
            {
                pgArgs.WorkDir = Directory.GetCurrentDirectory();
            });

            SeekFlag(nextArgs, config.UpdateMdFileNameCmdArgName, (flagValue, idx) =>
            {
                OnUpdateMdFileName(pgArgs, nextArgs, flagValue);
            },
            () =>
            {
                SeekFlag(nextArgs, config.OpenMdFileCmdArgName, (flagValue, idx) =>
                {
                    pgArgs.OpenMdFile = true;
                });

                OnCreateMdFile(pgArgs, nextArgs);
            });

            return pgArgs;
        }

        /// <summary>
        /// Normalizes the full file name part starting from the title passed in by the user, sanitizing it
        /// by replacing the <c>/</c> character with the <c>%</c> character and all the file system entry name
        /// invalid characters with the space character. Then, if the length of the result string is greater than
        /// the value specified by the <see cref="ProgramConfig.MaxDirNameLength" /> config property,
        /// the string is trimmed by removing the exceeding characters after the maximum allowed number of characters
        /// has been exceeded starting from the start of the string.
        /// </summary>
        /// <param name="fullFileNamePart">The initial value of the full file name part that is the
        /// title provided by the user that has been just trimmed of starting and trailling white spaces.</param>
        /// <returns>The normalized full folder name part that can be used as such when appending it to the
        /// short folder name and join string or when creating the name of the markdown file.</returns>
        public string NormalizeFullFileNamePart(
            string fullFileNamePart)
        {
            fullFileNamePart = fullFileNamePart.Replace('/', '%').Split(
                Path.GetInvalidFileNameChars(),
                StringSplitOptions.RemoveEmptyEntries).JoinStr(" ").Trim();

            if (fullFileNamePart.Length > config.MaxDirNameLength)
            {
                fullFileNamePart = fullFileNamePart.Substring(
                    0, config.MaxDirNameLength);

                fullFileNamePart = fullFileNamePart.TrimEnd();
            }

            if (fullFileNamePart.Last() == '.')
            {
                fullFileNamePart += "%";
            }

            return fullFileNamePart;
        }

        /// <summary>
        /// Normalizes the provided title.
        /// </summary>
        /// <param name="title">The provided title</param>
        /// <returns>The normalized title</returns>
        public string NormalizeTitle(
            string? title)
        {
            title = title?.Trim().Nullify();

            if (title != null)
            {
                if (title.StartsWith(":"))
                {
                    title = title.Substring(1);
                }

                title = string.Join("|", title.Split("||").Select(
                    part =>
                    {
                        foreach (var kvp in config.TitleMacros)
                        {
                            part = part.Replace(
                                kvp.Key, kvp.Value);
                        }

                        return part;
                    }));
            }

            return title;
        }

        /// <summary>
        /// Normalizes the provided args.
        /// </summary>
        /// <param name="pgArgs">The provided args</param>
        public void NormalizeArgs(
            ProgramArgs pgArgs,
            List<string>? nextArgs = null)
        {
            if (pgArgs.FullFileNamePart == null)
            {
                pgArgs.FullFileNamePart = pgArgs.Title;

                pgArgs.FullFileNamePart = NormalizeFullFileNamePart(
                    pgArgs.FullFileNamePart!);
            }

            pgArgs.JoinStr ??= nextArgs?.FirstOrDefault(
                ) ?? config.FullFileNameJoinStr;

            pgArgs.FileNamePrefix ??= pgArgs.UpdateMdFileName?.Split('*')[0].With(
                str => str.Substring(0, str.Length - pgArgs.JoinStr.Length));

            pgArgs.MdFileName ??= string.Join(
                pgArgs.JoinStr, pgArgs.FileNamePrefix,
                pgArgs.FullFileNamePart) + config.MdFileNameExtension;
        }

        /// <summary>
        /// Handles the command line arg that matches the update markdown file name option.
        /// </summary>
        /// <param name="pgArgs">The program args parsed so far</param>
        /// <param name="nextArgs">The list of command line args that have not yet been parsed</param>
        private void OnUpdateMdFileName(
            ProgramArgs pgArgs,
            List<string> nextArgs,
            string flagValue)
        {
            pgArgs.UpdateMdFileName = flagValue;

            if (nextArgs.Any())
            {
                pgArgs.Title = NormalizeTitle(
                    nextArgs[0]);

                pgArgs.MdTitleStr = HttpUtility.HtmlEncode(
                    pgArgs.Title);

                nextArgs.RemoveAt(0);

                if (nextArgs.Any())
                {
                    pgArgs.JoinStr = nextArgs[0];
                    nextArgs.RemoveAt(0);
                }
                else
                {
                    pgArgs.JoinStr = config.FullFileNameJoinStr;
                }
            }
            else
            {
                pgArgs.JoinStr = config.FullFileNameJoinStr;
            }
        }

        /// <summary>
        /// Handles the case when the pair of folders will be created.
        /// </summary>
        /// <param name="pgArgs">The program args parsed so far</param>
        /// <param name="nextArgs">The list of command line args that have not yet been parsed</param>
        private void OnCreateMdFile(
            ProgramArgs pgArgs,
            List<string> nextArgs)
        {
            pgArgs.FileNamePrefix = nextArgs[0].Trim(
                ).Nullify() ?? throw new ArgumentNullException(
                    nameof(pgArgs.FileNamePrefix));

            nextArgs.RemoveAt(0);

            if (nextArgs.Any())
            {
                pgArgs.Title = NormalizeTitle(
                    nextArgs[0]);

                pgArgs.MdTitleStr = HttpUtility.HtmlEncode(
                    pgArgs.Title);

                nextArgs.RemoveAt(0);
            }

            NormalizeArgs(pgArgs, nextArgs);
        }

        /// <summary>
        /// Searches the list of command line args for a one starting with the provided flag prefix.
        /// </summary>
        /// <param name="nextArgs">The list of command line args that have not yet been parsed</param>
        /// <param name="flagName">The flag name</param>
        /// <param name="flagValueCallback">The callback to be called upon finding a matching command line arg</param>
        /// <param name="defaultCallback">The callback to be called when no matching command line arg has been found</param>
        /// <returns>The substring of the matching command line arg that starts after the flag name prefix if
        /// a match has been found, or the <c>null</c> value if no such match has been found.</returns>
        private string? SeekFlag(
            List<string> nextArgs,
            string flagName,
            Action<string, int> flagValueCallback,
            Action defaultCallback = null)
        {
            var boolFlagNamePrefix = $":{flagName}";
            var flagNamePrefix = $"{boolFlagNamePrefix}:";

            var kvp = nextArgs.FirstKvp(
                arg => arg == boolFlagNamePrefix || arg.StartsWith(
                    flagNamePrefix));

            string? flagValue = null;

            if (kvp.Key >= 0)
            {
                if (kvp.Value != boolFlagNamePrefix)
                {
                    flagValue = kvp.Value.Substring(
                        flagNamePrefix.Length);
                }

                nextArgs.RemoveAt(kvp.Key);

                flagValueCallback(
                    flagValue,
                    kvp.Key);
            }
            else
            {
                defaultCallback?.Invoke();
            }

            return flagValue;
        }
    }
}
