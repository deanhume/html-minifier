using System;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "No resources to dispose of")]
public class ConsoleColour : IDisposable
{
    ConsoleColor c;
    static ConsoleColor staticColour = Console.ForegroundColor; // colour when process started

    public void ConsolecColour()
    {
        c = Console.ForegroundColor; // Remember the current colour
    }

    static ConsoleColour()
    {
        AppDomain thisAppDomain = AppDomain.CurrentDomain;
        thisAppDomain.DomainUnload += new EventHandler(DomainUnload);
        thisAppDomain.UnhandledException += new UnhandledExceptionEventHandler(thisAppDomain_UnhandledException);
        Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);
    }

    static void thisAppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        StaticDispose();
    }

    static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        StaticDispose();
    }

    static void DomainUnload(object sender, EventArgs e)
    {
        StaticDispose();
    }

    static void StaticDispose()
    {
        Console.ResetColor(); // new to .Net 2.0
    }

    public ConsoleColour(ConsoleColor c)
    {
        // set a new colour now
        Console.ForegroundColor = c;
    }

    #region IDisposable Members
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "No resources to dispose of")]
    public void Dispose()
    {
        // put it back
        Console.ResetColor();
    }
    #endregion
}//class ConsoleColour
