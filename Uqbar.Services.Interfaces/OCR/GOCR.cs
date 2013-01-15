using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.OCR
{
    public static class GOCR
    {
        private static readonly string EXECUTABLES_RELATIVEPATH = @"OCR\Executables\";

        public static string Resolve(MemoryStream bmp)
        {
            using (Image img = Image.FromStream(bmp))
            {
                string filename = "tmp.jpg";
                img.Save(EXECUTABLES_RELATIVEPATH + filename, ImageFormat.Jpeg);

                return ExecuteCommand("ocr " + filename);
            }

            return null;
        }

        private static string ExecuteCommand(string command)
        {
            string output = null;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, EXECUTABLES_RELATIVEPATH);

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C cd " + path + " && " + command;
            process.StartInfo = startInfo;
            
            process.Start();

            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        /*
        private static void ExecuteCommand2(string command)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
        */
    }
}
