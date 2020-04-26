namespace VirtualOrgan.PcService
{
    interface IFactory<out T>
    {
        T Create();
    }
}