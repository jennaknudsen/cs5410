using System.Collections.Generic;

namespace LunarLander
{
    public class HighScoreDiskStorage
    {
        // Control Scheme just holds three keys arrays
        public List<float> HighScores;

        // can't call default constructor
        private HighScoreDiskStorage()
        {
            // default constructor does nothing
        }

        public HighScoreDiskStorage(List<float> highScores)
        {
            HighScores = highScores;
        }
    }
}