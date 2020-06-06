using System;
using SocketIOClient;

namespace WeebQuizer_server_remote
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            
            Screen_Manager screen = new Screen_Manager();
            screen.WriteLine("Test", horizontal_aligment.center);
            Console.WriteLine("Hello World!");

            var client = new SocketIO("http://localhost:5000");
            client.On("message", response =>
            {
                screen.WriteLine("Got message:");
                screen.WriteLine(response.GetValue<string>());
            });
            await client.ConnectAsync();

            string comand = "";
            do
            {
                string input = Console.ReadLine();
                string[] in_arry = input.Split(" ", 2);
                string message = "";

                comand = in_arry[0];
                switch (comand)
                {
                    case "MSG":                       
                            screen.WriteLine("Sending...", horizontal_aligment.center);
                        await client.EmitAsync("msg", in_arry[1]);

                        
                        break;
                    case "EXIT":
                        break;
                    default:
                        screen.WriteLine("ERR: Unexpected token " + comand);
                        break;
                }
            } while (!comand.Equals("EXIT"));
          
        }
    }
}
