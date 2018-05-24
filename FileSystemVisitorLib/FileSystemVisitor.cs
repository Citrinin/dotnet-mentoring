﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace FileSystemVisitorLib
{
    /// <summary>
    /// Represents class for searching directories and folders in file system
    /// </summary>
    public class FileSystemVisitor
    {
        /// <summary>
        /// Represents filter for sorting content of the folder
        /// </summary>
        private readonly Predicate<string> _filter;

        /// <summary>
        /// File system for providing an opportunity for testing
        /// </summary>
        private readonly IFileSystem _fileSystem;

        /// <summary>
        /// Initializes a new instance of FileSystemVisitor for searching content of the folder in the test mode
        /// </summary>
        /// <param name="fileSystem">File system for providing an opportunity for testing</param>
        public FileSystemVisitor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Initializes a new instance of FileSystemVisitor for searching content of the folder 
        /// </summary>
        public FileSystemVisitor() : this(new FileSystem())
        {

        }

        /// <summary>
        /// Initializes a new instance of FileSystemVisitor for searching content of the folder
        /// </summary>
        /// <param name="filter">Represents algorithm for filtering result of search</param>
        public FileSystemVisitor(Predicate<string> filter) : this()
        {
            _filter = filter;
        }

        /// <summary>
        /// Initializes a new instance of FileSystemVisitor for searching content of the folder in the test mode
        /// </summary>
        /// <param name="fileSystem">File system for providing an opportunity for testing</param>
        /// <param name="filter">Represents algorithm for filtering result of search</param>
        public FileSystemVisitor(IFileSystem fileSystem, Predicate<string> filter) : this(fileSystem)
        {
            _filter = filter;
        }

        /// <summary>
        /// Returns full path of directories and files that are stored in the specified folder 
        /// </summary>
        /// <param name="directoryName">Path to the start folder</param>
        /// <returns>List of full path to items that are sotred from in specified folder</returns>
        /// <exception cref="ArgumentException"></exception>
        public IEnumerable<string> GetDirectoryContent(string directoryName)
        {
            OnStart();
            var cancelFlag = false;
            var directory = _fileSystem.DirectoryInfo.FromDirectoryName(directoryName);
            if (_filter != null)
            {
                foreach (var item in GetDirectoryItems(directory, ref cancelFlag))
                {
                    if (!_filter(item.FullName)) continue;
                    var eventParams = new ItemFindedEventArgs { FullName = item.FullName };

                    switch (item)
                    {
                        case DirectoryInfoBase _:
                        {
                            OnFilteredDirectoryFinded(eventParams);
                            break;
                        }
                        case FileInfoBase _:
                        {
                            OnFilteredFileFinded(eventParams);
                            break;
                        }
                        default:
                        {
                            throw new InvalidCastException();
                        }
                    }

                    if (eventParams.CancelSearching)
                    {
                        yield return item.FullName;
                        OnFinish();
                        yield break;

                    }

                    if (!eventParams.ExceptItemFromResult)
                    {
                        yield return item.FullName;
                    }
                }
            }
            else
            {
                foreach (var item in GetDirectoryItems(directory, ref cancelFlag))
                {
                    yield return item.FullName;
                }
            }
            OnFinish();
        }

        /// <summary>
        /// Returns directories and files that are stored in the specified directory
        /// </summary>
        /// <param name="dir">Start directory</param>
        /// <param name="cancelRecursionFlag">Flag to cancel searching</param>
        /// <returns>Directories and files that are stored in the specified directory</returns>
        private IEnumerable<FileSystemInfoBase> GetDirectoryItems(DirectoryInfoBase dir, ref bool cancelRecursionFlag)
        {
            var result = new List<FileSystemInfoBase>();

            if (cancelRecursionFlag)
            {
                return result;
            }

            IEnumerable<DirectoryInfoBase> directories;
            try
            {
                directories = dir.EnumerateDirectories();
            }
            catch (DirectoryNotFoundException e)
            {
                throw new ArgumentException($"Incorrect start path: {dir.FullName}", e);
            }

            foreach (var directory in directories)
            {
                var eventParams = new ItemFindedEventArgs { FullName = directory.FullName };
                OnDirectoryFinded(eventParams);

                if (eventParams.CancelSearching)
                {
                    cancelRecursionFlag = true;
                    result.Add(directory);
                    return result;
                }

                if (!eventParams.ExceptItemFromResult)
                {
                    result.Add(directory);
                    result.AddRange(GetDirectoryItems(directory, ref cancelRecursionFlag));
                    if (cancelRecursionFlag)
                    {
                        return result;
                    }
                }
            }
            foreach (var fileInfo in dir.EnumerateFiles())
            {

                var eventParams = new ItemFindedEventArgs { FullName = fileInfo.FullName };
                OnFileFinded(eventParams);

                if (eventParams.CancelSearching)
                {
                    cancelRecursionFlag = true;
                    result.Add(fileInfo);
                    return result;
                }

                if (!eventParams.ExceptItemFromResult)
                {
                    result.Add(fileInfo);
                }
            }
            return result;
        }

        /// <summary>
        /// Occures before search starts
        /// </summary>
        public event EventHandler Start;

        /// <summary>
        /// Occures after search ends
        /// </summary>
        public event EventHandler Finish;

        /// <summary>
        /// Occures when any file in the specified directory is found
        /// </summary>
        public event EventHandler<ItemFindedEventArgs> FileFinded;

        /// <summary>
        /// Occures when any folder in the specified directory is found
        /// </summary>
        public event EventHandler<ItemFindedEventArgs> DirectoryFinded;

        /// <summary>
        /// Occures when file in the specified directory that match filter is found
        /// </summary>
        public event EventHandler<ItemFindedEventArgs> FilteredFileFinded;

        /// <summary>
        /// Occures when folder in the specified directory that match filter is found
        /// </summary>
        public event EventHandler<ItemFindedEventArgs> FilteredDirectoryFinded;

        /// <summary>
        /// Raises the FileSystemVisitor Start event
        /// </summary>
        protected virtual void OnStart()
        {
            var tmp = Start;
            tmp?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the FileSystemVisitor Finish event
        /// </summary>
        protected virtual void OnFinish()
        {
            var tmp = Finish;
            tmp?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the FileSystemVisitor FileFinded event
        /// </summary>
        /// <param name="args">The event data for the FileFinded event</param>
        protected virtual void OnFileFinded(ItemFindedEventArgs args)
        {
            var tmp = FileFinded;
            tmp?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the FileSystemVisitor DirectoryFinded event
        /// </summary>
        /// <param name="args">The event data for the DirectoryFinded event</param>
        protected virtual void OnDirectoryFinded(ItemFindedEventArgs args)
        {
            var tmp = DirectoryFinded;
            tmp?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the FileSystemVisitor FilteredFileFinded event
        /// </summary>
        /// <param name="args">The event data for the FilteredFileFinded event</param>
        protected virtual void OnFilteredFileFinded(ItemFindedEventArgs args)
        {
            var tmp = FilteredFileFinded;
            tmp?.Invoke(this, args);
        }

        /// <summary>
        /// Raises the FileSystemVisitor FilteredDirectoryFinded event
        /// </summary>
        /// <param name="args">The event data for the FilteredDirectoryFinded event</param>
        protected virtual void OnFilteredDirectoryFinded(ItemFindedEventArgs args)
        {
            var tmp = FilteredDirectoryFinded;
            tmp?.Invoke(this, args);
        }
    }
}

