using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRelease
{
    class SolutionWatcher
    {
        private string _srcFolder;
        private string _siteRootFolder;

        private FileSystemWatcher _fileSystemWatcher;
        private Dictionary<string, List<DllFile>> _watchFiles;
        private Dictionary<string, DllFile> _releaseFiles;

        public SolutionWatcher(string srcFolder, string siteRootFolder)
        {
            this._srcFolder = srcFolder;
            this._siteRootFolder = siteRootFolder;


        }

        public void Start()
        {
            var siteDll = Directory.GetFiles(Path.Combine(_siteRootFolder, "bin"), "*.dll", SearchOption.TopDirectoryOnly);
            //_releaseFiles = siteDll.Select(x => new DllFile(x))
            //    .ToDictionary(x => x.ShortFileName);
            _releaseFiles = new Dictionary<string, DllFile>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in siteDll)
            {
                var dll = new DllFile(file);
                _releaseFiles.Add(dll.ShortFileName, dll);
            }

            _fileSystemWatcher = new FileSystemWatcher(_srcFolder);
            _fileSystemWatcher.Created += fileSystemWatcher_Changed;
            _fileSystemWatcher.Changed += fileSystemWatcher_Changed;
        }

        private async void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (string.Equals("dll", Path.GetExtension(e.Name)) == false)
                return;

            var file = new DllFile(e.FullPath);
            if (_releaseFiles.TryGetValue(file.ShortFileName, out var releaseFile) == false)
                return;

            if (string.Equals(releaseFile.GetFileHash(), file.GetFileHash()))
                return;

            releaseFile.CopyFrom(file.FileInfo.FullName);
        }
    }


    public class DllFile
    {
        public FileInfo FileInfo { get; }

        public string ShortFileName => FileInfo.Name;

        public DllFile(FileInfo file)
        {
            FileInfo = file;
        }

        public DllFile(string filepath)
        {
            FileInfo = new FileInfo(filepath);
        }


        private string _fileHash;
        public string GetFileHash()
        {
            if (string.IsNullOrEmpty(_fileHash))
            {
                _fileHash = FileInfo.Length + ";" + FileInfo.LastWriteTime.Ticks;
            }

            return _fileHash;
        }

        public void CopyFrom(string filename)
        {

        }

        public override bool Equals(object obj)
        {
            if (obj is DllFile dllFile)
            {
                return string.Equals(this.FileInfo.FullName, dllFile.FileInfo.FullName,
                    StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.FileInfo.FullName.GetHashCode();
        }
    }
}
