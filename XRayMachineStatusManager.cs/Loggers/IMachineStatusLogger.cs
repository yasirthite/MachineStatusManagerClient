// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------


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
