public class SettingsLosa
{
    public bool is_WebSocket { get; set; } = false;
    public EarthquakeAPISettings EarthquakeAPI { get; set; } = new EarthquakeAPISettings();
    public EEWAPISettings EEWAPI { get; set; } = new EEWAPISettings();
    public TunamiAPISettings TunamiAPI { get; set; } = new TunamiAPISettings();
    public class EarthquakeAPISettings
    {
        public int selectedAPI { get; set; }
    }
    public class EEWAPISettings
    {
        public int selectedAPI { get; set; }
    }
    public class TunamiAPISettings
    {
        public int selectedAPI { get; set; }
    }
}
