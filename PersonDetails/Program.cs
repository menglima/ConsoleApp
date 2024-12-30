using Registration.Services;
using Registration.Interfaces;

namespace Registration
{
    class Program
    {
        static void Main()
        {
            IStorageService storageService = new FileStorageService();
            IUserInputService inputService = new ConsoleInputService();

            var registration = new Registration(storageService, inputService);
            registration.Run();
        }
    }
}
