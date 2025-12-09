namespace ZIMAeTicket
{
    public static class Constants
    {
        public const string DatabaseFilename = "ZIMAeTicket.db3";

        public static string DatabasePath =>
            Path.Combine(FileSystem.Current.AppDataDirectory, DatabaseFilename);
    }
}
