/*
*
*   A very simple implementation of a very simple sound system.
*   @author Michael Heron
*   @version 1.0
*   
*/

using SDL2;
using System;
using System.Collections.Generic;

namespace Shard
{
    public class SoundSDL : Sound
    {
        private List<AudioDevice> channels;
        private int inOrder;
        class AudioDevice
        {
            public SDL.SDL_AudioSpec have;
            public SDL.SDL_AudioSpec want;
            public uint length;
            public uint dev;

            
        }
        
        
        public SoundSDL()
        {
            channels = new List<AudioDevice>();
            for(int i = 0; i < 10; i++)
            {
                channels.Add(new AudioDevice());
                channels[i].dev = SDL.SDL_OpenAudioDevice(IntPtr.Zero, 0, ref channels[i].have, out channels[i].want, 0);
            }
        }
        public override void playSound(string file)
        {
            AudioDevice device;
            inOrder++;
            if(inOrder >= channels.Count)
            {
                inOrder = 0;
            }
            device = channels[inOrder];
            IntPtr buffer;

            file = Bootstrap.getAssetManager().getAssetPath(file);

            SDL.SDL_LoadWAV(file, out device.have, out buffer, out device.length);
            
            string test = SDL.SDL_GetError();
            Debug.Log(test);
            int success = SDL.SDL_QueueAudio(device.dev, buffer, device.length);
            SDL.SDL_PauseAudioDevice(device.dev, 0);

        }

    }
}

