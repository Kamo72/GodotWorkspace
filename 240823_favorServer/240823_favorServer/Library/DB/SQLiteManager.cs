using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _240823_favorServer.Library.DB
{
    public class SQLiteManager
    {
        const string mainDbRoot = "mainDb.db";

        static SqliteConnection conn;
        static SQLiteManager() 
        {
            conn = new SqliteConnection("Data Source=" + mainDbRoot);
            conn.Open();
            InitiateDB();
        }

        static void InitiateDB()
        {
            // 테이블 생성
            var createTableCmd = conn.CreateCommand();
            createTableCmd.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id TEXT PRIMARY KEY NOT NULL,
                    Name TEXT NOT NULL,
                    Pw TEXT NOT NULL
                );
            ";
            createTableCmd.ExecuteNonQuery();

        }

        public static void AddUser(string id, string name, string pw)
        { // 데이터 삽입
            var cmd = conn.CreateCommand();
            cmd.CommandText =
            $@"
                INSERT INTO Users (Id, Name, Pw) 
                VALUES ('{id}', '{name}', '{pw}');
            ";
            cmd.ExecuteNonQuery();
        }

        public static (string id, string name, string pw)? GetUserByIdAndPw(string id, string pw)
        {
            try
            {
                // 데이터 삽입
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Name 
                    FROM Users 
                    WHERE Id = @Id AND Pw = @Pw;
                ";
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Pw", pw);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            object nameObj = reader["Name"];
                            if (nameObj == null) return null;

                            string name = nameObj.ToString();

                            return (id, name, pw);
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

        public static (string id, string name, string pw)? GetUser(string id)
        {
            try
            {
                // 데이터 삽입
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Name, Pw 
                    FROM Users 
                    WHERE Id = @Id;
                ";
                cmd.Parameters.AddWithValue("@Id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader["Name"]?.ToString();
                        string pw = reader["Pw"]?.ToString();

                        if (name == null || pw == null) return null;

                        return (id, name, pw);
                    }
                }
            }
            catch (SqliteException ex)
            {
                Console.WriteLine("SQLite exception: " + ex.Message);
            }
            catch (Exception ex) // 다른 예외를 처리
            {
                Console.WriteLine("General exception: " + ex.Message);
            }

        return null;
        }

        public static List<(string id, string name, string pw)> GetUserAll() 
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText =
            $@"
                SELECT Id, Name, Pw 
                FROM Users;
            ";

            List<(string id, string name, string pw)> userList = new List<(string id, string name, string pw)>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetString(0);
                    var name = reader.GetString(1);
                    var pw = reader.GetString(2);
                    userList.Add((id, name, pw));
                }
            }
            return userList;

        }

    }
}
