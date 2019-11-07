using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace SerializationFail.ViewModels
{
	public class SerializationPregenerationViewModel<T> : NotifyPropertyChanged where T : class, new()
	{
		private bool _testHasRun = false;

		private void DoTest()
		{
			Task.Run(() =>
			{
				try
				{
					// Create the serializer, if we've pregenerated it, then we shouldn't get
					// an internally caught exception here.
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					MemoryStream stream = new MemoryStream();

					// Create the test data
					T testWrite = new T();

					// Write out the test data
					serializer.Serialize(stream, testWrite);
					WriteStatus = true;

					FileContents = Encoding.ASCII.GetString(stream.ToArray());

					// Deserialize the test data
					stream.Seek(0, SeekOrigin.Begin);
					var testRead = serializer.Deserialize(stream) as T;
					ReadStatus = testRead.Equals(testWrite);

					ErrorStatus = "No Error";
				}
				catch (Exception ex)
				{
					ErrorStatus = ex.Message;
				}
				RaisePropertyChanged(nameof(AllTypes));
			});
		}

		private static string GenerateAssemblyId(Type type)
		{
			Module[] modules = type.Assembly.GetModules();
			List<object> moduleVersionIdList = new List<object>();
			for (int index = 0; index < modules.Length; ++index)
				moduleVersionIdList.Add((object)modules[index].ModuleVersionId.ToString());
			moduleVersionIdList.Sort();
			StringBuilder stringBuilder = new StringBuilder();
			for (int index = 0; index < moduleVersionIdList.Count; ++index)
			{
				stringBuilder.Append(moduleVersionIdList[index].ToString());
				stringBuilder.Append(",");
			}
			return stringBuilder.ToString();
		}

		private string GenerateSerializerParentAssemblyId(Type type)
		{
			var path = GetXmlSerializersPath(type);
			if (File.Exists(path))
			{
				var assembly = Assembly.LoadFile(path);
				object[] customAttributes = assembly.GetCustomAttributes(typeof(XmlSerializerVersionAttribute), false);
				XmlSerializerVersionAttribute versionAttribute = (XmlSerializerVersionAttribute)customAttributes[0];
				return versionAttribute.ParentAssemblyId;
			}
			else
			{
				return "";
			}
		}


		private string GenerateVersionAttributeNamespace(Type type)
		{
			var path = GetXmlSerializersPath(type);
			if (File.Exists(path))
			{
				var assembly = Assembly.LoadFile(path);
				object[] customAttributes = assembly.GetCustomAttributes(typeof(XmlSerializerVersionAttribute), false);
				XmlSerializerVersionAttribute versionAttribute = (XmlSerializerVersionAttribute)customAttributes[0];
				return versionAttribute.Namespace;
			}
			else
			{
				return null;
			}
		}

		private string GetXmlSerializersPath(Type type)
		{
			var path = Assembly.GetAssembly(type).Location;
			var directory = Path.GetDirectoryName(path);
			var name = Path.GetFileNameWithoutExtension(path);
			return $"{Path.Combine(directory, name)}.XmlSerializers.dll";
		}

		private string GetXmlSerializersFilename(Type type)
		{
			var path = GetXmlSerializersPath(type);
			var name = Path.GetFileName(path);
			return $"{name}.XmlSerializers.dll";
		}


		public string FileMessage => $"{GetXmlSerializersFilename(typeof(T))} Exists";

		public bool FileStatus => File.Exists(GetXmlSerializersPath(typeof(T)));

		public string ReadMessage => $"Serialization Read Successful";

		private bool _readStatus = false;
		public bool ReadStatus
		{
			get => _readStatus;
			set => SetProperty(ref _readStatus, value);
		}

		public string WriteMessage => $"Serialization Write Successful";

		private bool _writeStatus = false;
		public bool WriteStatus
		{
			get => _writeStatus;
			set => SetProperty(ref _writeStatus, value);
		}
		public string ErrorMessage => "Error Message";

		private string _errorStatus = "Not Tested Yet";
		public string ErrorStatus
		{
			get => _errorStatus;
			set => SetProperty(ref _errorStatus, value);
		}

		private RelayCommandFactory _runTestCommand = new RelayCommandFactory();
		public ICommand RunTestCommand => _runTestCommand.GetCommand(() =>
		{
			if (!_testHasRun)
			{
				_testHasRun = true;
				DoTest();
			}

		}, () => !_testHasRun);

		private string _fileContents = null;
		public string FileContents
		{
			get => _fileContents;
			set => SetProperty(ref _fileContents, value);
		}

		public string AssemblyVersion => GenerateAssemblyId(typeof(T));

		public string VersionAttributeNamespace => GenerateVersionAttributeNamespace(typeof(T)) ?? "{null}";

		public string SerializerParentAssemblyVersion => GenerateSerializerParentAssemblyId(typeof(T));

		public bool AssemblyVersionsEqual => AssemblyVersion == SerializerParentAssemblyVersion;

		public IEnumerable<Type> AllTypes =>
		  AppDomain
			.CurrentDomain
			.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(t => t.FullName.Contains("SerializationFail") && !t.FullName.Contains("_DisplayClass"));

	}
}
