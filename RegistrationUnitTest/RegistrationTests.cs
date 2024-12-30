using Moq;
using Registration;
using Registration.Interfaces;
using System.Text.RegularExpressions;

namespace RegistrationUnitTest
{
    [TestClass]
    public sealed class RegistrationTests
    {
        private Mock<IStorageService> _mockStorageService;
        private Mock<IUserInputService> _mockInputService;
        private Registration.Registration _registration;

        [TestMethod]
        public void Run_Registration_IfAgeLessThan16()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockInputService = new Mock<IUserInputService>();
            _registration = new Registration.Registration(_mockStorageService.Object, _mockInputService.Object);

            var person = new Person
            {
                FirstName = "Jen",
                SurName = "Kim",
                DateOfBirth = DateTime.Today.AddYears(-15), // 15 years old
                MaritalStatus = "Single"
            };

            _mockInputService.Setup(input => input.PromptUserDetails(It.IsAny<bool>())).Returns(person);
            
            _registration.Run();

            _mockStorageService.Verify(storage => storage.SavePersonalRecord(It.IsAny<Person>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Run_IfParentalConsentNotProvided_AndAgeLessThan18()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockInputService = new Mock<IUserInputService>();
            _registration = new Registration.Registration(_mockStorageService.Object, _mockInputService.Object);

            var person = new Person
            {
                FirstName = "Jonathan",
                SurName = "Meng-Lim",
                DateOfBirth = DateTime.Today.AddYears(-17), // 17 years old
                ParentAuthorization = "no",
                MaritalStatus = "Single"
            };

            _mockInputService.Setup(input => input.PromptUserDetails(It.IsAny<bool>())).Returns(person);

            // Simulate no parental consent
            Console.SetIn(new StringReader("no"));

            _registration.Run();

            _mockStorageService.Verify(storage => storage.SavePersonalRecord(It.IsAny<Person>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Run_SaveRecord_WithParentalConsent_For17YearOld()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockInputService = new Mock<IUserInputService>();
            _registration = new Registration.Registration(_mockStorageService.Object, _mockInputService.Object);

            var person = new Person
            {
                FirstName = "Michelle",
                SurName = "Chan",
                DateOfBirth = DateTime.Today.AddYears(-17), // 17 years old
                ParentAuthorization = "yes",
                MaritalStatus = "Single"
            };

            _mockInputService.Setup(input => input.PromptUserDetails(It.IsAny<bool>())).Returns(person);

            // Simulate parental consent
            Console.SetIn(new StringReader("yes"));

            _mockInputService.Setup(input => input.VerifyAgeAndConsent(It.IsAny<Person>(), 17)).Returns(true);

            _registration.Run();

            _mockStorageService.Verify(storage => storage.SavePersonalRecord(It.Is<Person>(p => p.FirstName == "Michelle" && p.SurName == "Chan"
            && p.MaritalStatus == "Single" && p.ParentAuthorization == "yes" && p.DateOfBirth == DateTime.Today.AddYears(-17)), null), Times.Once);
        }

        [TestMethod]
        public void Run_SavesSpouseAndPersonalRecord_IfMarried()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockInputService = new Mock<IUserInputService>();
            _registration = new Registration.Registration(_mockStorageService.Object, _mockInputService.Object);

            _mockInputService.SetupSequence(input => input.PromptUserDetails(It.IsAny<bool>()))
                .Returns(new Person
                {
                    FirstName = "John",
                    SurName = "Smith",
                    DateOfBirth = DateTime.Today.AddYears(-30), // 30 years old
                    MaritalStatus = "Married"
                })
                .Returns(new Person
                {
                    FirstName = "Steph",
                    SurName = "Smith",
                    DateOfBirth = DateTime.Today.AddYears(-28),
                });

            _mockInputService.Setup(input => input.VerifyAgeAndConsent(It.IsAny<Person>(), It.IsAny<int>())).Returns(true);

            _registration.Run();

            _mockStorageService.Verify(storage => storage.SaveSpouseRecord(It.Is<Person>(
                p => p != null && p.FirstName == "Steph" && p.SurName == "Smith" && p.DateOfBirth == DateTime.Today.AddYears(-28))),
                Times.Once);

            _mockStorageService.Verify(storage => storage.SavePersonalRecord(It.Is<Person>(
                p => p.FirstName == "John" && p.SurName == "Smith" && p.DateOfBirth == DateTime.Today.AddYears(-30) && p.MaritalStatus == "Married"),
                It.IsAny<string>()),
                Times.Once);
        }

        [TestMethod]
        public void Run_SavePersonalRecord_IfOver18_Single()
        {
            _mockStorageService = new Mock<IStorageService>();
            _mockInputService = new Mock<IUserInputService>();
            _registration = new Registration.Registration(_mockStorageService.Object, _mockInputService.Object);

            _mockInputService.Setup(input => input.PromptUserDetails(It.IsAny<bool>()))
                .Returns(new Person
                {
                    FirstName = "Anthony",
                    SurName = "Meng-Lim",
                    DateOfBirth = DateTime.Today.AddYears(-30), // 30 years old
                    MaritalStatus = "Single"
                });

            _mockInputService.Setup(input => input.VerifyAgeAndConsent(It.IsAny<Person>(), It.IsAny<int>())).Returns(true);

            _registration.Run();

            _mockStorageService.Verify(storage => storage.SavePersonalRecord(It.Is<Person>(
                p => p.FirstName == "Anthony" && p.SurName == "Meng-Lim" && p.DateOfBirth == DateTime.Today.AddYears(-30) && p.MaritalStatus == "Single"), It.IsAny<string>()), Times.Once);
        }
    }
}


