using NXOpen.Layout2d;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingStructureCreation.Model
{
    public class ProjectInfo
    {
        string model;
        string part;
        string dwgCodePrefix;
        string designer;
        string item;
        int stnNumber;
        string version;

        const string DIRECTORY = "D:/NXCUSTOM/temp";
        const string INFO_FILENAME = "project_info.data";
        public const string MODEL = "MODEL";
        public const string PART = "PART";
        public const string CODE_PREFIX = "CODE_PREFIX";
        public const string DESIGNER = "DESIGNER";

        public ProjectInfo(string model, string part, string dwgCodePrefix, string designer)
        {
            this.model = model;
            this.part = part;
            this.dwgCodePrefix = dwgCodePrefix;
            this.designer = designer;
        }

        public ProjectInfo(string model, string part, string dwgCodePrefix, string designer, string item, int stnNumber, string version) : this(model, part, dwgCodePrefix, designer)
        {
            this.item = item;
            this.stnNumber = stnNumber;
            this.version = version;
        }

        public string Model => model;
        public string Part => part;
        public string DwgCodePrefix => dwgCodePrefix;
        public string Designer => designer;
        public string Item { get => item; set => item = value; }
        public int StnNumber { get => stnNumber; set => stnNumber = value; } 
        public string Version { get => version; set => version = value; }

        public static void WriteToFile(List<string> projectInfoToText)
        {
            if (!Directory.Exists(DIRECTORY))
            {
                Directory.CreateDirectory(DIRECTORY);
            }
            string fullPathFileName = Path.Combine(DIRECTORY, INFO_FILENAME);
            TextWriter tw = null;
            try
            {
                tw = new StreamWriter(fullPathFileName);
                foreach (string line in projectInfoToText)
                {
                    tw.WriteLine(line);
                }
            }
            catch (IOException ex)
            {
                string message = $"Error writing to file: {ex.Message}";
                string title = "Error writing file";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);
            }
            finally
            {
                tw?.Close();
            }
        }

        public static Dictionary<string, string> ReadFromFile()
        {
            string fullPathFileName = Path.Combine(DIRECTORY, INFO_FILENAME);
            if (!File.Exists(fullPathFileName))
            {
                string message = $"File not found: {fullPathFileName}";
                throw new FileNotFoundException(message);
            }

            Dictionary<string, string> result = new Dictionary<string, string>();
            string[] keys = new string[] { MODEL, PART, CODE_PREFIX, DESIGNER };

            try
            {
                using (StreamReader reader = new StreamReader(fullPathFileName))
                {
                    string value;
                    foreach (string key in keys)
                    {
                        value = reader.ReadLine();
                        result.Add(key, value);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                string message = $"File not found: {ex.Message}";
                string title = "Error file not found";
                NXDrawing.ShowMessageBox(title, NXOpen.NXMessageBox.DialogType.Error, message);
            }

            return result;
        }
    }
}
