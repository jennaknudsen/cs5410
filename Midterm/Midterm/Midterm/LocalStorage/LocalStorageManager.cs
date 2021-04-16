using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;

namespace Midterm.LocalStorage
{
    public class LocalStorageManager
    {
       // flags to alert whether currently in IO operation
        private bool _loadingHighScores = false;
        private bool _savingHighScores = false;

        // file names
        private const string HighScoreFileName = "HighScores.xml";

        // make the stored control scheme public
        public HighScoreDiskStorage StoredHighScores;

        #region High Scores

        // public-facing method to load controls
        public void LoadHighScores()
        {
            // don't want to be loading multiple things at once
            lock (this)
            {
                if (this._loadingHighScores) return;
                this._loadingHighScores = true;

#pragma warning disable CS4014
                FinalizeLoadHighScoresAsync();
#pragma warning restore CS4014
            }
        }

        // internal Task used to read the XML and load the high scores
        private async Task FinalizeLoadHighScoresAsync()
        {
            await Task.Run(() =>
            {
                using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists(HighScoreFileName))
                        {
                            // open the XML file
                            using var fs = storage.OpenFile(HighScoreFileName, FileMode.Open);
                            var mySerializer = new XmlSerializer(typeof(HighScoreDiskStorage));
                            StoredHighScores = (HighScoreDiskStorage) mySerializer.Deserialize(fs);
                        }
                        else
                        {
                            var hsList = new List<int>();

                            // save high scores list (empty for now)
                            SaveHighScores(hsList);

                            // wait for save to finish before loading
                            while (this._savingHighScores)
                            {
                                Thread.Sleep(10);
                            }

                            // now, load this as default
                            using var fs = storage.OpenFile(HighScoreFileName, FileMode.Open);
                            var mySerializer = new XmlSerializer(typeof(HighScoreDiskStorage));
                            StoredHighScores = (HighScoreDiskStorage) mySerializer.Deserialize(fs);
                        }
                    }
                    catch (IsolatedStorageException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                this._loadingHighScores = false;
            });
        }

        // public facing method to save controls
        public void SaveHighScores(List<int> hsList)
        {
            lock (this)
            {
                // don't run if already saving
                if (this._savingHighScores) return;

                // flag that this is saving
                this._savingHighScores = true;

                // Create something to save
                var myState = new HighScoreDiskStorage(hsList);

                // save this control scheme
                FinalizeSaveHighScoresAsync(myState);
            }
        }

        // Finish saving the high scores with this async method
        private async void FinalizeSaveHighScoresAsync(HighScoreDiskStorage highScoreDiskStorage)
        {
            await Task.Run(() =>
            {
                using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        // open the XML file
                        using var fs = storage.OpenFile(HighScoreFileName, FileMode.Create);

                        // serialize our controlScheme into XML and save it
                        var mySerializer = new XmlSerializer(typeof(HighScoreDiskStorage));
                        mySerializer.Serialize(fs, highScoreDiskStorage);
                    }
                    catch (IsolatedStorageException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                // flag that we're no longer saving
                this._savingHighScores = false;
            });
        }
        #endregion
    }
}