using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EventMenu : Menu
{
    [SerializeField]
    public TMP_Text EventText;

    [SerializeField]
    public float TextPeriod;

    [SerializeField]
    private Queue<BattleAction> _queue = new Queue<BattleAction>();

    public override string GetName() {return "EventMenu";}

    public override void OnClose()
    {
    }

    public override void OnOpen() 
    {

    }
    public void ClearBattleActions()
    {
        _queue.Clear();
    }

    public void PushBattleAction(BattleAction battleEvent)
    {
        _queue.Enqueue(battleEvent);
    }

    public BattleAction PopBattleAction() 
    {
        if(_queue.Count > 0) 
        { 
            return _queue.Dequeue();
        }

        return null;
    }

    public IEnumerator FlushAndPresentBattleActionQueue(Action callback = null)
    {
        while(_queue.Count > 0)
        { 
            BattleAction battleEvent = _queue.Dequeue();

            yield return StartCoroutine(DisplayBattleAction(battleEvent, TextPeriod));

            foreach (var detail in battleEvent.Details)
            {
                yield return StartCoroutine(DisplayDetail(detail, TextPeriod));
            }
    	}

        EventText.text = "...";

        if (callback != null)
        {
            callback.Invoke();
        }
    }

    private IEnumerator DisplayBattleAction(BattleAction evt, float duration)
    {
        EventText.text = evt.ToString();

        float elapsedTime = 0;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
    	}
    }
    private IEnumerator DisplayDetail(string detail, float duration)
    {
        EventText.text = detail;

        float elapsedTime = 0;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
    	}
    }
}
