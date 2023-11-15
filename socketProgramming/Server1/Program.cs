using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Collections.Specialized;

using Server1;
using System.IO;
using System.Text.RegularExpressions;

namespace Server1
{
    internal class Program
    {
        static void Main()
        {
            // 서버의 IP 주소와 포트 번호
            string ipAddress = "127.0.0.1";
            int port = 50000;
            // 이미지 저장 경로
            string imagePath = "image.jpg";
            string saveFilePath = "log/image.jpg";

            try
            {
                // 서버 소켓 생성
                TcpListener server = new TcpListener(IPAddress.Any, port);

                // 클라이언트 연결 대기
                server.Start();
                Console.WriteLine("server start... waiting Client");

                while (true)
                {
                    // 클라이언트 연결 수락
                    TcpClient AcceptedClient = server.AcceptTcpClient();
                    Console.WriteLine("Connection to client!");
                    // 새로운 클라이언트가 연결되면 쓰레드 생성 및 시작
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(AcceptedClient);
                }


                void HandleClient(object clientObj)
                {
                    TcpClient client = (TcpClient)clientObj;
                    
                    // 클라이언트와의 데이터 통신 처리
                    NetworkStream stream = client.GetStream();

                    const int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    //// 파일 저장 경로 및 파일명 설정
                    //string filePath = "test.jpg";

                    //// 파일 스트림 생성
                    //using (FileStream fileStream = File.OpenWrite(filePath))
                    //{
                    //    // 클라이언트로부터 데이터 수신 및 파일에 저장
                    //    while ((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0)
                    //    {
                    //        fileStream.Write(buffer, 0, bytesRead);
                    //    }
                    //}

                    // 헤더를 읽음
                    //string header = ReadHeader(stream);
                    //int imageLength = ExtractImageLength(header); // 이미지 데이터 길이 추출
                    //Console.WriteLine(imageLength);
                    //byte[] imageBuffer = new byte[imageLength]; // 이미지 데이터를 저장할 버퍼
                    //int totalBytesReceived = 0; // 수신된 전체 바이트 수


                    //// 이미지 데이터를 읽음
                    //while (totalBytesReceived < imageLength)
                    //{
                    //    int bytesRead = stream.Read(imageBuffer, totalBytesReceived, imageLength - totalBytesReceived);
                    //    Console.WriteLine(bytesRead);
                    //    totalBytesReceived += bytesRead;
                    //}

                    //// 이미지를 파일로 저장 (예시: image.jpg)
                    //File.WriteAllBytes("image.jpg", imageBuffer);

                    //// 헤더를 읽는 메서드
                    //string ReadHeader(NetworkStream stream)
                    //{
                    //    byte[] headerBuffer = new byte[1024]; // 헤더를 저장할 버퍼
                    //    int bytesRead = stream.Read(headerBuffer, 0, headerBuffer.Length);
                    //    Console.WriteLine("bytesRead: " + bytesRead);
                    //    string header = Encoding.ASCII.GetString(headerBuffer, 0, bytesRead);
                    //    Console.WriteLine("header: "+ header);
                    //    return header;
                    //}

                    //// 이미지 데이터 길이 추출 메서드
                    //int ExtractImageLength(string header)
                    //{
                    //    string pattern = @"totalLen:\s*'(\d+)'";
                    //    Match match = Regex.Match(header, pattern);
                    //    if (match.Success)
                    //    {
                    //        Console.WriteLine("match!!!!!!!!!!!!");
                    //        string valueStr = match.Groups[1].Value;
                    //        int value = int.Parse(valueStr);
                    //        Console.WriteLine("value: " + value);
                    //        return value;
                    //    }
                    //    throw new InvalidOperationException("No image Length!!!!!!!!!!!!!!!!");
                    //}

                    //using (MemoryStream memoryStream = new MemoryStream())
                    //{
                    //    Console.WriteLine("memory streaming");
                    //    int byteRead;
                    //    int cnt = 0;
                    //if ((byteRead = stream.Read(buffer, 0, buffer.Length) )> 0)
                    //{

                    //    Console.WriteLine(buffer.Length);
                    //    Console.WriteLine(byteRead);
                    //    cnt++;
                    //    memoryStream.Write(buffer, 0, byteRead);
                    //    Console.WriteLine(memoryStream.ToArray());
                    //}

                    //    // 이미지 저장
                    //    File.WriteAllBytes("image"+cnt.ToString()+".png", memoryStream.ToArray());
                    //}
                    //Console.WriteLine("image store success!");
                    //TcpClient client = (TcpClient)clientObj;

                    //// 클라이언트와의 데이터 통신 처리
                    //NetworkStream stream = client.GetStream();
                    //byte[] buffer = new byte[1036];
                    //int bytesRead = 0;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // 수신한 데이터 처리
                        string res ="";
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine(data);
                        dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                        if (jsonObject.ROUTE == "Login") // Login이면
                        {
                            res = LoginHandler(jsonObject);
                        }
                        else if (jsonObject.ROUTE == "Register") // Register이면
                        {
                            res = RegisterHandler(jsonObject);
                        }
                        else if (jsonObject.ROUTE == "Calender")
                        {
                            Console.WriteLine("calender route!");
                            res = CalenderHandler(jsonObject);
                            Console.WriteLine(res);
                        }
                        else if (jsonObject.ROUTE == "GetChatRoomList")
                        {
                            Console.WriteLine("chatRoom Get route!");
                            res = GetChatRoomListHandler(jsonObject);
                        }
                        else if (jsonObject.ROUTE == "PostChatRoomList")
                        {
                            Console.WriteLine("chatRoom Post route!");
                            res = PostChatRoomListHandler(jsonObject);
                        }
                        else if (jsonObject.ROUTE == "PostChatting")
                        {
                            Console.WriteLine("post chatting route!");
                            res = PostChattingHandler(jsonObject);
                            Console.WriteLine(res);
                        }
                        else if (jsonObject.ROUTE == "GetChatting")
                        {
                            Console.WriteLine("get chatting route!");
                            res = GetChattingHandler(jsonObject);
                            Console.WriteLine(res);
                        }
                        else if(jsonObject.ROUTE == "GetEmotion")
                        {
                            Console.WriteLine("get emotion route!");
                            res = GetEmotionHandler(jsonObject);
                            Console.WriteLine(res);
                        }
                        else
                        {
                            Console.WriteLine("else");
                        }

                        // 클라이언트에게 응답 전송
                        byte[] response = Encoding.UTF8.GetBytes(res);
                        stream.Write(response, 0, response.Length);
                    }

                    // 클라이언트와의 연결 종료
                    Console.WriteLine("client " + client.Client.RemoteEndPoint + "end");
                    client.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("오류: " + e.Message);
            }
            Console.ReadLine();
        }

