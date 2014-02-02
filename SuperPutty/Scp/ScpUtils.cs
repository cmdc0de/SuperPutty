namespace SuperPutty.Scp
{
    class ScpUtils
    {
        public static string GetHomeDirectoryForUsername(string username)
        {
            return (username == "root")
                ? "/root"
                : string.Format("/home/{0}", username);
        }
    }
}
