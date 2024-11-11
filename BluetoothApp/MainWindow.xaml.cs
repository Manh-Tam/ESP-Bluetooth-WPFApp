using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;

namespace BluetoothApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BluetoothClient client = new BluetoothClient();
                var devices = client.DiscoverDevices();

                txtOutput.AppendText("Scanning for devices...\n");

                foreach (var device in devices)
                {
                    txtOutput.AppendText($"Found device: {device.DeviceName} [{device.DeviceAddress}]\n");

                    if (device.DeviceName == "ESP32_Test") // Tên thiết bị Bluetooth của ESP32
                    {
                        txtOutput.AppendText("Connecting to ESP32...\n");
                        client.Connect(device.DeviceAddress, BluetoothService.SerialPort);
                        txtOutput.AppendText("Connected to ESP32!\n");

                        var stream = client.GetStream();

                        // Gửi dữ liệu tới ESP32
                        byte[] sendBuffer = Encoding.ASCII.GetBytes("Hello from WPF!\n");
                        stream.Write(sendBuffer, 0, sendBuffer.Length);

                        // Đọc dữ liệu từ ESP32
                        byte[] readBuffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                        string receivedData = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
                        txtOutput.AppendText($"Received from ESP32: {receivedData}\n");

                        stream.Close();
                        client.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                txtOutput.AppendText($"Error: {ex.Message}\n");
            }
        }
    }
}
