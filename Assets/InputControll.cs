using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControll : MonoBehaviour
{
    public AudioRecorder audioRecorder;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            QuestionManager.Instance.InitFlow(ERelation.Friend);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            QuestionManager.Instance.InitFlow(ERelation.Couple);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            QuestionManager.Instance.InitFlow(ERelation.Stranger);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            EGameState state = GameManager.Instance.state;
            switch (state)
            {
                case EGameState.Idle:
                    break;
                case EGameState.Introduce:
                    GameManager.Instance.UpdateGameState(EGameState.PutQuestion);
                    break;
                case EGameState.PutQuestion:
                    GameManager.Instance.UpdateGameState(EGameState.AwaitAnswer);
                    break;
                case EGameState.AwaitAnswer:
                    AwaitAnswerHandler();
                    break;
                case EGameState.FinishAnswer:
                    GameManager.Instance.UpdateGameState(EGameState.ReviewAnswer);
                    break;
                case EGameState.ReviewAnswer:
                    GameManager.Instance.UpdateGameState(EGameState.Ending);
                    break;
                case EGameState.Ending:
                    GameManager.Instance.UpdateGameState(EGameState.Idle);
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            QuestionManager.Instance.RedrawQuestion();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            audioRecorder.StartRecording();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            audioRecorder.StopRecording();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            AudioClip audioAssets = audioRecorder.GetRecordedAsset();

            //Debug.Log(audioAssets);
            audioRecorder.audioSource.clip = audioAssets;
            audioRecorder.audioSource.Play();
        }
    }
    private void AwaitAnswerHandler()
    {
        // StopCoroutine(currentPutQuestionCoroutine);
        if (!QuestionManager.Instance.IsCompletedQuestion())
        {
            QuestionManager.Instance.RedrawQuestion();
            return;
        }
        uint roundCount = QuestionManager.Instance.roundCount;
        int maxCount = QuestionManager.Instance.relation.maxQuestionLimit;
        Debug.Log(roundCount);
        if (roundCount >= maxCount - 1)
        {
            GameManager.Instance.UpdateGameState(EGameState.FinishAnswer);
            return;
        }
        // Next Question case:
        QuestionManager.Instance.NextQuestion();
    }
}
