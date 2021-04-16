using System.Collections.Generic;

namespace Midterm.LocalStorage
{
    public class TimeScoreDiskStorage
    {
        // Control Scheme just holds three keys arrays
        public List<double> TimeHighScores;

        // can't call default constructor
        private TimeScoreDiskStorage()
        {
            // default constructor does nothing
        }

        public TimeScoreDiskStorage(List<double> highScores)
        {
            TimeHighScores = highScores;
        }
    }
}