using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiguTools
{
    public class FileInfomation
    {        
        public FileInfo FileInfo { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string NameWithoutExtension { get; set; }
        public FileType FileType { get; set; }
        public string[] Performers { get; set; }
        public string Title { get; set; }
    }

    public enum FileType
    {
        mp3,
        aac,
        wav,
        flac,
        ape,
    }
}
