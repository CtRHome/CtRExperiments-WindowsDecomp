
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

bool hasHandledException = false;

void OnCatchException(Exception ex)
{
	if (hasHandledException)
		return;
	try
	{
		try
		{
			File.WriteAllText("crashlog.txt", ex.ToString());
		}
		finally
		{
			MessageBox.Show(ex.ToString(), "Err...Game Crashed", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	catch
	{
	}
	hasHandledException = true;
}

try
{
    ComWrappers.RegisterForMarshalling(WinFormsComInterop.WinFormsComWrappers.Instance); // for NativeAOT
    // In NativeAOT, the form icon is always default
    
    AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
    {
        OnCatchException((Exception)e.ExceptionObject);
    };
    Application.ThreadException += (sender, e) =>
    {
        OnCatchException(e.Exception);
    };
    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

    using var game = new CutTheRope.Game1();
    game.Run();
}
catch (Exception ex) when(!Debugger.IsAttached)
{
    OnCatchException(ex);
    throw;
}
