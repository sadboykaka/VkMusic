using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Algh.interfaces;
using System.Collections.ObjectModel;
using System;


namespace Algh.Api.HttpUser
{
    class FileManager<T> : ITrackManager where T : IAudio, new()
    {
        public string dirpath { get; private set; }
        public string coverdir { get; private set; }
        public const string FolderName = "Audio";
        public const string CoverFolderName = "Covers";
        private IUser UserLogin;

        public ObservableCollection<IAudio> Downloads { get; private set; }

        public FileManager(string p, IUser user)
        {
            dirpath = p + '/' + FolderName;
            coverdir = p + '/' + CoverFolderName;
            if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
            if (!Directory.Exists(coverdir)) Directory.CreateDirectory(coverdir);
            UserLogin = user;

            Downloads = new ObservableCollection<IAudio>();
        }

        private (string,string) GetInfo(string path)
        {
            try
            {
                TagLib.File info = TagLib.File.Create(path);
                return (info.Tag.FirstPerformer, info.Tag.Title);
            }
            catch
            {
                return (null,null);
            }
        }

        private void SetInfo(string path, string Author, string Name)
        {
            TagLib.File info = TagLib.File.Create(path);
            info.Tag.Performers = new string[] { Author };
            info.Tag.Title = Name;
            info.Save();
        }

        private IAudio GetIAudio(string path)
        {
            Match buf = Regex.Match(Path.GetFileName(path), @"([0-9]+_[0-9]+).");
            if (!buf.Success) return null;
            var info = GetInfo(path);
            if (info.Item1 == null) return null;
            var tr = new T();
            tr.Url = path;
            tr.ID = buf.Groups[1].Value;
            tr.Author = info.Item1;
            tr.Name = info.Item2;
            string cpath = coverdir + '/' + tr.ID + ".jpg";
            if (tr.Cover != "" && System.IO.File.Exists(cpath)) tr.Cover = cpath;
            else tr.Cover = "";
            return tr;
        }

        public IEnumerable<IAudio> GetTracks()
        {
            var tracks = Directory.GetFiles(dirpath);

            var res = new List<IAudio>();
            IAudio track;
            foreach (string a in tracks)
            {
                track = GetIAudio(a);
                if (track == null) continue;
                res.Add(track);
            }
            return res;
        }

        public void DeleteTracks(IEnumerable<IAudio> trs)
        {
            foreach (IAudio tr in trs)
            {
                if (!System.IO.File.Exists(tr.Url)) continue;
                System.IO.File.Delete(tr.Url);
                if (tr.Cover != "" && System.IO.File.Exists(tr.Cover)) System.IO.File.Delete(tr.Cover);
            }
        }

        private string GetPath(IAudio tr)
        {
            var fileNameMask = dirpath + '/' + tr.ID;
            var filepath = fileNameMask + ".mp3";
            int i = 1;
            while (true)
            {
                if (System.IO.File.Exists(filepath)) filepath = fileNameMask + '(' + i + ").mp3";
                else break;
            }
            return filepath;
        }

        public async void LoadTrack(IAudio tr, ObservableCollection<IAudio> tl)
        {
            if (tl == null || tr == null) return;
            Stream streamF = null;
            Stream streamC = null;
            try
            {
                Downloads.Add(tr);
                streamF = await UserLogin.Data.Music.LoadTrack(tr);
                if(tr.Cover != "") streamC = await UserLogin.Data.Music.LoadCover(tr);

            }
            catch
            {
                return;
            }
            finally
            {
                Downloads.Remove(tr);
            }
            

            var filepath = GetPath(tr);
            var coverpath = coverdir + "/" + tr.ID + ".jpg";
            try
            {
                using (var fileStream = System.IO.File.Create(filepath)) streamF.CopyTo(fileStream);
                if (streamC != null && !System.IO.File.Exists(coverpath))
                {
                    using (var fileStream = System.IO.File.Create(coverpath)) streamC.CopyTo(fileStream);
                }
                SetInfo(filepath, tr.Author, tr.Name);
            }
            catch
            {
                return;
            }
            
            var res = GetIAudio(filepath);
   
            if (res != null) tl.Add(res);
        }

    }
}
