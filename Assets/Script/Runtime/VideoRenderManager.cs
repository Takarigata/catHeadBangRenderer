using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[System.Serializable]
public struct BPMChange
{
    
    [SerializeField]
    public float seconds;

    [SerializeField]
    public int BPM;
    
    [SerializeField]
    public bool done;

    public BPMChange (int startBPM)
    {
        seconds = 0;
        BPM = 117;
        done = false;
    }
}

[RequireComponent(typeof(AudioSource))]
public class VideoRenderManager : MonoBehaviour
{
    #region Field

    [Header("BPM Settings")]
    [SerializeField]
    private List<BPMChange> _BpmChanges = new List<BPMChange>();
    
    [SerializeField]
    private int _BaseCatBPM = 117;
    
    [SerializeField]
    private int _TargetCatBPM = 117;
    
    

    [Space(10)]
    [Header("Video Settings")]
    [SerializeField]
    private VideoPlayer _CatVideoPlayer;
    
    [SerializeField]
    private VideoPlayer _BackgroundVideoPlayer;
    
    [SerializeField]
    private float _startTime;
    
    [SerializeField]
    private float _catDelay;
    
    [SerializeField]
    private Vector2 _catOffset;
    
    [SerializeField]
    private VideoClip _BackgroundVideo;
    
    [Space(10)]
    [Header("Audio Settings")]
    [SerializeField]
    private bool _UseCustomAudioClip = false;
    
    [SerializeField]
    private AudioClip _CustomAudioClip;
    
    private AudioSource _customAudioSource;

    private bool _CheckBPMChangeList;
    
    [Space(10)]
    [Header("Audio Settings")]
    
    [SerializeField]
    private float _currentTime = 0;
    
    [Space(10)]
    [Header("Rendering Settings")]
    
    [SerializeField]
    private RawImage _renderer;
    
    
    #endregion

    
    public void Start()
    {
        OffsetCatTexture();
        _currentTime = _startTime;
        _customAudioSource = GetComponent<AudioSource>();
        if (_CatVideoPlayer)
        {
            _CatVideoPlayer.time = _catDelay;
            _CatVideoPlayer.playbackSpeed = CalculateCatVideoSpeed(_TargetCatBPM);
            _CatVideoPlayer.Play();
        }

        if (_BackgroundVideoPlayer)
        {
            _BackgroundVideoPlayer.clip = _BackgroundVideo;
            if (_UseCustomAudioClip)
            {
                _BackgroundVideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                _customAudioSource.clip = _CustomAudioClip;
                _customAudioSource.Play();
            }
            _BackgroundVideoPlayer.time = _startTime;
            _BackgroundVideoPlayer.Play();
            
        }
        _CheckBPMChangeList = (_BpmChanges.Count > 0);
    }

    void OffsetCatTexture()
    {
        if (_renderer)
        {
            _renderer.material.SetTextureOffset("_MainTex", _catOffset);
        }
    }
    private float CalculateCatVideoSpeed(int BPM)
    {
        return (float) BPM / _BaseCatBPM;
    }

    void FindClosestStruc()
    {
        List<float> tmpSeconds = new List<float>();
        for(int i = 0; i < _BpmChanges.Count; i++ )
        {
            BPMChange tmp = _BpmChanges[i];
            if (!tmp.done && _currentTime >= tmp.seconds)
            {
                tmp.done = true;
                _BpmChanges[i] = tmp;
                ReCalculateBPM(tmp.BPM);
            }
        }
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        //Degeu
        FindClosestStruc();
    }

    void ReCalculateBPM(int BPM)
    {
        _CatVideoPlayer.playbackSpeed = CalculateCatVideoSpeed(BPM);
    }
}
