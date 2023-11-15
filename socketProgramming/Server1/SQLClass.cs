using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Server1
{
    internal class SQLClass
    {
        // Register했을 때의 route(ID, NAME, PWD)
        public static string RegisterPostSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    conn.Open();
                    string InsertQuery = $"insert into user(id, name, pw) values('{jsonData.ID}', '{jsonData.NAME}', '{jsonData.PWD}')";
                    Console.Write("SQL insert start!");

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(InsertQuery, conn);

                    // 만약에 내가처리한 Mysql에 정상적으로 들어갔다면 메세지를 보여주라는 뜻
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.Write("Insert success!");
                        conn.Close();
                        return "success";
                        // 회원가입 완료됐다~
                    }
                    else
                    {
                        Console.Write("Insert error!");
                        conn.Close();
                        return "error";
                        // DB 오류났다~
                    }
                    
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }
        }

        // Login했을 때의 route(ID, PWD)
        public static string LoginPostSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    conn.Open();
                    string searchQuery = $"select * from user where id='{jsonData.ID}'";

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(searchQuery, conn);
                    MySqlDataReader DBresult = cmd.ExecuteReader();
                    if(DBresult.Read())
                    {
                        string DBpwd = DBresult["pw"].ToString(); //object to string
                        if(jsonData.PWD == DBpwd) // 비밀번호가 DB와 일치하면
                        {
                            Console.WriteLine("correct password!");
                            return DBresult["user_id"].ToString() ;
                        }
                        else
                        { // 비밀번호 불일치
                            Console.WriteLine("Incorrect password!");
                            return "incorrect";
                        }

                    }
                    else
                    {
                        Console.WriteLine("Unexist!");
                        return "unexist";
                        // 회원가입부터 해라~
                    }
                    conn.Close();

                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }
        }

        // Emotion List Get API
        public static string CalenderGetSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    conn.Open();
                    Console.WriteLine(jsonData);
                    int daysInMonth = DateTime.DaysInMonth(2023, Convert.ToInt32(jsonData.MONTH)); // 원하는 달에 해당하는 일 수를 가져옵니다.
                    string[] dataArray = new string[daysInMonth]; // 해당 일 수에 맞게 배열을 생성합니다.

                    string searchQuery = $"select emtion, date from log where user_id='{jsonData.USER_ID}' and month(date) = {jsonData.MONTH}";
                    
                    // command connection
                    MySqlCommand cmd = new MySqlCommand(searchQuery, conn);
                    MySqlDataReader DBresult = cmd.ExecuteReader();
                    while(DBresult.Read())
                    {
                        string emotion = DBresult["emtion"].ToString(); // 내용 컬럼 값
                        string date = DBresult["date"].ToString().Split(" ")[0]; // 날짜 컬럼 값
                        Console.WriteLine("date: "+ date);
                        int day = Convert.ToDateTime(date).Day; // 날짜에서 일(day) 값을 추출합니다.
                        Console.WriteLine("감정: "+ emotion);

                        dataArray[day - 1] = emotion; // 배열에 내용을 대입합니다. 날짜에서 1을 빼는 이유는 배열 인덱스가 0부터 시작하기 때문입니다.
                        Console.WriteLine("추출된 날짜: "+ day);
                        Console.WriteLine($"result: {DBresult["emtion"]} {DBresult["date"]}");
                    }
                    conn.Close();
                    string res = JsonConvert.SerializeObject(dataArray);
                    return res;
                    
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }

        }
       
        //Chat Room List Get API
        public static string GetChatRoomListSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    string Jsonresult = "";
                    conn.Open();
                    Console.WriteLine(jsonData);
                    List<List<string>> list = new List<List<string>>();

                    string searchQuery = $"select chatR_id, date, modification_date from chatRoom where user_id='{jsonData.USER_ID}'";

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(searchQuery, conn);
                    MySqlDataReader DBresult = cmd.ExecuteReader();
                    while (DBresult.Read())
                    {
                        Console.WriteLine($"result: {DBresult["chatR_id"]} {DBresult["date"]} {DBresult["modification_date"]}");
                        List<string> rowData = new List<string> { DBresult["chatR_id"].ToString(), DBresult["date"].ToString(), DBresult["modification_date"].ToString()};
                        list.Add(rowData);
                    }

                    // 리스트를 dateTime 값 기준으로 정렬
                    list.Sort((x, y) => DateTime.Compare(DateTime.Parse(y[1]), DateTime.Parse(x[1])));

                    Jsonresult = JsonConvert.SerializeObject(list) ;

                    Console.WriteLine(Jsonresult);
                    conn.Close();
                    return Jsonresult;
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }

        }

        // Chat Room Post API
        public static string PostChatRoomListSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    conn.Open();
                    Console.WriteLine(jsonData);
                    string InsertQuery = $"insert into chatRoom(user_id, date, modification_date) values('{jsonData.USER_ID}', now(), now())";
                    Console.Write("SQL insert start!");

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(InsertQuery, conn);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.Write("Insert success!");
                        conn.Close();
                        return "success";
                        // 회원가입 완료됐다~
                    }
                    else
                    {
                        Console.Write("Insert error!");
                        conn.Close();
                        return "error";
                        // DB 오류났다~
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }

        }

        //Get chatting List(with ChatBot)
        public static string GetChattingSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    string Jsonresult = "";
                    conn.Open();
                    Console.WriteLine(jsonData);
                    string searchQuery = $"select isuser, content, transit_time, emtion from chatting where chatR_id='{jsonData.CHATR_ID}'";
                    List<List<string>> list = new List<List<string>>();

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(searchQuery, conn);
                    MySqlDataReader DBresult = cmd.ExecuteReader();
                    while(DBresult.Read())
                    {
                        Console.WriteLine($"result: {DBresult["content"]}, {DBresult["transit_time"]}, {DBresult["isuser"]}, {DBresult["emtion"]}");
                        List<string> rowData = new List<string> { DBresult["content"].ToString(), DBresult["transit_time"].ToString(), DBresult["isuser"].ToString(), DBresult["emtion"].ToString()};
                        list.Add(rowData);
                    }
                    list.Sort((x, y) => DateTime.Compare(DateTime.Parse(x[1]), DateTime.Parse(y[1])));
                    Jsonresult = JsonConvert.SerializeObject(list);

                    Console.WriteLine(Jsonresult);
                    conn.Close();
                    return Jsonresult;
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }

        }

        // Post Chatting API
        public static string PostChattingSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    string sendChatID = "";
                    conn.Open();
                    Console.WriteLine(jsonData);
                    string InsertQuery = $"insert into chatting(chatR_id, isuser, content, emtion, transit_time) values('{jsonData.CHATR_ID}', {jsonData.ISUSER}, '{jsonData.CONTENT}', '{jsonData.EMOTION}', now())";
                    Console.Write("SQL insert start!");

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(InsertQuery, conn);
                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        Console.Write("Insert success!");
                        conn.Close();
                        return "success";
                        // 회원가입 완료됐다~
                    }
                    else
                    {
                        Console.Write("Insert error!");
                        conn.Close();
                        return "error";
                        // DB 오류났다~
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }

        }

        //Get emotion today and chatting
        public static string GetEmotionSQL(dynamic jsonData)
        {
            // RDS 서버에 접속
            string StringToConnection = "Server=nowmsm-db.cirkkpu5fv9s.us-east-1.rds.amazonaws.com;Database=nowMSM;Uid=admin;Pwd=00000000;";
            using (MySqlConnection conn = new MySqlConnection(StringToConnection))
            {
                Console.Write("success connection!");
                try
                {
                    string getEmotion = "";
                    conn.Open();
                    Console.WriteLine(jsonData);
                    string searchQuery = $"select emtion from log where user_id='{jsonData.USER_ID}' and date_format(date, '%Y-%m-%d') = date_format(now(), '%Y-%m-%d')";

                    // command connection
                    MySqlCommand cmd = new MySqlCommand(searchQuery, conn);
                    MySqlDataReader DBresult = cmd.ExecuteReader();
                    if (DBresult.Read())
                    {
                        Console.WriteLine($"result {DBresult["emtion"]}");
                        getEmotion = DBresult["emtion"].ToString();
                        conn.Close();
                        return getEmotion;
                    }
                    else
                    {
                        conn.Close();
                        return "not exist";
                        // DB 오류났다~
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.ToString());
                    return "";
                }
            }

        }
    }
}
