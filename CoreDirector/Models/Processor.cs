namespace CoreDirector.Models
{
    internal class Processor
    {
        #region Properties
        public string Name { get; init; }

        public int ThreadCount { get; init; }

        public int TotalCoreCount { get; init; }

        public int EfficientCoreCount => TotalCoreCount - PerformanceCoreCount;

        public int PerformanceCoreCount => ThreadCount - TotalCoreCount;

        public ArchitectureType Type => TotalCoreCount * 2 != ThreadCount
            ? ArchitectureType.BigLittle
            : ArchitectureType.Normal;
        #endregion

        #region Constructor
        public Processor(string name)
        {
            Name = name;
        }
        #endregion
    }
}
