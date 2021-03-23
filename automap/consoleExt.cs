using System;
using System.Collections.Generic;
using System.Text;

static class ConsoleExt
{
    public static void Write(this ConsoleColor c,object str)
    {
        Console.ForegroundColor = c;
        Console.Write(str);
    }
    public static void WriteLine(this ConsoleColor c, object str)
    {
        Console.ForegroundColor = c;
        Console.WriteLine(str);
    }
    public static void WriteLine(this ConsoleColor c, string str,params object[] p)
    {
        Console.ForegroundColor = c;
        Console.WriteLine(str,p);
    }
    public static string Read(this ConsoleColor c)
    {
        Console.ForegroundColor = c;
        return Console.ReadLine();
    }
}
