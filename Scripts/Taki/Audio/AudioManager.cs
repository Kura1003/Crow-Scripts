using AnnulusGames.LucidTools.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Serializable]
        private struct AudioSetting
        {
            public AudioClip Clip;
            public AnnulusGames.LucidTools.Audio.AudioType Type;
        }

        [SerializeField] private List<AudioSetting> _audioSettings = new List<AudioSetting>();

        private Dictionary<string, AudioClip> _audioClipDictionary;
        private Dictionary<string, AnnulusGames.LucidTools.Audio.AudioType> _audioTypeDictionary;

        private AudioPlayer _currentBgmPlayer;

        private bool _isVolumeMuted = false;

        public bool IsVolumeMuted => _isVolumeMuted;

        public AudioPlayer CurrentBgmPlayer
        {
            get => _currentBgmPlayer;
            set => _currentBgmPlayer = value;
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeAudioClips();
        }

        private void InitializeAudioClips()
        {
            _audioClipDictionary = _audioSettings.ToDictionary(
                setting => Path.GetFileNameWithoutExtension(setting.Clip.name),
                setting => setting.Clip
            );

            _audioTypeDictionary = _audioSettings.ToDictionary(
                setting => Path.GetFileNameWithoutExtension(setting.Clip.name),
                setting => setting.Type
            );
        }

        public AudioPlayer Play(string clipName, GameObject linkObject)
        {
            AudioPlayer player = null;
            if (_audioClipDictionary.TryGetValue(clipName, out var clip) &&
                _audioTypeDictionary.TryGetValue(clipName, out var type))
            {
                switch (type)
                {
                    case AnnulusGames.LucidTools.Audio.AudioType.BGM:
                        player = LucidAudio.PlayBGM(clip);
                        _currentBgmPlayer = player;
                        break;
                    case AnnulusGames.LucidTools.Audio.AudioType.SE:
                        player = LucidAudio.PlaySE(clip);
                        break;
                    default:
                        player = LucidAudio.PlaySE(clip);
                        break;
                }
            }

            return player?.SetLink(linkObject, AudioLinkBehaviour.StopOnDisable);
        }

        public void ToggleMasterVolume()
        {
            _isVolumeMuted = !_isVolumeMuted;

            if (_isVolumeMuted)
            {
                LucidAudio.BGMVolume = 0f;
                LucidAudio.SEVolume = 0f;
            }

            else
            {
                LucidAudio.BGMVolume = 1f;
                LucidAudio.SEVolume = 1f;
            }
        }
    }
}