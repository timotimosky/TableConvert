using System;
using System.IO;

namespace TableConvert.Utility.FileWatcher
{

    public class FileListener
    {

        private string _listeningFilter;
        private string _listeningToPath;
        private readonly OnPathChange _fileListener;
        private readonly FileSystemWatcher _fileSystemWatcher;


        public delegate void OnPathChange(WatcherChangeTypes type, FileSystemEventArgs args);

        public bool IsListener
        {
            get; private set;
        }


        public static FileListener FetchFileListener(string path, OnPathChange listener, string filter = "*.*")
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (listener == null)
                throw new ArgumentNullException("listener");

            return new FileListener(path, listener, filter);
        }


        internal FileListener(string path, OnPathChange listener, string filter)
        {
            if (Directory.Exists(path) == false)
                return;

            _listeningToPath = path;

            _fileListener = listener;
            _listeningFilter = filter;

            _fileSystemWatcher = new FileSystemWatcher(path, filter);
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Created += OnCreated;
            _fileSystemWatcher.Renamed += OnRenamed;
            _fileSystemWatcher.Deleted += OnDeleted;

            Start();
        }

     

        public void Start()
        {
            if (_fileSystemWatcher != null && _fileSystemWatcher.EnableRaisingEvents == false)
                _fileSystemWatcher.EnableRaisingEvents = true;

            IsListener = true;
        }

        public void Stop()
        {
            if (_fileSystemWatcher != null && _fileSystemWatcher.EnableRaisingEvents)
                _fileSystemWatcher.EnableRaisingEvents = false;

            IsListener = false;
        }


        private void OnChanged(object sender, FileSystemEventArgs args)
        {
            if (_fileListener != null)
            {
                _fileListener(args.ChangeType, args);
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs args)
        {
            if (_fileListener != null)
            {
                _fileListener(args.ChangeType, args);
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs args)
        {
            if (_fileListener != null)
            {
                _fileListener(args.ChangeType, args);
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs args)
        {
            if (_fileListener != null)
            {
                _fileListener(args.ChangeType, args);
            }
        }
    }
}