namespace Registration.Interfaces
{
    // This service contains file based operations for storing registration data to their correct file locations
    public interface IStorageService
    {
        void SavePersonalRecord(Person person, string spouseFilePath);
        string SaveSpouseRecord(Person spouse);
    }
}
