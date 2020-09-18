using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EVoting.Server.Utils.ShellExecutors
{
    public class ShellHelper
    {
        private static string _PYTHON_SCRIPT = "C:\\Users\\acosm\\Desktop\\EVoting\\EVoting.Face\\face_identifier.py";


        public static async Task<ShellResult<bool>> VerifyImageAsync(string filePath, string userName)
        {
            var start = new ProcessStartInfo();
            start.FileName = "python";
            start.Arguments = String.Format("\"{0}\" \"{1}\" \"{2}\" \"verify\"", _PYTHON_SCRIPT, filePath, userName);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            // start.RedirectStandardError = true;
            string result;
            using (var process = Process.Start(start))
            using (StreamReader reader = process.StandardOutput)
            {

                // string stderr = process.StandardError.ReadToEnd(); 
                result = await reader.ReadToEndAsync();
            }

            result = result.Replace(System.Environment.NewLine, String.Empty);
            return JsonSerializer.Deserialize<ShellResult<bool>>(result);

        }

        public static async Task<ShellResult<bool>> RegisterImageAsync(string filePath, string userName)
        {
            var start = new ProcessStartInfo();
            start.FileName = "python";
            start.Arguments = String.Format("\"{0}\" \"{1}\" \"{2}\" \"register\"", _PYTHON_SCRIPT, filePath, userName);
            start.UseShellExecute = false;
            start.CreateNoWindow = true;
            start.RedirectStandardOutput = true;
            // start.RedirectStandardError = true;
            string result;
            using (var process = Process.Start(start))
            using (StreamReader reader = process.StandardOutput)
            {
                // var stderr = process.StandardError.ReadToEnd();
                result = await reader.ReadToEndAsync();
            }

            result = result.Replace(System.Environment.NewLine, String.Empty);
            return JsonSerializer.Deserialize<ShellResult<bool>>(result);
        }
    }

}
