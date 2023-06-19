namespace XRayMachineStatusManagement.Loggers
{
    public interface IMachineStatusLogger
    {
        void LogInformation(string message);
        void LogError(string message);
        void LogWarning(string message);
        void LogCritical(string message);
    }
}
