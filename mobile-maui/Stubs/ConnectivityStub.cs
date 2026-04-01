using Microsoft.Maui.Networking;

// Compatibility shim for Plugin.Connectivity -> MAUI Connectivity
// CrossConnectivity.Current.IsConnected -> CrossConnectivity.Current.IsConnected
namespace BioBrain.Stubs
{
    public static class CrossConnectivity
    {
        public static ConnectivityAdapter Current { get; } = new();
    }

    public class ConnectivityAdapter
    {
        public bool IsConnected =>
            Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    }
}
