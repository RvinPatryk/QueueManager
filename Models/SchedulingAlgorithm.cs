namespace QueueManager.Models
{
    public enum SchedulingAlgorithm
    {
        FIFO,
        LIFO,
        Priority,
        SJF,
        LJF,
        Random,
        EDF,
        WeightedRandom
    }
}