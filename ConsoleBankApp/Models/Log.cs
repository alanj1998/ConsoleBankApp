using System;

namespace SSD.Models
{
    internal sealed class LogEntry
    {
        private static string Actor { get; set; } = "User";
        private static Roles Role { get; set; } = Roles.Unauthorized;
        private string Action { get; set; }
        private DateTime StartTime { get; set; }
        private DateTime EndTime { get; set; }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(Roles), Role)}, {Actor} performed {Action}, which started at {StartTime.ToLongDateString()} and finished at {EndTime.ToLongDateString()}";
        }

        internal LogEntry(string action, DateTime startTime)
        {
            this.Action = action;
            this.StartTime = startTime;
        }

        internal void AddEndTime(DateTime endTime)
        {
            this.EndTime = endTime;
        }

        internal static void SetActor(string actor, Roles role)
        {
            Actor = actor;
            Role = role;
        }
    }
}