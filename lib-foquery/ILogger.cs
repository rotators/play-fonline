﻿namespace FOQuery
{
    public interface ILogger
    {
        void Debug(string message, params object[] args);
        void Error(string message, params object[] args);
        void Fatal(string message, params object[] args);
        void Trace(string message, params object[] args);
        void Warn(string message, params object[] args);
    }
}