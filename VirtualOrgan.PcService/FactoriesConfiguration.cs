namespace VirtualOrgan.PcService
{
    sealed class FactoriesConfiguration
    {
        public string AudioCard { get; set; } = "motu";
        public string ShutdownProvider { get; set; } = "win32";
    }
}