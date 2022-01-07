﻿using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Andtech.DPM
{

	public interface IInstallLocation
	{

		string Destination { get; }
	}

	public class Include
	{
		public string path { get; set; }
		public string destination { get; set; }

		public string GetDestinationPath()
		{
			string fileName;
			if (!string.IsNullOrEmpty(destination))
			{
				fileName = destination;
				if (PathUtility.IsDirectory(destination))
				{
					fileName = Path.Combine(fileName, path);
				}
			}
			else
			{
				fileName = path;
			}

			return fileName;
		}
	}

	public class InstallLocation : IInstallLocation
	{
		public string destination { get; set; }

		string IInstallLocation.Destination => destination;
	}

	internal class Package : IInstallLocation
	{
		public List<Include> include { get; set; }
		public List<string> exclude { get; set; }
		public string destination;
		public string load_script;
		public string save_script;
		public InstallLocation windows;
		public InstallLocation macos;
		public InstallLocation linux;
		public InstallLocation wsl;
		public string Root => System.IO.Path.GetDirectoryName(Path);
		public string Path { get; set; }

		string IInstallLocation.Destination => destination;

		public IInstallLocation GetInstallLocation(Platform platform)
		{
			switch (platform)
			{
				case Platform.windows:
				case Platform.wsl:
					if (windows != null)
					{
						return windows;
					}
					break;
				case Platform.macos:
					if (macos != null)
					{
						return macos;
					}
					break;
				case Platform.linux:
					if (linux != null)
					{
						return linux;
					}
					break;
			}

			return this;
		}

		public static Package Read(string path)
		{
			var filename = System.IO.Path.GetFileName(path);
			if (!(filename == "package.yaml"))
			{
				path = System.IO.Path.Combine(path, "package.yaml");
			}

			var deserializer = new DeserializerBuilder()
				.Build();

			var package = deserializer.Deserialize<Package>(new StreamReader(path));
			package.Path = System.IO.Path.GetFullPath(path);

			return package;
		}
	}
}
