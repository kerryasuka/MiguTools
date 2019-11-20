using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiguTools
{
    public class FileOperation
    {
        public event Action<int> CountEvent;
        public event Action<List<string>> ErrorEvent;
        public event Action<string, int> LogEvent;
        public event Action EndEvent;

        private string sourcePath = string.Empty;
        private string desPath = string.Empty;

        private List<FileInfomation> m_fileInfomation;
        private List<string> m_abnormalFile;

        private int totalCount = 0;
        private int normalCount = 0;

        public FileOperation()
        {
            string basePath = AppContext.BaseDirectory;
            sourcePath = Path.Combine(basePath, "Source");
            desPath = Path.Combine(basePath, "Destination");
            m_fileInfomation = new List<FileInfomation>();
            m_abnormalFile = new List<string>();
        }

        public void Start()
        {
            GetFilesInfo();
            Rename();
            EndEvent?.Invoke();
        }

        private void GetFilesInfo()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(sourcePath);
            List<FileInfo> files = dirInfo.GetFiles().ToList();

            totalCount = files.Count;
            CountEvent?.Invoke(totalCount);

            Adapter(files);
        }

        private void Adapter(List<FileInfo> files)
        {
            files.ForEach(f =>
            {
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(f.Name);
                List<string> sns = nameWithoutExtension.Split("-".ToCharArray()).ToList();

                if (sns.Count > 2)
                {
                    m_abnormalFile.Add(nameWithoutExtension);
                    return;
                }

                sns.ForEach(s =>
                    {
                        if (string.IsNullOrEmpty(s)) { sns.Remove(s); };
                    });

                FileInfomation info = new FileInfomation()
                {
                    FileInfo = f,
                    SourcePath = sourcePath,
                    DestinationPath = desPath,
                    NameWithoutExtension = nameWithoutExtension,
                    FileType = (FileType)Enum.Parse(typeof(FileType), Path.GetExtension(f.Name).ToLower().Replace(".", "")),
                    Performers = new string[] { sns[1].Trim() },
                    Title = sns[0].Trim(),
                };

                m_fileInfomation.Add(info);
            });

            ErrorEvent?.Invoke(m_abnormalFile);
        }

        private void Rename()
        {
            m_fileInfomation.ForEach(f =>
            {
                string fileName = string.Format("{0} - {1}.{2}", string.Join(',', f.Performers), f.Title, f.FileType.ToString());
                string newFullPath = Path.Combine(f.DestinationPath, fileName);
                byte[] chunk = new byte[1024 * 1024];
                int count = 0;
                FileStream fsr = f.FileInfo.Open(FileMode.Open, FileAccess.Read);
                FileStream fsw = new FileStream(newFullPath, FileMode.Create, FileAccess.Write);

                while ((count = fsr.Read(chunk, 0, chunk.Length)) > 0)
                {
                    fsw.Write(chunk, 0, count);
                }

                fsw.Close();
                fsr.Close();

                TagModify(newFullPath, f);
            });
        }

        private void TagModify(string fullPath, FileInfomation fileInfomation)
        {
            var file = TagLib.File.Create(fullPath);
            var tag1 = file.GetTag(TagLib.TagTypes.Id3v1);

            tag1.Title = fileInfomation.Title;
            tag1.Performers = fileInfomation.Performers;
            tag1.Comment = "";

            var tag2 = file.GetTag(TagLib.TagTypes.Id3v2);

            tag2.Title = fileInfomation.Title;
            tag2.Performers = fileInfomation.Performers;
            tag2.Comment = "Create by MiguTools copyright protected MiguTools. Author kerryasuka.";

            file.Save();

            File.Delete(fileInfomation.FileInfo.FullName);
            normalCount++;
            LogEvent?.Invoke(string.Format("{0} - {1}", string.Join(',', fileInfomation.Performers), fileInfomation.Title), normalCount);
        }
    }
}
