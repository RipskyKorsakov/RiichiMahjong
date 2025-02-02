using RMU.Players;
using System;

namespace RMU.Calls.PotentialCalls;

public sealed class PriorityQueueForPotentialCalls : IPriorityQueue
{
    private readonly List<PotentialCall> _priorityQueue;

    public PriorityQueueForPotentialCalls()
    {
        _priorityQueue = new List<PotentialCall>();
    }

    public bool IsEmpty()
    {
        return _priorityQueue.Count == 0;
    }

    private void AddPotentialCall(PotentialCall potentialCall)
    {
        if (_priorityQueue.Contains(potentialCall))
            return;
        for (int i = 0; i < _priorityQueue.Count; i++)
        {
            if (potentialCall.GetPriority() >= _priorityQueue[i].GetPriority())
            {
                _priorityQueue.Insert(i, potentialCall);
                return;
            }
        }
        _priorityQueue.Add(potentialCall);
    }

    public void AddCall(ICallObject callObject)
    {
        Type type = callObject.GetType();
        if (type.IsSubclassOf(typeof(PotentialCall)))
        {
            AddPotentialCall((PotentialCall)callObject);
            return;
        }

        throw new ArgumentException("Invalid type");
    }

    public void Clear()
    {
        _priorityQueue.Clear();
    }

    public void RemoveByPlayer(Player player)
    {
        for (int i = _priorityQueue.Count - 1; i >= 0; i--)
        {
            if (_priorityQueue[i].GetPlayerMakingCall() == player)
            {
                _priorityQueue.RemoveAt(i);
            }
        }
    }

    public void RemoveByPriority(int priority)
    {
        for (int i = _priorityQueue.Count - 1; i >= 0; i--)
        {
            if (_priorityQueue[i].GetPriority() <= priority && _priorityQueue[i].GetPriority() != 3)
            {
                PotentialCall call = _priorityQueue[i];
                switch (call.GetCallType())
                {
                    case PotentialCallType.Pon:
                        call.GetPlayerMakingCall().InvokeOnCanNoLongerPon();
                        break;
                    case PotentialCallType.HighChii:
                    case PotentialCallType.MidChii:
                    case PotentialCallType.LowChii:
                        call.GetPlayerMakingCall().InvokeOnCanNoLongerChii();
                        break;
                }
                _priorityQueue.RemoveAt(i);
            }
        }
    }

    public List<PotentialCall> GetCallsByPlayer(Player player)
    {
        List<PotentialCall> outputList = new();
        if (player.IsActivePlayer())
            return outputList;
        foreach (PotentialCall call in _priorityQueue)
        {
            if (call.GetPlayerMakingCall() == player)
            {
                outputList.Add(call);
            }
        }

        return outputList;
    }
}