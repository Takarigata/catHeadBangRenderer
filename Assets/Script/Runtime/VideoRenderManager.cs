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
    
    Dictionary <float, BPMChange> _bpmDico = new Dictionary<float, BPMChange>();
    
    private BPMChange currentBPM = new BPMChange();
    
    [SerializeField]
    private int _BaseCatBPM = 117;
    
    [SerializeField]
    private int _TargetCatBPM = 117;
    
    

    [Space(10)]
    [Header("Video Settings")]
    [SerializeField]
    private VideoPlayer _CatVideoPlayer = null;
    
    [SerializeField]
    private VideoPlayer _BackgroundVideoPlayer = null;
    
    [SerializeField]
    private float _startTime = 0;
    
    [SerializeField]
    private float _catDelay = 0;
    
    [SerializeField]
    private Vector2 _catOffset = new Vector2(0,0);
    
    [SerializeField]
    private VideoClip _BackgroundVideo = null;
    
    [Space(10)]
    [Header("Audio Settings")]
    [SerializeField]
    private bool _UseCustomAudioClip = false;

    [SerializeField] 
    private AudioClip _CustomAudioClip = null;
    
    private AudioSource _customAudioSource = null;

    private bool _CheckBPMChangeList;
    
    [Space(10)]
    [Header("Audio Settings")]
    
    [SerializeField]
    private float _currentTime = 0;
    
    [Space(10)]
    [Header("Rendering Settings")]
    
    [SerializeField]
    private RawImage _renderer = null;
    
    
    #endregion

    
    public void Start()
    {
        PopulateDictionnary();
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

    void PopulateDictionnary()
    {
        foreach (BPMChange tmp in _BpmChanges)
        {
            _bpmDico.Add(tmp.seconds, tmp);
        }
        FindNextItem();
    }

    void FindNextItem()
    {
        List<float> tmpSeconds = new List<float>();
        List<float> keyList = new List<float>(this._bpmDico.Keys);
        
        for(int i = 0; i < keyList.Count; i++ )
        {
            BPMChange tmp = _bpmDico[keyList[i]];
            if (!tmp.done)
            {
                tmpSeconds.Add(tmp.seconds);
            }
        }
        _bpmDico.TryGetValue(tmpSeconds.Min(), out currentBPM);

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
        if (_bpmDico.Count > 0)
        {
            CheckForNextItem();
        }
    }

    void CheckForNextItem()
    {
        if (_currentTime > currentBPM.seconds)
        {
            currentBPM.done = true;
            ReCalculateBPM(currentBPM.BPM);
            _bpmDico.Remove(currentBPM.seconds);
            if (_bpmDico.Count > 0)
            {
                FindNextItem();
            }
        }
    }
    void ReCalculateBPM(int BPM)
    {
        _CatVideoPlayer.playbackSpeed = CalculateCatVideoSpeed(BPM);
    }
    
}
