using UnityEngine;

namespace Intersection
{
    public class CommunicationMonoBehaviour : MonoBehaviour
    {
        public CommunicationsManager communicationsManager;
        public string topic;

        private LogicGroup _logicGroup;
        public LogicGroup ParentLogicGroup => _logicGroup != null 
            ? _logicGroup 
            : _logicGroup = (transform.parent != null ? transform.parent.GetComponent<LogicGroup>() : null);
        public string Topic => ParentLogicGroup != null && ParentLogicGroup.Topic != null 
            ? $"{ParentLogicGroup.Topic}{topic}" 
            : topic;
    }
}