        // Login packet
        public static string LoginHandler(dynamic jsonData)
        {
            Console.WriteLine("Login Handler!");
            Console.WriteLine(jsonData);
            return SQLClass.LoginPostSQL(jsonData);
        }

        // Register packet
        public static string RegisterHandler(dynamic jsonData)
        {
            Console.WriteLine("Register Handler!");
            Console.WriteLine(jsonData);
            return  SQLClass.RegisterPostSQL(jsonData);
        }
        
        // Get Emotion(by clicked Calender)
        public static string CalenderHandler(dynamic jsonData)
        {
            return SQLClass.CalenderGetSQL(jsonData);
        }

        //Get ChatRoomList(by clicked talk button)
        public static string GetChatRoomListHandler(dynamic jsonData)
        {
             return SQLClass.GetChatRoomListSQL(jsonData);
        }
        //Post ChatRoomList(by clicked add room button)
        public static string PostChatRoomListHandler(dynamic jsonData)
        {
            return SQLClass.PostChatRoomListSQL(jsonData);
        }

        // Get Chatting(by clicked chatRoom Button)
        public static string GetChattingHandler(dynamic jsonData)
        {
            return SQLClass.GetChattingSQL(jsonData);
        }

        // Post Chatting(by clicked send Button)
        public static string PostChattingHandler(dynamic jsonData)
        {
            return SQLClass.PostChattingSQL(jsonData);
        }

        // Get Emotion(today's emotion)
        public static string GetEmotionHandler(dynamic jsonData)
        {
            return SQLClass.GetEmotionSQL(jsonData);
        }
    }
}