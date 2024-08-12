using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrmrkMkMdFile
{
    /// <summary>
    /// DTO containing config values that are deserialized from the text contents that have been read from the config file.
    /// </summary>
    public class ProgramConfig
    {
        /// <summary>
        /// Gets or sets the markdown file name extension.
        /// </summary>
        public string MdFileNameExtension { get; set; }

        /// <summary>
        /// Gets or sets the full file name join str.
        /// </summary>
        public string FullFileNameJoinStr { get; set; }

        /// <summary>
        /// Gets or sets the name of the program arguments flag indicating the work dir path.
        /// </summary>
        public string WorkDirCmdArgName { get; set; }

        /// <summary>
        /// Gets or sets the name of the program arguments flag indicating whether the newly created markdown file
        /// should be open in the default program after the pair of folders has been created.
        /// </summary>
        public string OpenMdFileCmdArgName { get; set; }

        /// <summary>
        /// Gets or sets the name of the program arguments flag indicating that instead of a note item, a pair of folders
        /// for the note files should be created.
        /// </summary>
        public string UpdateMdFileNameCmdArgName { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of characters the full dir name part should contain.
        /// </summary>
        public int MaxDirNameLength { get; set; }

        /// <summary>
        /// Gets or sets the markdown file contents template. It will be provided as the first argument to the
        /// <see cref="string.Format(string, object?)" /> method call when creating the markdown file for a newly created
        /// note item, the second one being the title of the note provided by the user.
        /// </summary>
        public string MdFileContentsTemplate { get; set; }

        /// <summary>
        /// Gets or sets the title macros.
        /// </summary>
        public Dictionary<string, string> TitleMacros { get; set; }
    }
}
