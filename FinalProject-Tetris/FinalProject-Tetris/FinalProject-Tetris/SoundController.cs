using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace FinalProject_Tetris
{
    public class SoundController
    {
        private readonly SoundEffect _soundBlockPlace;
        private readonly SoundEffect _soundLineClear;
        private readonly SoundEffect _soundGameOver;
        private readonly Song _songTetris;

        public SoundController(
            SoundEffect soundBlockPlace, SoundEffect soundLineClear, SoundEffect soundGameOver,
            Song songTetris)
        {
            _soundBlockPlace = soundBlockPlace;
            _soundLineClear = soundLineClear;
            _soundGameOver = soundGameOver;
            _songTetris = songTetris;
        }

        public void PlayBlockPlace()
        {
            var instanceBlockPlace = _soundBlockPlace.CreateInstance();
            instanceBlockPlace.Play();
        }

        public void PlayLineClear()
        {
            var instanceLineClear = _soundLineClear.CreateInstance();
            instanceLineClear.Play();
        }

        public void PlayGameOver()
        {
            var instanceGameOver = _soundGameOver.CreateInstance();
            instanceGameOver.Play();
        }

        public void PlayMusic()
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_songTetris);
        }

        public void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        public void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}