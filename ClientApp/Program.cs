// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;
using System;
using System.Globalization;
using System.Threading;
using System.Text;

namespace ClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("press enter to continue....");
            Console.ReadLine();


            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri ServiceUri = new Uri("ws://localhost:5000/send");
                var cTs = new CancellationTokenSource();
                cTs.CancelAfter(TimeSpan.FromSeconds(120));

                try
                {
                    await client.ConnectAsync(ServiceUri, cTs.Token);
                    var numb = 0;
                    while (client.State == WebSocketState.Open)
                    {
                        Console.WriteLine("Enter msg to send");
                        string message = Console.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cTs.Token);
                            var responsBuffer = new byte[1024];
                            var offSet = 0;
                            var packet = 1024;
                            while (true)
                            {
                                ArraySegment<byte> bytesRecieved = new ArraySegment<byte>(responsBuffer, offSet, packet);
                                WebSocketReceiveResult response = await client.ReceiveAsync(bytesRecieved, cTs.Token);
                                var responseMsg = Encoding.UTF8.GetString(responsBuffer, offSet, response.Count);
                                Console.WriteLine(responseMsg);
                                if (response.EndOfMessage)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (WebSocketException ex)
                {

                    Console.WriteLine(ex.Message);
                }


            }

            Console.ReadLine();
        }

    }
}