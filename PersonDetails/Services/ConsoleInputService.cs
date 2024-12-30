using Registration.Interfaces;

namespace Registration.Services
{
    public class ConsoleInputService : IUserInputService
    {
        public Person PromptUserDetails(bool skipMarriedPrompt)
        {
            Console.Write("First Name: "); 
            string firstName = Console.ReadLine().Trim(); 

            Console.Write("Surname: ");
            string surname = Console.ReadLine().Trim();

            DateTime dateOfBirthResult;
            // Verify that the Date of Birth format is correct
            while (true)
            {
                Console.Write("Date of Birth (MM-DD-YYYY): ");
                string inputDoB = Console.ReadLine().Trim(); 
                if (DateTime.TryParse(inputDoB, out dateOfBirthResult))
                {
                    break;
                }

                Console.WriteLine("Please enter a valid 'Date of Birth' format. Needs to match MM-DD-YYYY.");
            }

            string maritalStatus = "married";
            // Skip the married prompt. If the person said that they are married, then the spouse doesn't need to answer again
            if (skipMarriedPrompt == true)
            {
                // Verify that the marital status format is correct
                while (true)
                {
                    Console.Write("Marital Status (Single or Married): ");
                    maritalStatus = Console.ReadLine().Trim().ToLower();
                    if ((maritalStatus == "single") | (maritalStatus == "married"))
                    {
                        break;
                    }

                    Console.WriteLine("Enter a valid 'Marital Status'. Either 'Single' or 'Married'");
                }
            }

            return new Person { FirstName = firstName, SurName = surname, DateOfBirth = dateOfBirthResult, MaritalStatus = maritalStatus };
        }

        public bool VerifyAgeAndConsent(Person personInfo, int age)
        {
            if (age < 16) 
            {
                Console.WriteLine("Registration denied. In order to register, you need to be at least 16 years old.");
                return false;
            }

            if (age < 18) 
            {
                Console.WriteLine("Do your parents allow you to register? (Yes/No): ");
                string parentConsent = Console.ReadLine().Trim();
                while (true)
                {
                    if ((parentConsent == "yes") | (parentConsent == "no"))
                    {
                        break;
                    }
                    Console.WriteLine("Please enter a valid entry. Either 'Yes' or 'No'");

                }
                if (parentConsent == "no")
                {
                    Console.WriteLine("Registration denied. You need your parents permission to register.");
                    return false;
                }
                personInfo.ParentAuthorization = parentConsent;
            }
            return true;
        }
    }
}
