using System.Net;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace TCPCobotListener
{
    public class TCPClient
    {
        public static void Main()
        {
            string currentIp = GetLocalIPAddress();
            TcpListener listener = null;
            string folderPath = "C:/Users/lorenzoL/Documents/Microgate/CsvReadings";
            int fCount = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Length;
            var csv = new System.Text.StringBuilder();
                try
                {
                bool isConnected = true;
                listener = new TcpListener(System.Net.IPAddress.Parse(currentIp), 1337);
                listener.Start();
                Byte[] bytes = new Byte[256];
                String data = null;
                while (isConnected)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        File.AppendAllText("S:/Documentazione/COBOT/CvsFiles/ReadingsViaTcp/ArmReadingNumber" + fCount.ToString() + ".csv", data);
                    }
                    
                    // Shutdown and end connection
                    client.Close();
                    isConnected = false;
                }
                listener.Stop();
                //relaunch listener for new possible reading
                Main();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                 // Stop listening for new clients.
                listener.Stop();
            }
        }
        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}