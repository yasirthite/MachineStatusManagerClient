// -----------------------------------------------------------------------
// Copyright (c) WebEngineers Software India LLP, All rights reserved.
// Licensed under the MIT License.
// Source-Code modification requires explicit permission by the licensee
// -----------------------------------------------------------------------

namespace XRayMachineStatusManagement
{
    /// <summary>
    /// Enum representing a 4-Digit sensor signal.
    /// </summary>
    public enum SensorCode
    {
        #region MACHINE STATUS MANAGER CUSTOM CODES
        Empty = -1,
        SourceOnCircuitBreaker = -2,
        FaultySensorBlink = -3,
        #endregion MACHINE STATUS MANAGER CUSTOM CODES

        #region SENSOR FWD CODES
        S1_ON_FWD = 4849,
        S1_OFF_FWD = 5849,

        S2_ON_FWD = 4841,
        S2_OFF_FWD = 5841,

        S3_ON_FWD = 4844,
        S3_OFF_FWD = 5844,

        S4_ON_FWD = 4845,
        S4_OFF_FWD = 5845,

        S5_ON_FWD = 4846,
        S5_OFF_FWD = 5846,
        #endregion SENSOR FWD CODES

        #region BELT CODES
        BELT_FWD = 2168,
        BELT_PAUSE = 2169,
        BELT_REV = 2170,
        BELT_RESUME_FWD = 6666,
        BELT_RESUME_REV = 6668,
        #endregion BELT CODES

        #region SENSOR REV CODES
        S1_ON_REV = 4834,
        S1_OFF_REV = 5834,

        S2_ON_REV = 4836,
        S2_OFF_REV = 5836,

        S3_ON_REV = 4838,
        S3_OFF_REV = 5838,

        S4_ON_REV = 4839,
        S4_OFF_REV = 5839,

        S5_ON_REV = 4835,
        S5_OFF_REV = 5835,
        #endregion SENSOR REV CODES

        #region KEYBOARD CODES
        EMERGENCY_STOP_PRESSED = 4801,
        EMERGENCY_STOP_RELEASED = 4800 
        #endregion
    }
}
