#r "System"
#r "System.Core"
#r "System.Xml"
#r "System.Xml.Linq"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;


/** 対象とするAzure P2S VPNの名前 */
string TargetVPNName = "[Your VPN Connect Name]";


var ret = Main();

return ret;




void TraceOut(string msg)
{
    Console.Out.WriteLine(msg);
}

void TraceError(string msg)
{
    Console.Error.Write(msg);
}

string m_AppFolder;


int Main()
{
    try
    {

        m_AppFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        MainFunc();

    }
    catch(Exception ex)
    {
        TraceOut($"MainFunc Fail, {DateTime.Now.ToString()}, {ex.ToString()}");
        return 11;
    }

    return 0;
}


void MainFunc()
{
    TraceOut("MainFunc Start, " + DateTime.Now.ToString());




    TraceOut("Connect Start");
    //一般的にはrasphone -dを使用するようだが、なぜかAzureではその方法だと接続ボタンを押した後にエラーになる。Windowsの動作をプロセスモニタで見たところrasautouを使っていたので、それを真似る。
    var pbkFilePath = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Network\Connections\Pbk\rasphone.pbk");

    //64bitOSで32bitのrasautouを呼び出すと正常動作しないという現象があるので、64bitOSでは64bitのパスを呼ぶようにする
    string rasExePath;
    if(Environment.Is64BitOperatingSystem){
        rasExePath = Environment.ExpandEnvironmentVariables(@"%WinDir%\sysnative\rasautou.exe");
    }
    else{
        rasExePath = Environment.ExpandEnvironmentVariables(@"%WinDir%\System32\rasautou.exe");
    }
    


    //Azureは困ったことにここでサイレントで接続が行われず、接続ボタンを押す必要がある。という事でUWSCで接続ボタンを押せるように探索を開始しておく。
    var procinfo = new ProcessStartInfo("UWSC.exe", $"AzureVM接続ボタンが見つかるまで待って1回だけ押す_{TargetVPNName}.UWS");
    procinfo.WorkingDirectory = m_AppFolder;

    using (var UWSCProc = Process.Start(procinfo))
    {

        ProcStartAndEndWait_WithConsoleRedirect(rasExePath, "-o -f \"" + pbkFilePath + "\" -e " + TargetVPNName, 60 * 1000);

        UWSCProc.WaitForExit();
    }

    TraceOut("Connect End");




    TraceOut("MainFunc Success, " + DateTime.Now.ToString());

}



void ProcStartAndEndWait_WithConsoleRedirect(string ProcName, string ProcArg, int WaitTimeoutMS)
{
    TraceOut($"ProcStartAndEndWait_WithConsoleRedirect, {ProcName}, {ProcArg}, {WaitTimeoutMS}");

    var procInfo = new ProcessStartInfo(ProcName, ProcArg);
    procInfo.UseShellExecute = false;
    procInfo.RedirectStandardOutput = true;
    procInfo.RedirectStandardError = true;

    using (var proc = new Process())
    {
        proc.StartInfo = procInfo;
        proc.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        proc.ErrorDataReceived += (sender, e) => Console.Error.WriteLine(e.Data);

        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        proc.WaitForExit(WaitTimeoutMS);
    }
}

