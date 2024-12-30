namespace Registration.Interfaces
{
    // This service is for any user inputs through the console
    public interface IUserInputService
    {
        Person PromptUserDetails(bool skipMarriedPrompt);
        bool VerifyAgeAndConsent(Person person, int age);
    }
}
