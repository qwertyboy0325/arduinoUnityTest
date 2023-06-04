using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System;

public enum EVideo
{
    Idle,
    Introduce,
    FriendQuestion,
    CoupleQuestion,
    StrangerQuestion,
    FirstLevel,
    SecondLevel,
    ThirdLevel
}
public class VideoController : MonoBehaviour
{
    public Video[] videos;
    [System.Serializable]
    public struct Video
    {
        public string title;
        public VideoClip videoSource;
    }
    public VideoPlayer videoPlayer;
    private EVideo currentPlaying;

    private VideoStatus videoStatus;

    public EVideo playingVideo
    {
        get { return currentPlaying; }
        set { currentPlaying = value; }
    }

    public static event System.Action<VideoStatus> OnVideoStateChanged;
    private void Awake()
    {
        QuestionManager.OnQuestionChanged += OnQuestionChanged;
    }
    private void OnDestroy()
    {
        QuestionManager.OnQuestionChanged -= OnQuestionChanged;
    }
    void Start()
    {
        currentPlaying = EVideo.Idle;
        // 初始化VideoPlayer
        videoPlayer = gameObject.GetComponent<VideoPlayer>();


        PlayVideo(currentPlaying);
    }

    // Update is called once per frame
    void Update()
    {
        // When video play end
        if ((videoPlayer.frame) > 0 && (videoPlayer.isPlaying == false))
        {
            videoPlayer.Stop();

            UpdatePlayingState(currentPlaying, 1);
            OnVideoEnd();
        }
    }

    private void OnVideoEnd()
    {
        switch (currentPlaying)
        {
            case EVideo.Idle:
                videoPlayer.Stop();
                videoPlayer.Play();
                break;
            case EVideo.Introduce:
                GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
                // QuestionVideoSwitch();

                break;
            case EVideo.FriendQuestion:
                if (QuestionManager.Instance.roundCount >= QuestionManager.Instance.relation.maxQuestionLimit)
                {
                    GameManager.Instance.UpdateGameState(EGameState.FinishAnswer);
                    break;
                }
                //GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
                QuestionNextStep(currentPlaying);
                break;
            case EVideo.CoupleQuestion:
                if (QuestionManager.Instance.roundCount >= QuestionManager.Instance.relation.maxQuestionLimit)
                {
                    GameManager.Instance.UpdateGameState(EGameState.FinishAnswer);
                    break;
                }
                //GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
                QuestionNextStep(currentPlaying);
                break;
            case EVideo.StrangerQuestion:
                if (QuestionManager.Instance.roundCount >= QuestionManager.Instance.relation.maxQuestionLimit)
                {
                    GameManager.Instance.UpdateGameState(EGameState.FinishAnswer);
                    break;
                }
                //GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
                QuestionNextStep(currentPlaying);
                break;
            case EVideo.FirstLevel:
                break;
            case EVideo.SecondLevel:
                break;
            case EVideo.ThirdLevel:
                break;
        }
    }

    private void QuestionNextStep(EVideo video)
    {
        uint roundCount = QuestionManager.Instance.roundCount;
        int maxCount = QuestionManager.Instance.relation.maxQuestionLimit;
        if (roundCount >= maxCount - 1)
        {
            GameManager.Instance.UpdateGameState(EGameState.FinishAnswer);
            return;
        }
        // Next Question case:
        QuestionManager.Instance.NextQuestion();
    }

    private void QuestionVideoSwitch()
    {
        switch (QuestionManager.Instance.selectedRelation)
        {
            case ERelation.Friend:
                PlayVideo(EVideo.FriendQuestion);
                break;
            case ERelation.Couple:
                PlayVideo(EVideo.CoupleQuestion);
                break;
            case ERelation.Stranger:
                PlayVideo(EVideo.StrangerQuestion);
                break;
            case ERelation.None:
                break;
        }
    }

    public void PlayVideo(EVideo state)
    {
        try
        {
            videoPlayer.Stop();
            // Set video resource route
            videoPlayer.clip = videos[(int)state].videoSource;
            // Play the video
            videoPlayer.Play();
            playingVideo = state;

            UpdatePlayingState(currentPlaying, 0);
            //Debug.Log("Playing: " + videos[(int)state].title + (int)state);
        }
        // Avoid index out of bound
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void UpdatePlayingState(EVideo playing, int isEnd)
    {
        // Debug.Log("isEnd: " + isEnd);
        videoStatus = new VideoStatus(playing, isEnd);
        OnVideoStateChanged?.Invoke(videoStatus);
    }
    private void OnQuestionChanged(string question)
    {
        GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
    }
}
public struct VideoStatus
{
    public VideoStatus(EVideo EV, int CS)
    {
        currentVideo = EV;
        currentState = CS;
    }
    EVideo currentVideo;
    int currentState;
}