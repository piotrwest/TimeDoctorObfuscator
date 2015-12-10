using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace TimeDoctorObfuscator
{
    public class KeepComputerAwake
    {
        private EXECUTION_STATE? _previousState;

        [Flags]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        public void MakeItAwake()
        {
            _previousState = SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        public void RestorePreviousState()
        {
            if(_previousState.HasValue)
                SetThreadExecutionState(_previousState.Value);
        }
    }
}