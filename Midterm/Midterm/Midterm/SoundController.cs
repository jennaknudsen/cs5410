using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Midterm
{
    public class SoundController
    {
        private readonly SoundEffect _soundBlockPlace;
        // private readonly SoundEffect _soundLineClear;
        // private readonly SoundEffect _soundGameOver;
        private readonly SoundEffect _soundBackgroundMusic;
        private readonly SoundEffectInstance _instanceBackgroundMusic;

        private bool _fadingOutMusicPause = false;
        private bool _fadingOutMusicStop = false;
        private bool _fadingInMusic = false;

        public SoundController(
            SoundEffect soundBlockPlace, SoundEffect soundBackgroundMusic)
            // SoundEffect soundBlockPlace, SoundEffect soundLineClear,
            // SoundEffect soundGameOver, SoundEffect soundTetrisSong)
        {
            _soundBlockPlace = soundBlockPlace;
            // _soundLineClear = soundLineClear;
            // _soundGameOver = soundGameOver;
            _soundBackgroundMusic = soundBackgroundMusic;
            _instanceBackgroundMusic = _soundBackgroundMusic.CreateInstance();
            _instanceBackgroundMusic.IsLooped = true;
        }

        public void PlayBlockPlace()
        {
            var instanceBlockPlace = _soundBlockPlace.CreateInstance();
            instanceBlockPlace.Play();
        }
        //
        // public void PlayLineClear()
        // {
        //     var instanceLineClear = _soundLineClear.CreateInstance();
        //     instanceLineClear.Play();
        // }
        //
        // public void PlayGameOver()
        // {
        //     var instanceGameOver = _soundGameOver.CreateInstance();
        //     instanceGameOver.Play();
        // }

        public void PlayMusic()
        {
            _fadingInMusic = true;
            _fadingOutMusicPause = false;
            _fadingOutMusicStop = false;
        }

        public void PauseMusic()
        {
            _fadingOutMusicPause = true;
            _fadingOutMusicStop = false;
            _fadingInMusic = false;
        }

        public void StopMusic()
        {
            _fadingOutMusicStop = true;
            _fadingInMusic = false;
            _fadingOutMusicPause = false;
        }

        // use this Update loop to control fading in / out background music
        public void Update(GameTime gameTime)
        {
            if (_fadingInMusic)
            {
                FadeInSoundEffect(_instanceBackgroundMusic);
            }
            else if (_fadingOutMusicPause)
            {
                FadeOutSoundEffect(_instanceBackgroundMusic, false);
            }
            else if (_fadingOutMusicStop)
            {
                FadeOutSoundEffect(_instanceBackgroundMusic, true);
            }
        }

        // fade in a sound effect instead of cutting in
        private void FadeInSoundEffect(SoundEffectInstance sei)
        {
            // resume won't cause sound to constantly replay
            sei.Resume();
            if (sei.Volume < 1)
            {
                try
                {
                    sei.Volume += 0.2f;
                }
                catch
                {
                    sei.Volume = 1f;
                }
            }
            else
            {
                _fadingInMusic = false;
            }
        }

        // fade out a sound instead of cutting it off
        private void FadeOutSoundEffect(SoundEffectInstance sei, bool stop)
        {
            // discrete steps
            if (sei.Volume > 0)
            {
                // try to lower the volume by 20%, otherwise set to 0
                try
                {
                    sei.Volume -= 0.2f;
                }
                catch
                {
                    sei.Volume = 0f;
                }
            }
            else
            {
                // if the sound is completely silent, then stop it
                if (stop)
                    sei.Stop();
                else
                    sei.Pause();

                _fadingOutMusicPause = false;
            }
        }
    }
}