using JagiWebSample.Models;
using System.Collections.Generic;

namespace JagiWebSample.Areas.Admin.Models
{
    public class DetailsViewModel
    {
        #region StatusEnum enum

        public enum StatusEnum
        {
            Offline,
            Online,
            LockedOut,
            Unapproved
        }

        #endregion

        public string DisplayName { get; set; }
        public StatusEnum Status { get; set; }
        public ApplicationUser User { get; set; }
        public IDictionary<string, bool> Roles { get; set; }
    }
}