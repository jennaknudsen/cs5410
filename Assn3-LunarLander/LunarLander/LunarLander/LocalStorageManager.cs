using System.IO;
using System.IO.IsolatedStorage;
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
                if (!this._loading)
                {
                    this._loading = true;

#pragma warning disable CS4014
                    FinalizeLoadControlsAsync();
#pragma warning restore CS4014
                }
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
                        if (storage.FileExists("HighScores.xml"))
                        {
                            using var fs = storage.OpenFile("ControlScheme.xml", FileMode.Open);
                            if (fs != null)
                            {
                                var mySerializer = new XmlSerializer(typeof(ControlScheme));
                                StoredControlScheme = (ControlScheme) mySerializer.Deserialize(fs);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this._loading = false;
            });
        }

        // public facing method to save controls
        private void SaveControlScheme(Keys[] thrustKeys, Keys[] rotateLeftKeys, Keys[] rotateRightKeys)
        {
            lock (this)
            {
                if (!this._saving)
                {
                    this._saving = true;

                    // Create something to save
                    var myState = new ControlScheme(thrustKeys, rotateLeftKeys, rotateRightKeys);

                    // save this control scheme
                    FinalizeSaveControlsAsync(myState);
                }
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
                        using (IsolatedStorageFileStream fs = storage.OpenFile("ControlScheme.xml", FileMode.OpenOrCreate))
                        {
                            if (fs != null)
                            {
                                XmlSerializer mySerializer = new XmlSerializer(typeof(ControlScheme));
                                mySerializer.Serialize(fs, controlScheme);
                            }
                        }
                    }
                    catch (IsolatedStorageException)
                    {
                        // Ideally show something to the user, but this is demo code :)
                    }
                }

                this._saving = false;
            });
        }
    }
}