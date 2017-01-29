using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
namespace mpm
{
	public class Status {
		public int status {get; set;}
		public String name {get;set;}
		public String version {get;set;}
		public String link {get;set;}
		public String message {get;set;}
		public String filename {get;set;}
	}
	class Program {
		public static void Main(string[] args) {
			String Run_Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			String API_Addres = file_get_contents(Run_Path+"Config.json");
			double MPM_Version = 0.1;
			if(args.Count() == 0){
				Console.WriteLine("mpm: try 'mpm help' for more information");
				System.Diagnostics.Process.GetCurrentProcess().Kill();
			}
			switch(args[0]){
				case "install":
					String PackageName=args[1];
					Console.WriteLine("Using Repository Addres:"+API_Addres);
					Console.WriteLine("Searching PackageName:"+PackageName);
					Thread.Sleep(1000);
					String result = XMLHttpRequest(API_Addres+"?package="+PackageName);
					var Json = JsonConvert.DeserializeObject<Status>(result);
					if(Json.status!= 200) {
						Console.WriteLine("ErrCode:"+Json.status+",Message:"+Json.message);
						System.Diagnostics.Process.GetCurrentProcess().Kill();
					}
					Console.WriteLine("ResponseCode:"+Json.status+",OK");
					Console.WriteLine("PackName:"+Json.name);
					Console.WriteLine("SupportVersion:"+Json.version);
					Console.WriteLine("Please enter the version you need to install...");
					Console.Write("Version:");
					String GetVersion = Console.ReadLine();
					Console.WriteLine("Downloading "+PackageName+",Version"+GetVersion+".....");
					file_put_contents(Json.filename,file_get_contents(Json.link+GetVersion+"/"+Json.filename));
					Console.WriteLine("Download Success,FileHash:"+GetMD5HashFromFile(Json.filename));
					//Thread.Sleep(2000);
					String Path = System.Environment.CurrentDirectory;
					Console.WriteLine("Save To:"+Path+"\\"+Json.filename);
					//Console.ReadKey(true);
					break;
				case "update":
					Console.WriteLine("TODO....");
					break;
				case "help":
					Console.WriteLine("Usage mpm [options...] <package|url>");
					Console.WriteLine("Options:");
					Console.WriteLine("	install	<package>		Install Package");
					Console.WriteLine("	remove <package>		Remove Package");
					Console.WriteLine("	repo <url>			Switch Repository Addres");
					Console.WriteLine("	info				Show MPM Info");
					break;
				case "remove":
					String RemovePackageName=args[1];
					String RemovePath = System.Environment.CurrentDirectory;
					Console.WriteLine("Uninstall PackageName:"+RemovePackageName);
					if(File.Exists(RemovePath+"\\"+RemovePackageName+".jar")){
						File.Delete(RemovePath+"\\"+RemovePackageName+".jar");
						Console.WriteLine("Remove Success");
					}else{
						Console.WriteLine("Package Name Not Found in:"+RemovePath);
					}
					break;
				case "repo":
					String Repository_Addr = args[1];
					String Exe_Path = System.Environment.CurrentDirectory;
					file_put_contents(Run_Path+"Config.json",Repository_Addr);
					Console.WriteLine("Switch Repository To:"+Repository_Addr);
					break;
				case "info":
					Console.WriteLine("Minecraft Package Manager Ver"+MPM_Version);
					Console.WriteLine("Binary Addres:"+System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
					Console.WriteLine("Current path:"+System.Environment.CurrentDirectory);
					Console.WriteLine("Repository Addres:"+API_Addres);
					Console.WriteLine("Contact:admin@touhou.cx");
					break;
			}
		}
		private static string GetMD5HashFromFile(string fileName){
			try
			{
				FileStream file = new FileStream(fileName, FileMode.Open);
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
				byte[] retVal = md5.ComputeHash(file);
				file.Close();
				
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < retVal.Length; i++)
				{
					sb.Append(retVal[i].ToString("x2"));
				}
				return sb.ToString();
			}
			catch (Exception ex)
			{
				throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
			}
		}
		public static String XMLHttpRequest(String API_Addres) {
			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(API_Addres);
			httpRequest.Timeout = 5000;
			httpRequest.Method = "GET";
			HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
			StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("gb2312"));
			String result = sr.ReadToEnd();
			result = result.Replace("\r","").Replace("\n","").Replace("\t","");
			int status_code = (int)httpResponse.StatusCode;
			sr.Close();
			return result;
		}
		public static bool file_put_contents(String PathFileName,String Content) {
			System.IO.File.WriteAllText(@PathFileName,Content, Encoding.UTF8);
			return true;
		}
		public static String file_get_contents(String fileName){
			String Contents = String.Empty;
			if (fileName.ToLower().IndexOf("http") > -1){
				System.Net.WebClient wc = new System.Net.WebClient();
				Byte[] response = wc.DownloadData(fileName);
				Contents = System.Text.Encoding.UTF8.GetString(response);
			}else{
				System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
				Contents = sr.ReadToEnd();
				sr.Close();
			}
			return Contents;
		}
	}
}