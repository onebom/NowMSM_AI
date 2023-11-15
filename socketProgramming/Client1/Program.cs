using Microsoft.Win32;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
// using Newtonsoft.Json;

namespace Client1
{
    internal class Program
    {
        //json structure
        public class RegisterObject
        {
            public string ROUTE { get; set; }
            public string ID { get; set; }
            public string NAME { get; set; }
            public string PWD { get; set; }
        }

        public class LoginObject
        {
            public string ROUTE { get; set; }
            public string ID { get; set; }
            public string PWD { get; set; }
        }
        static void Main()
        {
            // EC2 인스턴스의 IP 주소와 포트 번호
            // string ipAddress = "52.206.228.119";
            string ipAddress = "127.0.0.1";
            int port = 50000;
            TcpClient client = null;
            Console.WriteLine("Client Console.");
            try
            {
                RegisterObject data = new RegisterObject
                {
                    ROUTE = "Register",
                    ID = "seojin",
                    NAME = "seojin",
                    PWD = "seojin1234"
                };

                //LoginObject data = new LoginObject
                //{ 
                //    ROUTE = "Login",
                //    ID = "seojin",
                //    PWD = "seojin1234"
                //};
                string jsonString = JsonSerializer.Serialize(data); // object -> json
                Console.WriteLine(jsonString);

                // 서버에 연결
                client = new TcpClient(ipAddress, port);
                Console.WriteLine("서버에 연결되었습니다.");

                // 서버와 데이터 교환 stram 형성
                NetworkStream stream = client.GetStream();

                // 보낼 데이터 준비
                string message = jsonString;
                byte[] buffer = Encoding.UTF8.GetBytes(message);

                // 데이터 전송
                stream.Write(buffer, 0, buffer.Length);
                Console.WriteLine("데이터를 서버로 전송하였습니다.");

                // 서버로부터의 응답 데이터 받기
                buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("서버로부터의 응답 데이터: " + response);

            }
            catch (Exception e)
            {
                Console.WriteLine("오류: " + e.Message);
            }
            finally
            {
                client.Close();
            }

            Console.ReadLine();
        }
    }
}