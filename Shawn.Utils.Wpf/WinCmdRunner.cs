using System;
using System.Diagnostics;
using System.IO;

namespace Shawn.Utils.Wpf
{
    public static class WinCmdRunner
    {
        /// <summary>
        /// cmd by cmd.exe
        /// </summary>
        /// <returns>[0] = output info，[1] = ret code</returns>
        public static string[] RunCmdSync(string cmd, bool isHideWindow = false)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = isHideWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                    UseShellExecute = !isHideWindow,
                    CreateNoWindow = isHideWindow,
                }
            };
            //pro.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pro.Start();
            pro.StandardInput.WriteLine(cmd);
            pro.StandardInput.WriteLine("---------------split_line---------------");// add a symble for exit code
            pro.StandardInput.WriteLine("exit");// add a symble for exit code
            pro.StandardInput.AutoFlush = true;
            var output = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            pro.Close();

            var content = output.Substring(output.IndexOf(cmd + "\r\n") + (cmd + "\r\n").Length);
            content = content.Substring(0, content.IndexOf("---------------split_line---------------\r\n"));
            content = content.Substring(0, content.LastIndexOf("\r\n")).Trim(new[] { '\r', '\n', ' ' });

            var retCode = output.Substring(output.LastIndexOf("---------------split_line---------------\r\n") + "---------------split_line---------------\r\n".Length);
            retCode = retCode.Substring(0, retCode.IndexOf("\r\n")).Trim(new[] { '\r', '\n', ' ' });
            return new[] { content, retCode };
        }

        /// <summary>
        /// cmd by cmd.exe
        /// </summary>
        public static void RunCmdAsync(string cmd, bool isHideWindow = false)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = isHideWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                    UseShellExecute = !isHideWindow,
                    CreateNoWindow = isHideWindow,
                }
            };
            pro.Start();
            pro.StandardInput.WriteLine(cmd);
            pro.StandardInput.WriteLine("---------------split_line---------------");// add a symble for exit code
            pro.StandardInput.WriteLine("exit");// add a symble for exit code
        }

        public static void RunExe(string filePath, string arguments = "", bool isAsync = false, bool isHideWindow = false)
        {
            var pro = new Process
            {
                StartInfo =
                {
                    FileName = filePath,
                    Arguments = arguments,
                    WindowStyle = isHideWindow ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
                    UseShellExecute = !isHideWindow,
                    CreateNoWindow = isHideWindow,
                }
            };

            pro.Start();
            if (isAsync == false)
            {
                pro.WaitForExit();
            }
        }


        public static void RunScriptFile(string filePath, bool isAsync = false, bool isHideWindow = false)
        {
            if (File.Exists(filePath) == false)
                throw new MissingFieldException($"{filePath} not found!");
            var ext = Path.GetExtension(filePath).ToLower();
            if (ext == ".py")
            {
                RunExe("python", filePath, isAsync: isAsync, isHideWindow: isHideWindow);
                return;
            }
            RunExe(filePath, isAsync: isAsync, isHideWindow: isHideWindow);
        }
    }
}