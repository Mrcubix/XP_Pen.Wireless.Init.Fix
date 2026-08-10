using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace XP_Pen.Wireless.Init.Fix;

[PluginName(PLUGIN_NAME)]
public class XP_PenWirelessInitFix : WirelessInitializerFixBase, IPositionedPipelineElement<IDeviceReport>
{
    #region Constants

    public const string PLUGIN_NAME = "XP-Pen Wireless Init Fix";

    #endregion

    #region Fields

    private bool _isInitialized = false;
    private bool _shouldReport = false;
    private bool _isOn = true;

    #endregion

    #region Properties

    public PipelinePosition Position => PipelinePosition.None;

    [Property("Initialization data"),
     DefaultPropertyValue("ArAE"),
     ToolTip("XP-Pen Wireless Init Fix: \n\n" + 
             "XP-Pen tablet required a report to be sent ot the tablet to switch between\n" + 
             "Vendor or Plug & Play Modes.\n" +
             "This value should be the same Base64 value as the one inside the configuration file.\n" +
             "Default value : ArAE")]
    public string InitializationData
    {
        get => Convert.ToBase64String(_initializationData);
        set
        {
            try
            {
                _initializationData = Convert.FromBase64String(InitializationData);
            }
            catch
            {
                Log.Write(PLUGIN_NAME, $"Could not convert '{InitializationData}' to Base64", LogLevel.Error);
                return;
            }
        }
    }

    [BooleanProperty("Additional debugging info", ""),
     DefaultPropertyValue(false),
     ToolTip("XP-Pen Wireless Init Fix: \n\n" + 
             "Additional debugging info in the Console tab.\n" + 
             "Default value : false")]
    public bool Debug { get; set; }

    [BooleanProperty("Report debugging info", ""),
     DefaultPropertyValue(false),
     ToolTip("XP-Pen Wireless Init Fix: \n\n" + 
             "Prints the raw reports to the Console tab.\n" + 
             "This will cause some lag on even high-end devices.\n" +
             "Default value : false")]
    public bool ReportDebug { get; set; }

    #endregion

    #region Events

    public event Action<IDeviceReport>? Emit;

    #endregion

    #region Methods

    public void Consume(IDeviceReport report)
    {
        if (report.Raw.Length > 3)
            _shouldReport = HandleReport(report);

        if (_shouldReport)
            Emit?.Invoke(report);
    }

    private bool HandleReport(IDeviceReport report)
    {
        bool hasAuxBitSet = report.Raw[1].IsBitSet(4);
        bool hasOfflineBitSet = report.Raw[1].IsBitSet(3);

        bool oldState = _isOn;

        if (hasAuxBitSet && hasOfflineBitSet && report.Raw[3] == 0x63)
        {
            _isOn = false;
            SendDebugLog("Device seems to have gone offline.");
        }
        else if (hasAuxBitSet && !hasOfflineBitSet)
        {
            _isOn = true;
            SendDebugLog("Device seems to have come online.");
        }

        SendDebugLog($"Device is {(_isOn ? "ON" : "OFF")}");
        
        if (ReportDebug)
            SendDebugLog($"Report: {BitConverter.ToString(report.Raw)}");

        if (_isOn != oldState && oldState == false && _isInitialized)
            IntializeTablet(_initializationData);

        return true;
    }

    private void SendDebugLog(string message)
    {
        if (Debug)
            Log.Write(PLUGIN_NAME, message, LogLevel.Debug);
    }

    #endregion
}
