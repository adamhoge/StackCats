using System;
using System.Collections.Generic;

namespace Tofuwu.StackCats.Data
{
    [Serializable]
    public class VisitorData
    {
        public Guid VisitorGuid;
        public string CatId;
        public DateTime LeaveDateTime;
    }

    public interface IHomeData
    {
        string CurrentWallpaperId { get; set; }
        string CurrentFloorId { get; set; }
        string CurrentWindowId { get; set; }
        string CurrentDresserId { get; set; }
        string GetDresserObjectId(int index);
        void SetDresserObjectId(int index, string dresserObjectItemId, DateTime? placedDateTime = null, DateTime? activatedDateTime = null);
        DateTime GetDresserObjectPlacedDateTime(string placeableObjectId);
        DateTime GetDresserObjectActivatedDateTime(int index);
        DateTime GetDresserObjectActivatedDateTime(string placeableObjectId);
        void ActivateDresserObjectId(string dresserObjectItemId, DateTime activatedDateTime);
        void AddInvitedCatId(string catId);
        void RemoveInvitedCatId(string catId);
        List<string> GetInvitedCatIds();
        void AddVisitor(VisitorData visitor);
        void RemoveVisitor(Guid visitorId);
        List<VisitorData> GetVisitors();
    }
}