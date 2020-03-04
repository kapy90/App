using App.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace App.Infrastructure
{
	public class AppFileProvider : PhysicalFileProvider, IAppFileProvider, IFileProvider
	{
		protected string BaseDirectory
		{
			get;
		}

		public AppFileProvider(IHostEnvironment hostingEnvironment, string webRootPath)
			: base(File.Exists(webRootPath) ? Path.GetDirectoryName(webRootPath) : webRootPath)
		{
			string path = hostingEnvironment.ContentRootPath ?? string.Empty;
			if (File.Exists(path))
			{
				path = Path.GetDirectoryName(path);
			}
			BaseDirectory = path;
		}

		private static void DeleteDirectoryRecursive(string path)
		{
			Directory.Delete(path, recursive: true);
			int curIteration = 0;
			while (Directory.Exists(path))
			{
				curIteration++;
				if (curIteration > 10)
				{
					break;
				}
				Thread.Sleep(100);
			}
		}

		protected static bool IsUncPath(string path)
		{
			if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri))
			{
				return uri.IsUnc;
			}
			return false;
		}

		public virtual string Combine(params string[] paths)
		{
			string path = Path.Combine(paths.SelectMany((string p) => (IEnumerable<string>)(IsUncPath(p) ? ((object)new string[1]
			{
			p
			}) : ((object)p.Split('\\', '/')))).ToArray());
			if (Environment.OSVersion.Platform == PlatformID.Unix && !IsUncPath(path))
			{
				path = "/" + path;
			}
			return path;
		}

		public virtual void CreateDirectory(string path)
		{
			if (!DirectoryExists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public virtual void CreateFile(string path)
		{
			if (!FileExists(path))
			{
				FileInfo fileInfo = new FileInfo(path);
				CreateDirectory(fileInfo.DirectoryName);
				using (File.Create(path))
				{
				}
			}
		}

		public void DeleteDirectory(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(path);
			}
			string[] directories = Directory.GetDirectories(path);
			foreach (string directory in directories)
			{
				DeleteDirectory(directory);
			}
			try
			{
				DeleteDirectoryRecursive(path);
			}
			catch (IOException)
			{
				DeleteDirectoryRecursive(path);
			}
			catch (UnauthorizedAccessException)
			{
				DeleteDirectoryRecursive(path);
			}
		}

		public virtual void DeleteFile(string filePath)
		{
			if (FileExists(filePath))
			{
				File.Delete(filePath);
			}
		}

		public virtual bool DirectoryExists(string path)
		{
			return Directory.Exists(path);
		}

		public virtual void DirectoryMove(string sourceDirName, string destDirName)
		{
			Directory.Move(sourceDirName, destDirName);
		}

		public virtual IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern, bool topDirectoryOnly = true)
		{
			return Directory.EnumerateFiles(directoryPath, searchPattern, (!topDirectoryOnly) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public virtual void FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
		{
			File.Copy(sourceFileName, destFileName, overwrite);
		}

		public virtual bool FileExists(string filePath)
		{
			return File.Exists(filePath);
		}

		public virtual long FileLength(string path)
		{
			if (!FileExists(path))
			{
				return -1L;
			}
			return new FileInfo(path).Length;
		}

		public virtual void FileMove(string sourceFileName, string destFileName)
		{
			File.Move(sourceFileName, destFileName);
		}

		public virtual string GetAbsolutePath(params string[] paths)
		{
			List<string> allPaths = new List<string>
		{
			base.Root
		};
			allPaths.AddRange(paths);
			return Combine(allPaths.ToArray());
		}

		public virtual DateTime GetCreationTime(string path)
		{
			return File.GetCreationTime(path);
		}

		public virtual string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true)
		{
			if (string.IsNullOrEmpty(searchPattern))
			{
				searchPattern = "*";
			}
			return Directory.GetDirectories(path, searchPattern, (!topDirectoryOnly) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public virtual string GetDirectoryName(string path)
		{
			return Path.GetDirectoryName(path);
		}

		public virtual string GetDirectoryNameOnly(string path)
		{
			return new DirectoryInfo(path).Name;
		}

		public virtual string GetFileExtension(string filePath)
		{
			return Path.GetExtension(filePath);
		}

		public virtual string GetFileName(string path)
		{
			return Path.GetFileName(path);
		}

		public virtual string GetFileNameWithoutExtension(string filePath)
		{
			return Path.GetFileNameWithoutExtension(filePath);
		}

		public virtual string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true)
		{
			if (string.IsNullOrEmpty(searchPattern))
			{
				searchPattern = "*.*";
			}
			return Directory.GetFiles(directoryPath, searchPattern, (!topDirectoryOnly) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
		}

		public virtual DateTime GetLastAccessTime(string path)
		{
			return File.GetLastAccessTime(path);
		}

		public virtual DateTime GetLastWriteTime(string path)
		{
			return File.GetLastWriteTime(path);
		}

		public virtual DateTime GetLastWriteTimeUtc(string path)
		{
			return File.GetLastWriteTimeUtc(path);
		}

		public virtual string GetParentDirectory(string directoryPath)
		{
			return Directory.GetParent(directoryPath).FullName;
		}

		public virtual string GetVirtualPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			if (!IsDirectory(path) && FileExists(path))
			{
				path = new FileInfo(path).DirectoryName;
			}
			path = path?.Replace(base.Root, "").Replace('\\', '/').Trim('/')
				.TrimStart('~', '/');
			return "~/" + path;
		}

		public virtual bool IsDirectory(string path)
		{
			return DirectoryExists(path);
		}

		public virtual string MapPath(string path)
		{
			path = path.Replace("~/", string.Empty).TrimStart('/');
			string pathEnd = path.EndsWith('/') ? Path.DirectorySeparatorChar.ToString() : string.Empty;
			return Combine(BaseDirectory ?? string.Empty, path) + pathEnd;
		}

		public virtual byte[] ReadAllBytes(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return new byte[0];
			}
			return File.ReadAllBytes(filePath);
		}

		public virtual string ReadAllText(string path, Encoding encoding)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(fileStream, encoding))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		public virtual void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
		}

		public virtual void WriteAllBytes(string filePath, byte[] bytes)
		{
			File.WriteAllBytes(filePath, bytes);
		}

		public virtual void WriteAllText(string path, string contents, Encoding encoding)
		{
			File.WriteAllText(path, contents, encoding);
		}
	}
}
