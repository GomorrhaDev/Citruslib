namespace WobbleBridge
{
    public static class SettingsRegistry
    {
        public static void Register()
        {
            Wobble.ExtraSettings.AddSetting(new GameSetting
            {
                name = "Suppress Landlog",
                value = false.ToString(),
                description = "Prevents most landfall debug messages from being written."
            });

            Wobble.ExtraSettings.AddSetting(new GameSetting
            {
                name = "Disable Missing Command Parrot",
                value = false.ToString(),
                description = "Disables the unknown command parrot response when tryping an unknown command"
            });

            Wobble.ExtraSettings.AddSetting(new GameSetting
            {
                name = "AdminFileLocation",
                value = "",
                description = "filepath to a FOLDER where you want the PlayerPerms setting to be stored. Usefull when hosting multiple servers on the same computer leave blank to edit locally."
            });
        }
    }
}