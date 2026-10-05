using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;

namespace model.Interfaces.Sound
{
    public interface IMusicQueueService
    {
        public void AddMusic(IEnumerable<Song> songs);
        public void AddMusic(Song song);
        public void ClearQueue();
        public Song? GetSelectedSong();
        public void SetSelectedSong(int index);
        public int GetSelectedSongIndex();
        public void SetWrap(bool wrap);
        public bool GetWrap();
    }
}