using System;

namespace ScopedUnitOfWork.Interfaces
{
    public static class ScopedUnitOfWorkConfiguration
    {
        public static bool IsDiagnosticsMode { get; set; }

        private static Action<string>_loggingAction;

        public static Action<string> LoggingAction
        {
            get
            {
                return entry =>
                {
                    if (IsDiagnosticsMode)
                        _loggingAction(entry);
                };
            }
        }

        public static void ConfigureDiagnosticsMode(Action<string> loggingAction, bool enableDiagnostics = false)
        {
            _loggingAction = loggingAction;
            IsDiagnosticsMode = enableDiagnostics;
        }
    }
}