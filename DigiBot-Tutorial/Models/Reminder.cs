using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiBot_Tutorial.Models
{
    [Serializable]
    public class Reminder : IEquatable<Reminder>
    {
        public string What { get; set; }
        public DateTime When { get; set; }

        public override string ToString()
        {
            return $"[{this.What} at {this.When}]";
        }

        public bool Equals(Reminder other)
        {
            return other != null
                && this.When == other.When
                && this.What == other.What;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Reminder);
        }

        public override int GetHashCode()
        {
            return this.What.GetHashCode();
        }
    }
}