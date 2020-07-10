namespace VirtualOrgan.PcService
{
    interface ITaskFactory
    {
        Operation Operation { get; }
        ITaskWrapper Create();
    }
}