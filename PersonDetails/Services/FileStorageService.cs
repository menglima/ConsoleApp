using Registration.Interfaces;

namespace Registration.Services
{
    public class FileStorageService : IStorageService
    {
        public readonly string _mainDirectory;
        private readonly string _personalFilePath;
        private readonly string _spouseDirectory;

        public FileStorageService()
        {
            // Currently uses local storage but can convert logic to web based storage 
            _mainDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "people");
            _personalFilePath = Path.Combine(_mainDirectory, "mainfile.txt");
            _spouseDirectory = Path.Combine(_mainDirectory, "spouses");

            if (!Directory.Exists(_spouseDirectory))
            {
                Directory.CreateDirectory(_spouseDirectory);
            }
            if (!Directory.Exists(_mainDirectory))
            {
                Directory.CreateDirectory(_mainDirectory);
            }
        }

        public void SavePersonalRecord(Person p, string spouseFilePath)
        {
            StreamWriter sw = new StreamWriter(_personalFilePath, true);
            using (sw)
            {
                string record = FormatPersonalDetails(p) + $" | {spouseFilePath ?? "null"}";
                sw.WriteLine(record);
            }
            Console.WriteLine("\nPersonal record saved to: " + _personalFilePath);
            if (p.MaritalStatus == "married")
            {
                Console.WriteLine("Spouses record saved to: " + _spouseDirectory);
            }
        }

        // Write to the spouse file and format their personal details
        public string SaveSpouseRecord(Person spouse)
        {
            // Create a unique file name if there are spouses with the same name. 
            string filePath = Path.Combine(_spouseDirectory, $"{spouse.FirstName}_{spouse.SurName}" + "_" + DateTime.Now.ToString("MMddyyyyHHmm") + ".txt");
            File.WriteAllText(filePath, FormatPersonalDetails(spouse)); 
            return filePath;
        }

        private string FormatPersonalDetails(Person p)
        {
            return $"{p.FirstName} | {p.SurName} | {p.DateOfBirth:MM-dd-yyyy} | {p.MaritalStatus} | {p.ParentAuthorization ?? "null"}";
        }
    }
}
