using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class LocalHomeData : LocalData<LocalHomeData>, IHomeData
    {
        public string CurrentWallpaperId { get => _currentWallpaperId; set { _currentWallpaperId = value; Save(); } }

        public string CurrentFloorId { get => _currentFloorId; set { _currentFloorId = value; Save(); } }

        public string CurrentWindowId { get => _currentWindowId; set { _currentWindowId = value; Save(); } }

        public string CurrentDresserId { get => _currentDresserId; set { _currentDresserId = value; Save(); } }

        private string _currentWallpaperId;
        private string _currentFloorId;
        private string _currentWindowId;
        private string _currentDresserId;
        private LocalPlacedObjectData[] _dresserObjects;
        private List<string> _invitedCatIds;
        private List<VisitorData> _visitors;

        public LocalHomeData(string dataPath, int dresserObjectsSize) : base(dataPath)
        {
            _dresserObjects = new LocalPlacedObjectData[dresserObjectsSize];
            _invitedCatIds = new List<string>();
            _visitors = new List<VisitorData>();

            Save();
        }

        public string GetDresserObjectId(int index)
        {
            if (index < 0 || index >= _dresserObjects.Length) return null;

            return _dresserObjects[index] != null ? _dresserObjects[index].PlaceableObjectItemId : null;
        }

        public void SetDresserObjectId(int index, string dresserObjectId, DateTime? placedDateTime = null, DateTime? activatedDateTime = null)
        {
            if (index > -1 && index < _dresserObjects.Length)
            {
                if (string.IsNullOrEmpty(dresserObjectId))
                {
                    _dresserObjects[index] = null;
                }
                else
                {
                    if (placedDateTime == null) placedDateTime = DateTime.Now;
                    if (activatedDateTime == null) activatedDateTime = placedDateTime;

                    _dresserObjects[index] = new LocalPlacedObjectData
                    {
                        PlaceableObjectItemId = dresserObjectId,
                        PlacedDateTime = placedDateTime.Value,
                        ActivatedDateTime = activatedDateTime.Value
                    };
                }

                Save();
            }
        }

        public void ActivateDresserObject(int index)
        {
            if (index < 0 || _dresserObjects[index] == null) return;

            _dresserObjects[index].ActivatedDateTime = DateTime.Now;

            Save();
        }

        public DateTime GetDresserObjectPlacedDateTime(string placeableObjectId)
        {
            LocalPlacedObjectData placedObject = _dresserObjects.FirstOrDefault(po => po != null && po.PlaceableObjectItemId == placeableObjectId);
            return placedObject != null ? placedObject.ActivatedDateTime : DateTime.MinValue;
        }

        public DateTime GetDresserObjectActivatedDateTime(int index)
        {
            LocalPlacedObjectData placedObject = _dresserObjects[index];
            return placedObject != null ? placedObject.ActivatedDateTime : DateTime.Now;
        }

        public DateTime GetDresserObjectActivatedDateTime(string placeableObjectId)
        {
            LocalPlacedObjectData placedObject = _dresserObjects.FirstOrDefault(po => po != null && po.PlaceableObjectItemId == placeableObjectId);
            return placedObject != null ? placedObject.ActivatedDateTime : DateTime.Now;
        }

        public void ActivateDresserObjectId(string dresserObjectItemId, DateTime activationDateTime)
        {
            LocalPlacedObjectData placedObject = _dresserObjects.FirstOrDefault(po => po != null && po.PlaceableObjectItemId == dresserObjectItemId);
            if (placedObject != null)
            {
                placedObject.ActivatedDateTime = activationDateTime;
               
                Save();
            }
        }

        public void AddInvitedCatId(string catId)
        {
            _invitedCatIds.Add(catId);
            
            Save();
        }

        public void RemoveInvitedCatId(string catId)
        {
            _invitedCatIds.Remove(catId);

            Save();
        }

        public List<string> GetInvitedCatIds()
        {
            return _invitedCatIds;
        }

        public void AddVisitor(VisitorData visitor)
        {
            _visitors.Add(visitor);
           
            Save();
        }

        public void RemoveVisitor(Guid visitorId)
        {
            _visitors.Remove(_visitors.FirstOrDefault(v => v.VisitorGuid == visitorId));
           
            Save();
        }

        public List<VisitorData> GetVisitors()
        {
            return _visitors;
        }
    }
}