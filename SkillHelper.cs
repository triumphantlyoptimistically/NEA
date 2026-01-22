using System;
using System.Data.SQLite;
using System.Globalization;

namespace Technical_Solution
{
    internal static class SkillHelper
    {
        public static string GetRunnerSkill(SQLiteConnection conn, int runnerId)
        {
            string finishTime;
            int eventId;
            string difficulty;

            if (!DatabaseHelper.TryGetRunnerResult(conn, runnerId, out finishTime, out eventId, out difficulty))
            {
                return "Novice";
            }

            double pace = CalculatePace(conn, eventId, finishTime);

            if (difficulty == "Easy")
            {
                if (pace <= 5) return "Elite";
                if (pace <= 7) return "Advanced";
                if (pace <= 10) return "Intermediate";
                return "Novice";
            }
            else
            {
                if (pace <= 6) return "Elite";
                if (pace <= 8) return "Advanced";
                if (pace <= 12) return "Intermediate";
                return "Novice";
            }
        }

        private static double CalculatePace(SQLiteConnection conn, int eventId, string finishTime)
        {
            TimeSpan timeTaken = CalculateTimeTaken(conn, eventId, finishTime);
            double distance = DatabaseHelper.GetRouteDistanceForEvent(conn, eventId);

            if (distance <= 0)
            {
                return 0;
            }

            return timeTaken.TotalMinutes / distance;
        }

        private static TimeSpan CalculateTimeTaken(SQLiteConnection conn, int eventId, string finishTime)
        {
            string startTimeText = DatabaseHelper.GetEventStartTime(conn, eventId);
            if (string.IsNullOrWhiteSpace(startTimeText))
            {
                return TimeSpan.Zero;
            }

            TimeSpan startTime = TimeSpan.Parse(startTimeText, CultureInfo.InvariantCulture);
            TimeSpan endTime = TimeSpan.Parse(finishTime, CultureInfo.InvariantCulture);

            if (endTime < startTime)
            {
                endTime = endTime.Add(TimeSpan.FromDays(1));
            }

            return endTime - startTime;
        }
    }
}