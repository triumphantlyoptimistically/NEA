using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Technical_Solution
{
    internal static class DatabaseHelper
    {
        public static void EnsureRoutesTableExists(SQLiteConnection conn)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Routes (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Distance REAL, RouteImage BLOB, Difficulty TEXT)", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void EnsureRunnersTableExists(SQLiteConnection conn)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Runners (RunnerID INTEGER PRIMARY KEY AUTOINCREMENT, Forename TEXT, Surname TEXT, Age INTEGER, Gender TEXT);", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void EnsureEventsTableExists(SQLiteConnection conn)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Events (EventID INTEGER PRIMARY KEY AUTOINCREMENT, RouteID INTEGER, Day TEXT, Time TEXT, Location TEXT, FOREIGN KEY(RouteID) REFERENCES Routes(ID));", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void EnsureParticipantsTableExists(SQLiteConnection conn)
        {
            using (SQLiteCommand cmd = new SQLiteCommand("CREATE TABLE IF NOT EXISTS Participants (EventID INTEGER NOT NULL, RunnerID INTEGER NOT NULL, FinishTime TIME, PRIMARY KEY (EventID, RunnerID), FOREIGN KEY(EventID) REFERENCES Events(EventID), FOREIGN KEY(RunnerID) REFERENCES Runners(RunnerID));", conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddRoute(SQLiteConnection conn, string routeName, double distance, string difficulty, byte[] routeImage)
        {
            EnsureRoutesTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Routes (Name, Distance, RouteImage, Difficulty) VALUES (@name, @distance, @routeImage, @difficulty)", conn))
            {
                cmd.Parameters.AddWithValue("@name", routeName);
                cmd.Parameters.AddWithValue("@distance", distance);
                cmd.Parameters.AddWithValue("@routeImage", routeImage);
                cmd.Parameters.AddWithValue("@difficulty", difficulty);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Route> GetRoutes(SQLiteConnection conn)
        {
            EnsureRoutesTableExists(conn);

            List<Route> routes = new List<Route>();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT ID, Name, Difficulty, RouteImage FROM Routes", conn))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = int.Parse(reader["ID"].ToString());
                    string name = reader["Name"].ToString();
                    string difficulty = reader["Difficulty"].ToString();
                    byte[] imageBytes = reader["RouteImage"] == DBNull.Value ? null : (byte[])reader["RouteImage"];

                    routes.Add(new Route(id, name, difficulty, imageBytes));
                }
            }

            return routes;
        }

        public static int DeleteRoute(SQLiteConnection conn, int routeId)
        {
            EnsureRoutesTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Routes WHERE ID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", routeId);
                return cmd.ExecuteNonQuery();
            }
        }

        public static void AddRunner(SQLiteConnection conn, string forename, string surname, int age, string gender)
        {
            EnsureRunnersTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Runners (Forename, Surname, Age, Gender) VALUES (@forename, @surname, @age, @gender)", conn))
            {
                cmd.Parameters.AddWithValue("@forename", forename);
                cmd.Parameters.AddWithValue("@surname", surname);
                cmd.Parameters.AddWithValue("@age", age);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Runner> GetRunners(SQLiteConnection conn)
        {
            EnsureRunnersTableExists(conn);

            List<Runner> runners = new List<Runner>();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT Forename, Surname, Age, Gender FROM Runners ORDER BY RunnerID", conn))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    runners.Add(new Runner(
                        reader["Forename"].ToString(),
                        reader["Surname"].ToString(),
                        int.Parse(reader["Age"].ToString()),
                        reader["Gender"].ToString()
                    ));
                }
            }

            return runners;
        }

        public static List<int> GetRunnerIds(SQLiteConnection conn)
        {
            EnsureRunnersTableExists(conn);

            List<int> runnerIds = new List<int>();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT RunnerID FROM Runners ORDER BY RunnerID", conn))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    runnerIds.Add(int.Parse(reader["RunnerID"].ToString()));
                }
            }

            return runnerIds;
        }

        public static int DeleteRunner(SQLiteConnection conn, int runnerId)
        {
            EnsureRunnersTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Runners WHERE RunnerID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", runnerId);
                return cmd.ExecuteNonQuery();
            }
        }

        public static bool DoesRouteIdExist(SQLiteConnection conn, int routeId)
        {
            EnsureRoutesTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(ID) FROM Routes WHERE ID = @routeID", conn))
            {
                cmd.Parameters.AddWithValue("@routeID", routeId);
                int count = int.Parse(cmd.ExecuteScalar().ToString());
                return count > 0;
            }
        }

        public static void AddEvent(SQLiteConnection conn, int routeId, string day, string time, string location)
        {
            EnsureEventsTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Events (RouteID, Day, Time, Location) VALUES (@routeID, @day, @time, @location)", conn))
            {
                cmd.Parameters.AddWithValue("@routeID", routeId);
                cmd.Parameters.AddWithValue("@day", day);
                cmd.Parameters.AddWithValue("@time", time);
                cmd.Parameters.AddWithValue("@location", location);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<Event> GetEvents(SQLiteConnection conn)
        {
            EnsureEventsTableExists(conn);

            List<Event> events = new List<Event>();
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT EventID, RouteID, Day, Time, Location FROM Events", conn))
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    events.Add(new Event(
                        int.Parse(reader["EventID"].ToString()),
                        int.Parse(reader["RouteID"].ToString()),
                        reader["Day"].ToString(),
                        reader["Time"].ToString(),
                        reader["Location"].ToString(),
                        null
                    ));
                }
            }

            return events;
        }

        public static int DeleteEvent(SQLiteConnection conn, int eventId)
        {
            EnsureEventsTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Events WHERE EventID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", eventId);
                return cmd.ExecuteNonQuery();
            }
        }

        public static bool DoesRunnerExist(SQLiteConnection conn, int runnerId)
        {
            EnsureRunnersTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(1) FROM Runners WHERE RunnerID = @runnerID", conn))
            {
                cmd.Parameters.AddWithValue("@runnerID", runnerId);
                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static bool DoesEventExist(SQLiteConnection conn, int eventId)
        {
            EnsureEventsTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(1) FROM Events WHERE EventID = @eventID", conn))
            {
                cmd.Parameters.AddWithValue("@eventID", eventId);
                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static void AddParticipant(SQLiteConnection conn, int eventId, int runnerId)
        {
            EnsureParticipantsTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Participants (EventID, RunnerID) VALUES (@eventID, @runnerID)", conn))
            {
                cmd.Parameters.AddWithValue("@eventID", eventId);
                cmd.Parameters.AddWithValue("@runnerID", runnerId);
                cmd.ExecuteNonQuery();
            }
        }

        public static void AddParticipantResult(SQLiteConnection conn, int eventId, int runnerId, string finishTime)
        {
            EnsureParticipantsTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("UPDATE Participants SET FinishTime = @finishTime WHERE EventID = @eventID AND RunnerID = @runnerID", conn))
            {
                cmd.Parameters.AddWithValue("@finishTime", finishTime);
                cmd.Parameters.AddWithValue("@eventID", eventId);
                cmd.Parameters.AddWithValue("@runnerID", runnerId);
                cmd.ExecuteNonQuery();
            }
        }

        public static string GetEventStartTime(SQLiteConnection conn, int eventId)
        {
            EnsureEventsTableExists(conn);
            using (SQLiteCommand cmd = new SQLiteCommand("SELECT Time FROM Events WHERE EventID = @eventID", conn))
            {
                cmd.Parameters.AddWithValue("@eventID", eventId);
                object value = cmd.ExecuteScalar();
                if (value == null)
                {
                    return "";
                }

                return value.ToString();
            }
        }

        public static double GetRouteDistanceForEvent(SQLiteConnection conn, int eventId)
        {
            EnsureEventsTableExists(conn);
            EnsureRoutesTableExists(conn);

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT Distance FROM Events JOIN Routes ON Events.RouteID = Routes.ID WHERE EventID = @eventID", conn))
            {
                cmd.Parameters.AddWithValue("@eventID", eventId);
                object value = cmd.ExecuteScalar();
                if (value == null)
                {
                    return 0;
                }

                return double.Parse(value.ToString());
            }
        }

        public static bool TryGetRunnerResult(SQLiteConnection conn, int runnerId, out string finishTime, out int eventId, out string difficulty)
        {
            finishTime = "";
            eventId = 0;
            difficulty = "";

            EnsureParticipantsTableExists(conn);
            EnsureEventsTableExists(conn);
            EnsureRoutesTableExists(conn);

            using (SQLiteCommand cmd = new SQLiteCommand("SELECT Participants.FinishTime, Events.EventID, Difficulty FROM Participants JOIN Events ON Participants.EventID = Events.EventID JOIN Routes ON Events.RouteID = Routes.ID WHERE Participants.RunnerID = @runnerID AND FinishTime IS NOT NULL", conn))
            {
                cmd.Parameters.AddWithValue("@runnerID", runnerId);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    finishTime = reader["FinishTime"].ToString();
                    eventId = int.Parse(reader["EventID"].ToString());
                    difficulty = reader["Difficulty"].ToString();
                    return true;
                }
            }
        }
    }
}