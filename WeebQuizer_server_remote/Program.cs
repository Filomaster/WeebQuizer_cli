using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Pastel;
using SocketIOClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WeebQuizer_server_remote
{
    public class Request
    {
        public string command { get; set; }
        public string[] parameters { get; set; }
        public string content { get; set; }
    }

    public class Login
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Commands
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    class Program
    {
        static HttpClient http = new HttpClient();

        // async Task Login()
        // {
        //     Login login = new Login();

        //     screen.WriteLine("Pls login", horizontal_aligment.center);
        //     Console.Write("Username: ");
        //     login.username = Console.ReadLine();
        //     Console.Write("Password: ");
        //     login.password = Console.ReadLine();


        //     HttpResponseMessage res = await http.PostAsJsonAsync("/login", login);
        // }

        static async Task Main()
        {
            Screen_Manager screen = new Screen_Manager();

            http.BaseAddress = new Uri("http://localhost:5000/admin/");
            http.DefaultRequestHeaders.Accept.Clear();
            http.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            Login login = new Login();
            HttpResponseMessage log = new HttpResponseMessage();
            string login_redirect;
            do
            {
                screen.WriteLine("Pls login", horizontal_aligment.center);
                Console.Write("Username: ");
                login.username = Console.ReadLine();
                Console.Write("Password: ");
                login.password = Console.ReadLine();


                log = await http.PostAsJsonAsync("login", login);
                Console.WriteLine((int)log.StatusCode);



                login_redirect = log.RequestMessage.RequestUri.ToString().Replace(http.BaseAddress.ToString(), "");

            } while (login_redirect.Equals("login"));


            // var message = JsonConvert.DeserializeObject(await log.Content.ReadAsStringAsync());
            // Console.Write(message);




            //const string address = "http://localhost:5000/admin";
            //Console.BufferWidth = 256;
            //Console.BufferHeight = 256;
            //const int leap = 2;
            //for(int i = 0; i < 256; i+=leap)
            //{
            //    for (int j = 0; j < 256; j+=leap)
            //    {
            //        for (int k = 0; k < 256; k+=leap)
            //        {
            //            Color color = Color.FromArgb(i, j, k);
            //            Console.Write("#".Pastel(color));

            //        }
            //        Console.Write("\n");
            //    }
            //}
            //Console.Read();
            //screen.WriteLine("Test", horizontal_aligment.center);
            //screen.WriteLine("\u001b[31m Hello World! \u001b[0m");
            //screen.WriteLine("Test Pastel".Pastel("#223344"));

            //var client = new SocketIO(address);
            //client.On("message", response =>
            //{
            //    screen.WriteLine("Got message:");
            //    screen.WriteLine(response.GetValue<string>());
            //});
            //await client.ConnectAsync();

            string command = "";
            do
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                string[] in_arry = input.Split(" ", 2);
                Request req = new Request();

                command = in_arry[0].ToLower();

                switch (command)
                {
                    case "exit":
                        break;

                    case "cls":
                        screen.Clear();
                        break;
                    case "logout":
                        log = await http.DeleteAsync("logout");
                        break;
                    default:
                        screen.WriteAtLastLine("Sending...", horizontal_aligment.center);
                        req.command = command.ToLower();
                        if (in_arry.Length == 2)
                            req.content = in_arry[1];

                        HttpResponseMessage res = await http.PostAsJsonAsync(
                        "command", req);

                        // Console.WriteLine(res.StatusCode);

                        var status = res.StatusCode.ToString();
                        screen.WriteAtLastLine(status, horizontal_aligment.right);
                        //Console.WriteLine(res.Headers.GetValues("res-type").Contains("help"));
                        if (res.Headers.Contains("res-type") && res.Headers.GetValues("res-type").Contains("help"))
                        {
                            var response = await res.Content.ReadAsStringAsync();
                            List<Commands> commands = JsonConvert.DeserializeObject<List<Commands>>(response);
                            for (int i = 0; i < commands.Count; i++)
                            {
                                Console.WriteLine(" {0}: \t {1}", commands[i].name.ToUpper(), commands[i].description);
                            }
                        }
                        break;
                }



                //string res;

                //client.On("command", async response =>
                //{
                //    res = response.GetValue<string>();
                //    if (response.GetValue<string>().Equals("ok"))
                //    {
                //        if (in_arry.Length > 1)
                //            await client.EmitAsync(comand, in_arry[1]);
                //        else
                //            await client.EmitAsync(comand, "none");
                //    }
                //    else
                //    {
                //        screen.WriteLine("Unexpected Token");
                //    }
                //});



                //client.On("serv_res", response =>
                //{
                //    Console.WriteLine(response);
                //});
            } while (!command.ToLower().Equals("exit"));

            //Console.Read();

        }
    }
}
