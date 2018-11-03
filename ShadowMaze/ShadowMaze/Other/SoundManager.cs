using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace ShadowMaze
{
    // Loads and plays sounds for the game as needed.
    public class SoundManager
    {
        private Dictionary<string, SoundEffect> dictSound; // Stores a dictionary of sound effects.
        private Dictionary<string, SoundEffectInstance> dictInstSound; // Stores a dictionary of instances of sounds.
        private string[] strEffects; // Stores the names of the sound effects.

        // Loads the audio files for each sound effect and adds the sound effects to the dictionaries.
        public void LoadContent(ContentManager Content)
        {
            strEffects = new string[]{ "Rain", "Button", "drk", "bld", "lbm", "frt", "Damage"};

            dictInstSound = new Dictionary<string, SoundEffectInstance>();
            dictSound = new Dictionary<string, SoundEffect>();
            
            // Adds each sound effect and each sound effect's instance to its respective dictionary.
            for (int intEffectIndex = 0; intEffectIndex < strEffects.GetLength(0); intEffectIndex++)
            {
                dictSound.Add(strEffects[intEffectIndex], Content.Load<SoundEffect>("SoundEffects/" + strEffects[intEffectIndex]));
                dictInstSound.Add(strEffects[intEffectIndex], dictSound[strEffects[intEffectIndex]].CreateInstance());
            }
        }

        // Plays a sound from the dictionary of sound effect instances whose name matches the string parameter.
        public void PlaySound(string strSound)
        {
            if (strSound != "Rain")
            {
                SoundEffectInstance soundToPlay;
                soundToPlay = dictInstSound[strSound];
                // The sound to be played is the sound whose dictionary key is the same as the string paramater.
                if (soundToPlay.State != SoundState.Playing)
                    soundToPlay.Play(); // Play the sound if it isn't already playing.
                else
                { // If it is playing, restart the sound effect by disposing of the instance and creating a new one.
                    soundToPlay.Dispose();
                    soundToPlay = dictSound[strSound].CreateInstance();
                    dictInstSound[strSound] = soundToPlay;
                    soundToPlay.Play();
                }
            } // If the sound was rain, simply play it.
            else dictInstSound[strSound].Play();
        }

        // Stops the background rain sound.
        public void StopRain()
        {
            dictInstSound["Rain"].Stop();
        }
    }
}
