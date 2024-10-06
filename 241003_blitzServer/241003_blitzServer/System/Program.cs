using System;
using System.Collections;
using _231018_WBNET;
using Microsoft.Data.Sqlite;


namespace _241003_blitzServer.System
{
    public class Program()
    {
        static MainServer ms;

        static void Main()
        {
            ms = MainServer.GetInstance();
            Console.ReadLine();
        }

    }

}




