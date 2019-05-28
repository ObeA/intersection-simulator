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

        private CommunicationsManager _communicationsManager;

        public CommunicationsManager CommunicationsManager =>
            _communicationsManager != null ? _communicationsManager : (_communicationsManager = ResolveCommunicationManager());

        private CommunicationsManager ResolveCommunicationManager()
        {
            return communicationsManager != null 
                ? communicationsManager 
                : FindObjectOfType<CommunicationsManager>();
        }

        public bool IsReady => CommunicationsManager != null && CommunicationsManager.IsInitialized && Topic != null;
    }
}