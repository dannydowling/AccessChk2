
using System.Diagnostics;

public class RunAccessChk
{
    public string[] unfilteredData { get; set; }


    public RunAccessChk()
    {
        using (Process process = new Process())
        {
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = @"C:\";
            process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

            // Redirects the standard input so that commands can be sent to the shell.
            process.StartInfo.RedirectStandardInput = true;
            // Runs the specified command and exits the shell immediately.
            //process.StartInfo.Arguments = @"/c ""dir""";

            process.OutputDataReceived += ProcessOutputDataHandler;
            process.ErrorDataReceived += ProcessErrorDataHandler;

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            foreach (var item in System.Diagnostics.Process.GetProcesses())
            {
                process.StandardInput.WriteLine("AccessChk -p -f {0}", item.Id);
            }

            // we've gone through each process and populated the unfilteredData collection
            foreach (var @string in unfilteredData)
            {
                // get the substring of the flags value, convert it to hex
                int flagInteger;

                // get the enum values...
                IEnumerable<FlagsEnum> someEnums = Enum
                  .GetValues(typeof(FlagsEnum))
                  .Cast<FlagsEnum>();


                // this is an issue, we need to change this to cast twice to and from enums.
                // once to get the enums into integers, and then do a manual reduction of the flags value.
                // after that we should have a set of integers that add up to make the original flag.
                // we cast those back into enums and then .tostring() out to the console and to the 
                // thing that checks for permission discrepancies.

                if (flagInteger != someEnums.First())
                {
                   flagInteger -= (x => x <= someEnums.Where(y => y.CompareTo(x) == -1).First));
                }
            }

            process.StandardInput.WriteLine("exit");

            process.WaitForExit();
        }
    }

    enum FlagsEnum
    {
        20, 
        8, 
        4, 
        3, 
        1,
        0
    }

    public void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        unfilteredData.Append(outLine.Data);
        Console.WriteLine(outLine.Data);
    }

    public void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
    {
        Console.WriteLine(outLine.Data);
    }
}
