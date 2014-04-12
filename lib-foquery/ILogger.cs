namespace FOQuery
{
    public interface ILogger
    {
        public void Debug(string message, params object[] args);
        public void Error(string message, params object[] args);
        public void Fatal(string message, params object[] args);
        public void Trace(string message, params object[] args);
        public void Warn(string message, params object[] args);
    }
}
