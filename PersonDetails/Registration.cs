using Registration.Interfaces;

namespace Registration
{
    public class Registration
    {
        private readonly IStorageService _storageService;
        private readonly IUserInputService _inputService;

        public Registration(IStorageService storageService, IUserInputService inputService)
        {
            _storageService = storageService;
            _inputService = inputService;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to the Registration!\n");

            Console.WriteLine("Please enter your personal details below: ");
            var personInfo = _inputService.PromptUserDetails(true);

            int age = CalcAge(personInfo.DateOfBirth);

            // Business Rules
            bool verify = _inputService.VerifyAgeAndConsent(personInfo, age);

            if (verify == true) // if they are over 18
            {
                // Save Spouse information if person is married
                string spouseFilePath = null;
                if (personInfo.MaritalStatus == "Married" || personInfo.MaritalStatus == "married")
                {
                    Console.WriteLine("\nPlease enter spouse details below: ");
                    var spouseInfo = _inputService.PromptUserDetails(false);
                    spouseFilePath = _storageService.SaveSpouseRecord(spouseInfo);
                }
                // Save the record to mainfile.txt and if theres a spouse, save to the spouseFilePath
                _storageService.SavePersonalRecord(personInfo, spouseFilePath); 
                Console.WriteLine("Registration Successful!");
            }
        }

        // Find the persons age based on their date of birth
        private static int CalcAge(DateTime dateOfBirth)
        {
            int age = DateTime.Today.Year - dateOfBirth.Year;
            // If the persons dob is after todays day, then it wasn't their birthday yet. Subtract 1 year from their dob
            if (dateOfBirth > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }
}
