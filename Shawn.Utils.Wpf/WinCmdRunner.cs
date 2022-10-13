using System;
using System.Collections.Generic;
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

        public static void RunFile(string filePath, string arguments = "", bool isAsync = false, bool isHideWindow = false)
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

        /// <summary>
        /// 解析单行命令中 exe 路径存在的空格或引号包裹的 "EXE 路径"，返回 EXE 路径 + 参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static Tuple<string, string> DisassembleOneLineScriptCmd(string cmd)
        {
            var parameters = "";
            cmd = cmd.Trim();
            var file = cmd;
            if (File.Exists(file))
            {
            }
            else if (cmd.StartsWith(@""""))
            {
                var i = cmd.IndexOf('"', 1);
                file = cmd.Substring(1, i - 1).Trim();
                parameters  = cmd.Substring(i + 1).Trim();
            }
            else if(cmd.IndexOf(" ", StringComparison.Ordinal) > 0)
            {
                var s = cmd.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var st = new Queue<string>(s);
                var filePath = st.Dequeue();
                while (st.Count > 0)
                {
                    if (File.Exists(filePath))
                    {
                        file = filePath;
                        parameters = string.Join(" ", st);
                        break;
                    }
                    filePath += " " + st.Dequeue();
                }
            }
            if (File.Exists(file))
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".py")
                {
                    parameters = file + " " + parameters;
                    file = "python";
                }
            }
            return new Tuple<string, string>(file, parameters);
        }

        public static Tuple<bool, string> CheckFileExistsAndFullName(string fileName)
        {
            // 判断是否有环境变量
            var splitByPercent = $"_{fileName}_".Split(new[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitByPercent.Length > 2)
            {
                for (int i = 1; i < splitByPercent.Length - 1; i++)
                {
                    var tmp = Environment.GetEnvironmentVariable(splitByPercent[i]);
                    if (string.IsNullOrEmpty(tmp) == false)
                    {
                        fileName = fileName.Replace($"%{splitByPercent[i]}%", tmp);
                    }
                }
            }


            if (Path.IsPathRooted(fileName))
            {
                return new Tuple<bool, string>(File.Exists(fileName), Path.GetFullPath(fileName));
            }

            if (File.Exists(fileName))
            {
                return new Tuple<bool, string>(true, Path.GetFullPath(fileName));
            }

            var file = Environment.ExpandEnvironmentVariables(fileName);
            if (Path.GetDirectoryName(file) == string.Empty)
            {
                var paths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(';');
                foreach (string test in paths)
                {
                    string path = Path.Combine(test.Trim(), file);
                    if (File.Exists(path))
                        return new Tuple<bool, string>(true, Path.GetFullPath(path));
                }
            }
            return new Tuple<bool, string>(false, Path.GetFullPath(fileName));
        }
    }
}