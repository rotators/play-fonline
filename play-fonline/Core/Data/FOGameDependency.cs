namespace PlayFOnline.Data
{
    /// <summary>
    /// Fallout .dat files and so on not supplied with game.
    /// </summary>
    public class FOGameDependency
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public FOScriptInfo Script { get; set; }
    }
}