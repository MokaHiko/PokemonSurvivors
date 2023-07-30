using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BattleEvent
{
    public BattleEvent(string msg, uint priority = 0)
    {
        message = msg;
    }

    public string message;
};

public class EventMenu : Menu
{
    [SerializeField]
    public TMP_Text EventText;

    [SerializeField]
    public float TextPeriod;

    [SerializeField]
    private Queue<BattleEvent> _queue;
    public override string GetName() {return "EventMenu";}

    private void Awake()
    {
        _queue = new Queue<BattleEvent>();
    }

    public override void OnClose()
    {

    }

    public override void OnOpen() 
    {

    }
    public void ClearBattleEvents()
    {
        _queue.Clear();
    }

    public void PushBattleEvent(BattleEvent battleEvent)
    {
        _queue.Enqueue(battleEvent);
    }

    public BattleEvent PopBattleEvent() 
    {
        if(_queue.Count > 0) 
        { 
            return _queue.Dequeue();
        }

        return null;
    }

    public IEnumerator FlushBattleEventQueue(Action callback = null)
    {
        float elaspedTime = 0;
        foreach (BattleEvent battleEvent in _queue)
        {
            while (elaspedTime < TextPeriod)
            {
                elaspedTime += Time.deltaTime;
                yield return null;
            }

            elaspedTime = 0;
            DisplayBattleEvent(battleEvent);
            yield return null;
        }
        ClearBattleEvents();

        if (callback != null)
        {
            callback.Invoke();
        }
    }

    private void DisplayBattleEvent(BattleEvent evt)
    {
        EventText.text = evt.message;
    }
}
