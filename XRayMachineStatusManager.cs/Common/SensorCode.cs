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
        Empty = -1,
        SourceOnCircuitBreaker = -2,
        FaultySensorBlink = -3,

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

        BELT_FWD = 2168,
        BELT_STOP = 2169,
        BELT_REV = 2170
    }
}
