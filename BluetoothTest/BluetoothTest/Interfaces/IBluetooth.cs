using System.Collections.ObjectModel;

namespace BluetoothTest.Interfaces
{
    public interface IBluetooth
    {
        void Start(string name, string colour, int sleepTime, bool readAsCharArray);
        void Cancel();
        ObservableCollection<string> PairedDevices();
    }
}