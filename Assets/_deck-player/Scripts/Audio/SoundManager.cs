using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeckPlayer.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public List<AudioSource> sources;

        private AudioDataCollection collection;
        public Dictionary<string, AudioData> AudioDict = new Dictionary<string, AudioData>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            collection = Resources.Load<AudioDataCollection>("AudioData/AudioDataCollection");

            foreach (AudioData audio in collection.GetCollection())
            {
                AudioDict.Add(audio.AudioName, audio);
            }
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            DestroySources();
        }

        /// <summary>
        /// The standard Play function, plays a clip
        /// </summary>
        public void Play(string audioName)
        {
            AudioSource source = PrepareSource(audioName);
            
            source.loop = false;
            source.Play();
        }

        /// <summary>
        /// Creates and prepares a source, used in the Play method
        /// </summary>
        private AudioSource PrepareSource(string audioName)
        {
            AudioData audio = AudioDict[audioName];
            AudioSource source = GetSource();
            source.clip = audio.GetClip();
            source.volume = audio.volume;
            if (!audio.randomPitch)
                source.pitch = audio.pitch;
            else
                source.pitch = Random.Range(0.9f, 1.3f);

            return source;
        }

        /// <summary>
        /// This returns the unused source if there is any, if not creates a new one
        /// </summary>
        private AudioSource GetSource()
        {
            foreach (AudioSource source in sources)
            {
                if (source != null)
                {
                    if (!source.isPlaying)
                    {
                        source.loop = false;
                        return source;
                    }
                }
            }
            return CreateSource();
        }

        private AudioSource CreateSource()
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            sources.Add(newSource);
            return newSource;
        }

        /// <summary>
        /// Destroys the unused sources, optimizing the amount of audio sources
        /// </summary>
        public void DestroySources()
        {
            List<AudioSource> unusedSources = new List<AudioSource>();
            foreach (AudioSource source in sources)
            {
                if (!source.isPlaying)
                {
                    unusedSources.Add(source);
                }
            }
            foreach (AudioSource unusedSource in unusedSources)
            {
                sources.Remove(unusedSource);
                Destroy(unusedSource);
            }
        }
    }
}