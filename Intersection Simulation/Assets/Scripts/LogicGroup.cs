using System.Text;
using UnityEngine;

public class LogicGroup : MonoBehaviour
{
    public string topic;
    
    private LogicGroup _logicGroup;
    private bool _hasProbed = false;

    public LogicGroup ParentLogicGroup
    {
        get
        {
            if (_hasProbed)
            {
                return _logicGroup;
            }

            _hasProbed = true;
            return _logicGroup != null ? _logicGroup : _logicGroup = (transform.parent != null ? transform.parent.GetComponent<LogicGroup>() : null);
        }
    }
    public string Topic => ParentLogicGroup != null && ParentLogicGroup.Topic != null ? $"{ParentLogicGroup.Topic}{topic}" : topic;
}
