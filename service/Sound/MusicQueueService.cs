using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Media;
using model.Interfaces.Sound;

namespace service.Sound
{
    public class MusicQueueService : IMusicQueueService
    {
        private readonly List<Song> _songCollection = new List<Song>();
        private int _selectedIndex = 0;
        private bool _wrap = false;

        public void AddMusic(IEnumerable<Song> songs)
        {
            _songCollection.AddRange(songs);
        }

        public void AddMusic(Song song)
        {
            _songCollection.Add(song);
        }

        public void ClearQueue()
        {
            _songCollection.Clear();
        }

        public Song? GetSelectedSong()
        {
            Song? sng = null;
            if(_selectedIndex >= 0 && _selectedIndex < _songCollection.Count)
            {
                sng = _songCollection[_selectedIndex];
            }   
            return sng;
        }

        public void SetSelectedSong(int index)
        {
            if(_wrap == false)
            {
                SetSelectedSongNowrap(index);
            } else {
                SetSelectedSongWrap(index);
            }
        }

        private void SetSelectedSongNowrap(int index)
        {
            _selectedIndex = Math.Clamp(index, 0, _songCollection.Count - 1);
        }

        private void SetSelectedSongWrap(int index)
        {
            var maxValCollection = _songCollection.Count - 1;
            if(maxValCollection == 0)
            {
                SetSelectedSongNowrap(index);
                return;
            }
            
            var indexRemainder = index % maxValCollection;
            if(indexRemainder >= 0)
            {
                _selectedIndex = indexRemainder;
            } else {
                _selectedIndex = maxValCollection - indexRemainder + 1;
            }
        }

        public int GetSelectedSongIndex()
        {
            return _selectedIndex;
        }

        public void SetWrap(bool wrap)
        {
            _wrap = wrap;
        }

        public bool GetWrap()
        {
            return _wrap;
        }
    }
}