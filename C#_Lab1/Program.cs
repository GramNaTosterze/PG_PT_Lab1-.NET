using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

class Program
{
    static void Main(string[] args)
    {
        foreach (string arg in args)
        {
            ProcessDir(arg, "");
            DirectoryInfo directory = new DirectoryInfo(arg);
            Console.WriteLine("\n"+directory.OldestCreationTime()+"\n");
            CreateCollection(arg);
        }
    }
    public static void ProcessDir(string path, string d)
    {
        FileSystemInfo currentDir = new FileInfo(path);
        string[] files = Directory.GetFiles(path);
        string[] subDirectories = Directory.GetDirectories(path);

        Console.WriteLine("{0}{1} ({2}) {3}", d, currentDir.Name, (files.Length + subDirectories.Length), currentDir.GetRAHS());
        d += "        ";




        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            Console.WriteLine("{0}{1} {2} bajtów {3}", d, fileInfo.Name, fileInfo.Length, fileInfo.GetRAHS());
        }
        foreach (string subDirectory in subDirectories)
            ProcessDir(subDirectory, d);
    }
    public static void CreateCollection(string path)
    {
        SortedDictionary<string, int> collection = new SortedDictionary<string, int>(new CompareInCollection());


        string[] files = Directory.GetFiles(path);
        string[] subDirectories = Directory.GetDirectories(path);
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            collection.Add(fileInfo.Name, (int)fileInfo.Length);
        }
        foreach (string subDirectory in subDirectories)
        {
            FileSystemInfo dirInfo = new FileInfo(subDirectory);
            int numberOfFiles = Directory.GetFiles(subDirectory).Length;
            int numberOfDirectories = Directory.GetDirectories(subDirectory).Length;
            collection.Add(dirInfo.Name, numberOfFiles + numberOfDirectories);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream("collection.data", FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, collection);
        stream.Close();

        Decentralize();
    }

    public static void Decentralize()
    {
        Stream stream = new FileStream("collection.data", FileMode.Open, FileAccess.Read);
        BinaryFormatter formatter = new BinaryFormatter();
        SortedDictionary<string, int> collection = (SortedDictionary<string, int>)formatter.Deserialize(stream);
        foreach (var pair in collection)
            Console.WriteLine("{0} -> {1}",pair.Key, pair.Value);
    }
}

[Serializable]
public class CompareInCollection : IComparer<string>
{
    public int Compare(string x, string y)
    {
        if (x.Length > y.Length) return 1;
        if (x.Length < y.Length) return -1;
        else return x.CompareTo(y);
    }
}
public static class ExtendIO
{
    public static DateTime OldestCreationTime(this System.IO.DirectoryInfo io)
    {
        DateTime minCreationTime = io.CreationTime;
        FileInfo[] files = io.GetFiles();
        DirectoryInfo[] directories = io.GetDirectories();
        
        foreach (FileInfo file in files)
        {
            minCreationTime = (minCreationTime < file.CreationTime ? minCreationTime : file.CreationTime);
        }
        foreach (DirectoryInfo directory in directories)
        {
            DateTime ret = directory.OldestCreationTime();
            minCreationTime = (minCreationTime < ret ? minCreationTime : ret);
        }

        return minCreationTime;
    }
    public static string GetRAHS(this System.IO.FileSystemInfo io)
    { 
        FileAttributes atr = io.Attributes;
        bool r = (atr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        bool a = (atr & FileAttributes.Archive) == FileAttributes.Archive;
        bool h = (atr & FileAttributes.Hidden) == FileAttributes.Hidden;
        bool s = (atr & FileAttributes.System) == FileAttributes.System;
        return (r ? "r" : "-") + (a ? "a" : "-") + (h ? "h" : "-") + (s ? "s" : "-");
    }
}