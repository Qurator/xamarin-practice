using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Util;

namespace BluetoothTest
{
    public class Bluetooth : Interfaces.IBluetooth
    {
        private CancellationTokenSource _cancellationToken { get; set; }

        const int RequestResolveError = 1000;

        public Bluetooth() { }

        public void Start(string name,string colour, int sleepTime = 200, bool readAsCharArray = false)
        {
            Task.Run(async () => loop(name, colour, sleepTime, readAsCharArray));
        }

        public void Cancel()
        {
            if (_cancellationToken != null)
            {
                System.Diagnostics.Debug.WriteLine("Cancelling Task");
                _cancellationToken.Cancel();
            }
        }

        public ObservableCollection<string> PairedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            ObservableCollection<string> devices = new ObservableCollection<string>();
            foreach (BluetoothDevice device in adapter.BondedDevices)
            {
                devices.Add(device.Name);
            }

            return devices;
        }

        private async Task loop(string name, string colour, int sleepTime, bool readAsCharArray)
        {
            BluetoothDevice device = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            BluetoothSocket socket = null;

            _cancellationToken = new CancellationTokenSource();
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Thread.Sleep(sleepTime);

                    if (adapter == null)
                    {
                        System.Diagnostics.Debug.WriteLine("No adapter found");
                        return;
                    }

                    if (!adapter.IsEnabled)
                    {
                        System.Diagnostics.Debug.WriteLine("Adapter is disabled");
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine($"Trying to connect to device: {name}");
                    foreach (BluetoothDevice bd in adapter.BondedDevices)
                    {
                        System.Diagnostics.Debug.WriteLine($"Device Found: {bd.Name}");
                        if (bd.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            System.Diagnostics.Debug.WriteLine($"Found: {bd.Name}. Trying to connect to it...");
                            device = bd;
                            break;
                        }
                    }

                    if (device == null)
                    {
                        System.Diagnostics.Debug.Write("Device not found");
                        return;
                    }

                    UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
                    if ((int)Android.OS.Build.VERSION.SdkInt >= 10) // Gingerbread 2.3.3
                    {
                        socket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);
                    }
                    else
                    {
                        socket = device.CreateRfcommSocketToServiceRecord(uuid);
                    }

                    if (socket == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Could not create socket");
                        return;
                    }

                    await socket.ConnectAsync();

                    if (socket.IsConnected)
                    {
                        System.Diagnostics.Debug.WriteLine("Connected to device");
                        Stream writer = socket.OutputStream;
                        byte[] array = Encoding.ASCII.GetBytes(colour);
                        writer.Write(array, 0, array.Length);
                        socket.Close();
                    }

                    Thread.Sleep(sleepTime);
                    if (!socket.IsConnected)
                    {
                        System.Diagnostics.Debug.WriteLine("Socket is not connected");
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                }
                finally
                {
                    if (socket != null)
                    {
                        socket.Close();
                    }

                    device = null;
                    adapter = null;
                }
            }
        }
    }
}