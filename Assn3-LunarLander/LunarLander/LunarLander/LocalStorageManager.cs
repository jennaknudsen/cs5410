using System;
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
        private bool _loading = false;
        private bool _saving = false;

        // reference to the LanderGameController
        // private LanderGameController _landerGameController;

        // make the stored control scheme public
        public ControlScheme StoredControlScheme;

        // constructor will just assign the correct LanderGameController
        // public LocalStorageManager(LanderGameController landerGameController)
        // {
        //     _landerGameController = landerGameController;
        // }

        // public-facing method to load controls
        public void LoadControlScheme()
        {
            // don't want to be loading multiple things at once
            lock (this)
            {
                if (this._loading) return;
                this._loading = true;

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
                        if (storage.FileExists("ControlScheme.xml"))
                        {
                            // open the XML file
                            using var fs = storage.OpenFile("ControlScheme.xml", FileMode.Open);
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
                            while (this._saving)
                            {
                                Thread.Sleep(10);
                            }

                            // now, load this as default
                            using var fs = storage.OpenFile("ControlScheme.xml", FileMode.Open);
                            var mySerializer = new XmlSerializer(typeof(ControlScheme));
                            StoredControlScheme = (ControlScheme) mySerializer.Deserialize(fs);
                        }
                    }
                    catch (IsolatedStorageException ex)
                    {
                        Console.WriteLine("File didn't exist!!");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                    }
                }

                this._loading = false;
            });
        }

        // public facing method to save controls
        public void SaveControlScheme(Keys[] thrustKeys, Keys[] rotateLeftKeys, Keys[] rotateRightKeys)
        {
            lock (this)
            {
                // don't run if already saving
                if (this._saving) return;

                // flag that this is saving
                this._saving = true;

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
                        using var fs = storage.OpenFile("ControlScheme.xml", FileMode.Create);

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
                this._saving = false;
            });
        }
    }
}