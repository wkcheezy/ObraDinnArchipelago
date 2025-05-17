namespace ObraDinnArchipelago.Archipelago;

internal class ArchipelagoOptions
{
    internal static bool deathlink = false;
    // internal static bool randomizeCodes = false;
    // internal static Goal goal;
    // internal static bool skipEpilogue = false;
    
    internal static void SetupRandomizedCodes()
    {
        
    }
    

    internal static void RandomizeCodes(int seed)
    {
        // List<int> cabinSafeCode = new List<int>();
        // do
        // {
        //     int number = SeededRandom.Range(0, 9, seed++);
        //     if (!cabinSafeCode.Contains(number))
        //         cabinSafeCode.Add(number);
        // } while (cabinSafeCode.Count < 3);
        //
        // ArchipelagoData.Data.cabinSafeCode = cabinSafeCode;
        //
        // List<int> cabinClockCode = new List<int>();
        // do
        // {
        //     int number = SeededRandom.Range(0, 11, seed++);
        //     if (!cabinClockCode.Contains(number))
        //         cabinClockCode.Add(number);
        // } while (cabinClockCode.Count < 3);
        //
        // ArchipelagoData.Data.cabinClockCode = cabinClockCode;
        //
        // ArchipelagoData.Data.cabinSmallClockCode = new List<int> { 0, 0, SeededRandom.Range(0, 11, seed++) };
        //
        // ArchipelagoData.Data.factoryClockCode = new List<int> { 0, 0, SeededRandom.Range(0, 11, seed++) };
        //
        // ArchipelagoData.Data.wizardCode1 = new List<int>
        //     { SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++) };
        // ArchipelagoData.Data.wizardCode2 = new List<int>
        //     { SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++) };
        // ArchipelagoData.Data.wizardCode3 = new List<int>
        //     { SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++), SeededRandom.Range(0, 6, seed++) };
    }

    internal static void SkipTutorial()
    {
       
    }
}