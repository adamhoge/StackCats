using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    [DataContract]
    public class LocalTutorialData : LocalData<LocalTutorialData>, ITutorialData
    {
        public TutorialSceneState TutorialState { get { return _tutorialState; } set { _tutorialState = value; Save(); } }

        public LocalTutorialData(string dataPath) : base(dataPath) { }

        [DataMember] private TutorialSceneState _tutorialState;
        [DataMember] private List<string> _typesKnown = new List<string>();

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            if (_typesKnown == null)
            {
                _typesKnown = new List<string>();
            }
        }

        public void SetTypeKnown(string typeName)
        {
            if (!_typesKnown.Contains(typeName))
            {
                _typesKnown.Add(typeName);
            }

            Save();
        }

        public bool IsTypeKnown(string typeName)
        {
            return _typesKnown.Contains(typeName);
        }
    }
}