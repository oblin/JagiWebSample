using System;

namespace Jagi.Interface
{
    public abstract class Entity : IEntity
    {
        public int Id { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedUser { get; set; }
    }
}