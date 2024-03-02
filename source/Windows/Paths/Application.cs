namespace ChaosUtil.Platform.Paths
{
    /// <summary> Provides functions to retrieve information about the running application. </summary>
    public static class Application
    {
        /// <summary> Returns the path to the application's executable file. </summary>
        public static string GetExecutableFile() => System.Windows.Forms.Application.ExecutablePath;

        /// <summary> Returns the directory the application's executable file lies in. </summary>
        public static string GetExecutableDirectory() => System.IO.Path.GetDirectoryName(GetExecutableFile());
    }
}
