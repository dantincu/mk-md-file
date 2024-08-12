using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TrmrkMkMdFile
{
    /// <summary>
    /// The component that retrieves and normalizes the config values from the config file.
    /// </summary>
    internal class ProgramConfigRetriever
    {
        /// <summary>
        /// The default markdown file name extension.
        /// </summary>
        const string MD_FILE_NAME_EXTENSION = ".md";

        /// <summary>
        /// The default name for the work dir user arguments flag.
        /// </summary>
        const string WORK_DIR = "wd";

        /// <summary>
        /// The name of the file that contains json serialized representation of the configuration values.
        /// </summary>
        const string CONFIG_FILE_NAME = "trmrk-config.json";

        /// <summary>
        /// The default value for the string used for joining the short folder name with the full folder name part
        /// when creating the full folder name.
        /// </summary>
        const string JOIN_STR = "-";

        /// <summary>
        /// The default name for the user arguments flag that indicates whether the newly created markdown file
        /// should be open in the default program after the pair of folders have been created.
        /// </summary>
        const string OPEN_MD_FILE = "o";

        /// <summary>
        /// The default name for the update markdown file name user arguments flag.
        /// </summary>
        const string UPDATE_MD_FILE_NAME = "u";

        /// <summary>
        /// The default value for the markdown file text contents template.
        /// </summary>
        const string MD_FILE_CONTENTS_TEMPLATE = "# {0}  \n\n";

        /// <summary>
        /// The default value for the maximum number of characters allowed for the full folder name part.
        /// </summary>
        const int MAX_DIR_NAME_LEN = 100;

        /// <summary>
        /// The only constructor of the singleton component where the config is being read from the disk and normalized.
        /// </summary>
        public ProgramConfigRetriever()
        {
            Config = new Lazy<ProgramConfig>(() => GetConfig());
        }

        /// <summary>
        /// Gets the path of the config file.
        /// </summary>
        public static string ConfigFilePath { get; } = Path.Combine(
            UtilsH.ExecutingAssemmblyPath, CONFIG_FILE_NAME);

        /// <summary>
        /// Gets the object containing the normalized config values.
        /// </summary>
        public Lazy<ProgramConfig> Config { get; }

        /// <summary>
        /// Returns an object containing the normalized config values.
        /// </summary>
        /// <returns>An object containing the normalized config values.</returns>
        private ProgramConfig GetConfig()
        {
            ProgramConfig programConfig;

            if (File.Exists(ConfigFilePath))
            {
                programConfig = GetConfigCore();
            }
            else
            {
                programConfig = new ProgramConfig();
            }

            NormalizeConfig(programConfig);
            return programConfig;
        }

        /// <summary>
        /// Normalizes the config values by assigning hardcoded constants to properties whose values are <c>null</c>.
        /// </summary>
        /// <param name="config"></param>
        private void NormalizeConfig(
            ProgramConfig config)
        {
            config.MdFileNameExtension ??= MD_FILE_NAME_EXTENSION;
            config.WorkDirCmdArgName ??= WORK_DIR;
            config.FullFileNameJoinStr ??= JOIN_STR;
            config.OpenMdFileCmdArgName ??= OPEN_MD_FILE;
            config.UpdateMdFileNameCmdArgName ??= UPDATE_MD_FILE_NAME;
            config.MaxDirNameLength = config.MaxDirNameLength.Nullify() ?? MAX_DIR_NAME_LEN;
            config.MdFileContentsTemplate ??= MD_FILE_CONTENTS_TEMPLATE;
            config.TitleMacros ??= new Dictionary<string, string>();
        }

        /// <summary>
        /// Reads the contents of the config file, deserializes it into an object and returns that object.
        /// </summary>
        /// <returns>An object obtained by deserializing the contents of the config file.</returns>
        private ProgramConfig GetConfigCore()
        {
            string json = File.ReadAllText(ConfigFilePath);
            var programConfig = JsonSerializer.Deserialize<ProgramConfig>(json);

            return programConfig;
        }
    }
}
