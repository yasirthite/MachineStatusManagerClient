﻿using XRayMachineStatusManagement.Loggers;

namespace XRayMachineStatusManager.cs
{
    internal class FileLogger: IMachineStatusLogger
    {
        // code to log to a txt file "xRayMachineStatusManager.Log"
        public FileLogger()
        {
        
        }

        public void LogCritical(string message)
        {
            throw new System.NotImplementedException();
        }

        public void LogError(string message)
        {
            throw new System.NotImplementedException();
        }

        public void LogInformation(string message)
        {
            throw new System.NotImplementedException();
        }

        public void LogWarning(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
