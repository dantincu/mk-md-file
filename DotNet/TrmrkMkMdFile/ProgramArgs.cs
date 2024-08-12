using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrmrkMkMdFile
{
    /// <summary>
    /// Contains arguments and flags passed in by the user and normalized with default values from config.
    /// </summary>
    internal class ProgramArgs
    {
        /// <summary>
        /// Gets or sets the work dir path, where the markdown file will be created.
        /// </summary>
        public string WorkDir { get; set; }

        /// <summary>
        /// Gets or sets the short folder name passed in by the user (it should be a 3 digits number like 999, 998... or 199, 198...
        /// This is mandatory and must be the first argument when creating a new note dir, as this app
        /// doesn't parse the existing entry names in the current folder to extract indexes and retrieve the next index.
        /// </summary>
        public string FileNamePrefix { get; set; }

        /// <summary>
        /// Gets or sets the note title passed in by the user.
        /// This is mandatory and must be the second argument when creating a new note dir.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the note title string that appears in the markdown file.
        /// </summary>
        public string MdTitleStr { get; set; }

        /// <summary>
        /// Gets or sets the part of the full file name that is appended after the file name prefix and the join str.
        /// </summary>
        public string FullFileNamePart { get; set; }

        /// <summary>
        /// Gets or sets the full file name join str.
        /// </summary>
        public string JoinStr { get; set; }

        /// <summary>
        /// Gets or sets the boolean value indicating whether the newly create markdown file will be open with
        /// the default program after the folders pair has been created. It is set to true if the user passes in
        /// the optional flag according to <see cref="ProgramConfig.OpenMdFileCmdArgName" /> config property.
        /// </summary>
        public bool OpenMdFile { get; set; }

        /// <summary>
        /// Gets or sets the markdown file name.
        /// </summary>
        public string MdFileName { get; set; }

        /// <summary>
        /// Gets or sets the boolean value indicating whether instead of creating a new markdown file,
        /// the name of an existing markdown file should be updated.
        /// </summary>
        public string UpdateMdFileName { get; set; }
    }
}
