using System;
using UnityEngine;
using UnityEngine.Events;

namespace Standard_Library
{
    [Serializable]
    public abstract class BlockRunner
    {
        [field:SerializeField] protected string blockName { get; private set;}
        [field: SerializeField] protected bool counterbalanceByParticipantNumber { get; private set;}
        public readonly UnityEvent onBlockComplete = new UnityEvent();
        public abstract void RunBlock(bool logData);
        public abstract void InitBlock();
    }
}