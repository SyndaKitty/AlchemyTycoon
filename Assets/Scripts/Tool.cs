﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DropReceiver))]
public class Tool : MonoBehaviour, IDropReceiver
{
    const int WAITING = 0;
    const int PROCESSING = 1;

    // Configuration
    [EnumToggleButtons]
    public PhysicalState AcceptedPhysical;
    public float ProcessTime = 20;
    public List<AttributeAffector> Affectors;

    // Runtime
    int currentState;
    float currentProcessingProgress;
    Ingredient processingTarget;
    SpriteRenderer processingRender;

    public bool Receive(GameObject obj)
    {
        if (currentState == PROCESSING) return false;

        var ingredient = obj.GetComponent<Ingredient>();
        if (ingredient == null) return false;

        var renderer = obj.GetComponent<SpriteRenderer>();
        if ((ingredient.PhysicalState & AcceptedPhysical) == 0) return false;

        StartProcessing(ingredient, renderer);
        return true;
    }

    void Update()
    {
        if (currentState == PROCESSING)
        {
            currentProcessingProgress += Time.deltaTime;

            if (currentProcessingProgress >= ProcessTime)
            {
                FinishProcessing();
            }
        }
    }

    void StartProcessing(Ingredient ing, SpriteRenderer renderer)
    {
        processingTarget = ing;
        processingRender = renderer;

        renderer.enabled = false;
        currentState = PROCESSING;
    }

    void FinishProcessing()
    {
        foreach (var affector in Affectors)
        {
            if (processingTarget.Attributes.Count > affector.Index)
            {
                processingTarget.Attributes[affector.Index].Intensity += affector.Delta;
            }
        }

        processingRender.enabled = true;
        processingTarget.transform.position = transform.position + new Vector3(1, 0);

        currentProcessingProgress = 0;
        currentState = WAITING;
    }

    void Awake()
    {
        GetComponent<DropReceiver>().behaviour.Result = this;
    }
}

[Serializable]
public class AttributeAffector
{
    public int Index;
    public int Delta;
}