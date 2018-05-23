using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcWebTool.Core.DataAccess
{
    public abstract class Entity : IEntity
    {
        public string Id { get; set; }
        public override bool Equals(object obj)
        {
            if(obj.GetType() == this.GetType())
            {
                if (((IEntity)obj).Id == this.Id) return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ 236567421;
        }

        public override string ToString()
        {
            return $"Id = {Id}";
        }

        public bool IsTransient => string.IsNullOrEmpty(Id);
    }
}
