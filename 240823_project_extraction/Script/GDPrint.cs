using System;
using Godot;

public static class GDPrint
{
    public static string GetTimestamp()
    {
        return DateTime.Now.ToString("dd.MM.yyyy-HH:mm:ss");
    }

    public static void Print(string message)
    {
        GD.Print($"{GetTimestamp()}: {message}");
    }

    public static void Print(int id, string message)
    {
        GD.Print($"{GetTimestamp()} - <{id}>: {message}");
    }

    public static void PrintErr(string message)
    {
        GD.PrintErr($"{GetTimestamp()}: {message}");
    }

    public static void PrintErr(int id, string message)
    {
        GD.PrintErr($"{GetTimestamp()} - <{id}>: {message}");
    }
}