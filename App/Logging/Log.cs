using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace App.Logging
{
	public class Log
	{
		private string path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Path.Combine(Directory.GetCurrentDirectory(), "Logs") : Path.Combine(AppContext.BaseDirectory, "Logs");

		private bool EnableDebug = true;

		private bool EnableInfor = true;

		private bool EnableWarning = true;

		private bool EnableError = true;

		public Log()
		{
		}

		public Log(string logPath, bool enableDebug = true, bool enableInfor = true, bool enableWarning = true, bool enableError = true)
		{
			path = logPath;
			EnableDebug = enableDebug;
			EnableInfor = enableInfor;
			EnableWarning = enableWarning;
			EnableError = enableError;
		}
		public Log(string logPath)
		{
			path = logPath;
		}


		public void Debug(string className, string content)
		{
			if (EnableDebug)
			{
				WriteLog("DEBUG", className, content);
			}
		}

		public void Info(string className, string content)
		{
			if (EnableInfor)
			{
				WriteLog("INFO", className, content);
			}
		}

		public void Warning(string className, string content)
		{
			if (EnableWarning)
			{
				WriteLog("Warning", className, content);
			}
		}

		public void Error(string className, string content)
		{
			if (EnableError)
			{
				WriteLog("ERROR", className, content);
			}
		}

		protected void WriteLog(string type, string className, string content)
		{
			string typePath = Path.Combine(path, type);
			if (!Directory.Exists(typePath))
			{
				Directory.CreateDirectory(typePath);
			}
			string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			StreamWriter streamWriter = File.AppendText(Path.Combine(typePath, DateTime.Now.ToString("yyyy-MM-dd") + ".log"));
			StringBuilder write_content = new StringBuilder();
			write_content.AppendLine();
			write_content.AppendLine("=================================================================");
			write_content.AppendLine("日期：" + time);
			write_content.AppendLine("类型：" + type);
			write_content.AppendLine("类名称：" + className);
			write_content.AppendLine("内容：");
			write_content.AppendLine(content);
			streamWriter.WriteLine(write_content);
			streamWriter.Close();
		}
	}
}
