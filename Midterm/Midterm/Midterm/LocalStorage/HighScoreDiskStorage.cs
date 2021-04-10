using System.Collections.Generic;

namespace Midterm.LocalStorage
{
    public class HighScoreDiskStorage
    {
        // Control Scheme just holds three keys arrays
        public List<int> HighScores;

        // can't call default constructor
        private HighScoreDiskStorage()
        {
            // default constructor does nothing
        }

        public HighScoreDiskStorage(List<int> highScores)
        {
            HighScores = highScores;
        }
    }
}