using Jagi.Interface;
using Jagi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace JagiWebSample.Models
{
    public enum ActionType
    {
        Create,
        Read,
        Update,
        Delete,
    }

    public class DataAccessLog
    {
        private string _actionDescription;
        private const string _descriptionFormat = "{0} for {1}.{2};"; 

        public int Id { get; set; }
        public string UserName { get; set; }
        public int? PatientId { get; set; }
        public string ActionDescription { get { return _actionDescription; } set { _actionDescription = value; } }
        public DateTime AccessDate { get; set; }

        public void SetEntityDescription(DbEntityEntry dbEntity, ActionType action)
        {
            object obj = dbEntity.CurrentValues.ToObject();     // 已經修改的資料
            object originObj = dbEntity.OriginalValues.ToObject();  // 原始的資料
            SetJagiEntityDescription(obj, action);
        }

        public void SetJagiEntityDescription(object obj, ActionType action)
        {
            if (!(obj is Entity))
                return;

            if (obj is IPatient)
            {
                this.PatientId = ((IPatient)obj).PatientId;
            }
            if (obj is Patient)
                this.PatientId = ((Patient)obj).Id;

            Entity entity = (Entity)obj;
            SetProperties(entity, action, entity.Id);
        }

        public void SetObjectDescription(object obj, ActionType action, string pkField)
        {
            string fullName = obj.GetType().Name;
            var value= obj.GetType().GetProperty(pkField).GetValue(obj, null);
            SetProperties(obj, action, value);
        }

        private void SetProperties(object entity, ActionType action, object pkValue)
        {
            string fullName = entity.GetType().Name;
            string description = _descriptionFormat
                .FormatWith(Enum.GetName(typeof(ActionType), action), fullName, pkValue);
            _actionDescription += description;
        }
    }
}