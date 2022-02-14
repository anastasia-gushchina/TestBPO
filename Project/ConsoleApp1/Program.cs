using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.DirectoryServices;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;



namespace ConsoleApp1
{
    
   public class Server
    {
        TcpListener listener;
        public Server(int port) {
            
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            while (true)
            {
                work(listener.AcceptTcpClient());
            }
        }
        private string getType(string url)
        {
            string extention = url.Substring(url.LastIndexOf('.'));
            switch (extention)
            {
                case ".css":
                    return"text/css";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".js":
                    return "text/javascript";
            };
            return "";
        }
        private void work(TcpClient client)
        {
            
            byte[] input = new byte[1024];
            string request="";
            int count;
            string response = "";
            string file = "";
            string contentType = "";

            while ((count = client.GetStream().Read(input, 0, input.Length)) > 0) {
                request += Encoding.UTF8.GetString(input,0,count);
                if (request.Contains("\r\n\r\n")) break;
            }
            if (request == "")
            {
                response = "HTTP/1.1 500 Error ";
                client.GetStream().Write(Encoding.UTF8.GetBytes(response), 0, Encoding.UTF8.GetBytes(response).Length);
                client.Close();
                return;
            }
            
            string url = (request.Split('\r','\n')[0]).Split(' ')[1];
            Console.WriteLine(url);

            if (url == "/") url = "/index.html";
            try
            {
                if (url.Contains("userName"))
                {
                    bool hasUser = checkUser(url.Substring(url.IndexOf("=") + 1));
                    response = "HTTP/1.1 200 OK \nContent-type: text/\nContent-Length: " + Encoding.UTF8.GetBytes(hasUser.ToString()).Length + "\n\n" + hasUser.ToString();
                }
                else
                {
                    file = File.ReadAllText("../../../.." + url);
                    contentType = getType(url);
                    response = "HTTP/1.1 200 OK\nContent-type: " + contentType + "\nContent-Length:" + Encoding.UTF8.GetBytes(file).Length.ToString() + "\n\n" + file;

                }
            }
            catch (FileNotFoundException)
            {
                response = "HTTP/1.1 404 NotFound";
            }
            finally
            {
                client.GetStream().Write(Encoding.UTF8.GetBytes(response), 0, Encoding.UTF8.GetBytes(response).Length);
                client.Close();
            }
        }
        ~Server()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }
        private bool checkUser(string name)
        {
            name = Uri.UnescapeDataString(name);
            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName);
            foreach (DirectoryEntry child in localMachine.Children)
            {
                if (child.SchemaClassName == "User")
                {
                    if (child.Name == name)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
     class Program
    {
         
        static void Main(string[] args)
        {
            try
            {
                new Server(3000);
            }
            catch (SocketException)
            {
                Console.Write("Введите номер порта:");
                int port;
                while (true)
                {
                    try
                    {
                        port = int.Parse(Console.ReadLine());
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Введите номер порта:");
                    }
                }
                new Server(port);
            }

        }
        
    }
}
