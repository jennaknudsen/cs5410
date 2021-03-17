using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;

namespace LunarLander
{
    public class LocalStorageManager
    {
        // flags to alert whether currently in IO operation
        private bool _loadingControls = false;
        private bool _savingControls = false;
        private bool _loadingHighScores = false;
        private bool _savingHighScores = false;

        // file names
        private const string ControlsFileName = "ControlScheme.xml";
        private const string HighScoreFileName = "HighScores.xml";

        // make the stored control scheme public
        public ControlScheme StoredControlScheme;
        public HighScoreDiskStorage StoredHighScores;

        #region Control Scheme
        // public-facing method to load controls
        public void LoadControlScheme()
        {
            // don't want to be loading multiple things at once
            lock (this)
            {
                if (this._loadingControls) return;
                this._loadingControls = true;

#pragma warning disable CS4014
                FinalizeLoadControlsAsync();
#pragma warning restore CS4014
            }
        }

        // internal Task used to read the XML and load the controls
        private async Task FinalizeLoadControlsAsync()
        {
            await Task.Run(() =>
            {
                using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        if (storage.FileExists(ControlsFileName))
                        {
                            // open the XML file
                            using var fs = storage.OpenFile(ControlsFileName, FileMode.Open);
                            var mySerializer = new XmlSerializer(typeof(ControlScheme));
                            StoredControlScheme = (ControlScheme) mySerializer.Deserialize(fs);
                        }
                        else
                        {
                            // Make default control scheme if file doesn't exist
                            var thrustKeys = new[] {Keys.Up};
                            var leftKeys = new[] {Keys.Left};
                            var rightKeys = new[] {Keys.Right};

                            // save this as default scheme
                            SaveControlScheme(thrustKeys, leftKeys, rightKeys);

                            // wait for save to finish before loading
                            while (this._savingControls)
                            {
                                Thread.Sleep(10);
                            }

                            // now, load this as default
                            using var fs = storage.OpenFile(ControlsFileName, FileMode.Open);
                            var mySerializer = new XmlSerializer(typeof(ControlScheme));
                            StoredControlScheme = (ControlScheme) mySerializer.Deserialize(fs);
                        }
                    }
                    catch (IsolatedStorageException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                this._loadingControls = false;
            });
        }

        // public facing method to save controls
        public void SaveControlScheme(Keys[] thrustKeys, Keys[] rotateLeftKeys, Keys[] rotateRightKeys)
        {
            lock (this)
            {
                // don't run if already saving
                if (this._savingControls) return;

                // flag that this is saving
                this._savingControls = true;

                // Create something to save
                var myState = new ControlScheme(thrustKeys, rotateLeftKeys, rotateRightKeys);

                // save this control scheme
                FinalizeSaveControlsAsync(myState);
            }
        }

        // Finish saving the game with this async method
        private async void FinalizeSaveControlsAsync(ControlScheme controlScheme)
        {
            await Task.Run(() =>
            {
                using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        // open the XML file
                        using var fs = storage.OpenFile(ControlsFileName, FileMode.Create);

                        // serialize our controlScheme into XML and save it
                        var mySerializer = new XmlSerializer(typeof(ControlScheme));
                        mySerializer.Serialize(fs, controlScheme);
                    }
                    catch (IsolatedStorageException ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                // flag that we're no longer saving
                this._savingControls = false;
            });
        }
        #endregion

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
                            var hsList = new List<float>();

                            hsList.Add(54.33f);
                            hsList.Add(513.33f);
                            hsList.Add(11.3f);
                            hsList.Add(1f);

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

                this._loadingControls = false;
            });
        }

        // public facing method to save controls
        public void SaveHighScores(List<float> hsList)
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