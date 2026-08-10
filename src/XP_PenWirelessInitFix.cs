using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace XP_Pen.Wireless.Init.Fix;

public class XP_PenWirelessInitFix : WirelessInitializerFixBase, IPositionedPipelineElement<IDeviceReport>
{
    #region Constants

    public const string PLUGIN_NAME = "XP-Pen Wireless Init Fix";

    #endregion

    #region Fields

    private bool _isInitialized = false;

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
    public string InitializationData { get; set; } = string.Empty;

    #endregion

    #region Events

    public event Action<IDeviceReport>? Emit;

    #endregion

    #region Methods

    public void Consume(IDeviceReport value)
        => Emit?.Invoke(value);

    public override void PostInitialize()
    {
        if (!_isInitialized)
        {
            byte[] data;

            try
            {
                data = Convert.FromBase64String(InitializationData);
            }
            catch
            {
                Log.Write(PLUGIN_NAME, $"Could not convert '{InitializationData}' to Base64", LogLevel.Error);
                return;
            }

            IntializeTablet(data);

            _isInitialized = true;
        }
    }    

    #endregion
}
