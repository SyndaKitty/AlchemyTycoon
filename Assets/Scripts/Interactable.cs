﻿using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Configuration
    public bool Clickable;
    public bool Draggable;
    public bool DropReceiver;

    // Runtime
    SpriteRenderer sr;
    List<IDragReceiver> DragReceivers = new List<IDragReceiver>();
    Transform previousParent;

    public int Layer => SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
    public int Order => sr.sortingOrder;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

    }

    public bool CanReceive(GameObject obj)
    {
        if (!DropReceiver) return false;
        foreach (var receiver in DragReceivers)
        {
            if (receiver.CanReceive(obj))
            {
                return true;
            }
        }
        return false;
    }

    public void SetEnabled(bool enabled)
    {
        sr.enabled = enabled;

    }

    public void Register(IDragReceiver receiver)
    {
        DragReceivers.Add(receiver);
    }
    
    public delegate void ClickDown();
    public event ClickDown OnClickDown;
    public void InvokeClickDown() => OnClickDown?.Invoke();

    public delegate void ClickHold(float totalTime);
    public event ClickHold OnClickHold;
    public void InvokeClickHold(float totalTime) => OnClickHold?.Invoke(totalTime);

    public delegate void ClickRelease(float totalTime);
    public event ClickRelease OnClickRelease;
    public void InvokeClickRelease(float totalTime) => OnClickRelease?.Invoke(totalTime);

    public delegate void StartDrag();
    public event StartDrag OnStartDrag;
    public void InvokeStartDrag()
    {
        //previousParent = transform.parent;
        //transform.parent = ClickController.Instance.gameObject.transform;
        OnStartDrag?.Invoke();
    } 

    public delegate void Drag(Interactable over);
    public event Drag OnDrag;
    public void InvokeDrag(Interactable over) => OnDrag?.Invoke(over);

    public delegate void Drop(GameObject self, Interactable on);
    public event Drop OnDrop;
    public void InvokeDrop(GameObject self, Interactable on)
    {
        //transform.parent = previousParent;
        //previousParent = null;
        OnDrop?.Invoke(self, on);
    } 

    public delegate void Receive(GameObject obj);
    public event Receive OnReceive;
    public void InvokeReceive(GameObject obj) => OnReceive?.Invoke(obj);
}