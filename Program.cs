using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Technical_Solution
{
    internal class Program
    {
        public static SQLiteConnection conn = new SQLiteConnection("Data Source=Running Event Manager.db;Version=3;New=True;Compress=True;");

        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                DisplayMenu();
                int optionChosen = GetOption(1, 4);

                if (optionChosen == 1)
                {
                    Console.Clear();
                    DisplayRoutesMenu();
                }
                else if (optionChosen == 2)
                {
                    Console.Clear();
                    DisplayRunnersMenu();
                }
                else if (optionChosen == 3)
                {
                    Console.Clear();
                    DisplayEventsMenu();
                }
                else if (optionChosen == 4)
                {
                    Console.Clear();
                    Environment.Exit(0);
                }
            }
        }

        static void DisplayMenu()
        {
            Console.WriteLine("Welcome to the Running Event Planner\n\nPlease choose one of the following options:\n\n\n  1. Manage Routes\n  2. Manage Runners\n  3. Manage Events\n  4. Exit");
            Console.CursorLeft = 0;
            Console.CursorTop = 5;
            Console.Write(">");
        }

        static void DisplayRoutesMenu()
        {
            Console.Clear();
            Console.WriteLine("Routes Management Menu\n\nPlease choose one of the following options:\n\n\n  1. Add a new route\n  2. View existing routes\n  3. Delete a route\n  4. Back to main menu");
            Console.CursorLeft = 0;
            Console.CursorTop = 5;
            Console.Write(">");

            switch (GetOption(1, 4))
            {
                case 1:
                    Console.Clear();
                    AddRoute();
                    break;
                case 2:
                    Console.Clear();
                    ViewRoutes();
                    break;
                case 3:
                    Console.Clear();
                    DeleteRoute();
                    break;
                case 4:
                    Console.Clear();
                    return;
            }
        }

        static void AddRoute()
        {
            string filePath = GetFilePath();

            try
            {
                using (Bitmap inputImage = (Bitmap)Image.FromFile(filePath))
                {
                    string routeName = GetRouteName();
                    double distance = GetRouteDistance();
                    string difficulty = GetRouteDifficulty();

                    Point start;
                    Point goal;

                    using (Bitmap thinForSnap = RouteImageHelper.CreateThinnedImage(inputImage))
                    using (RouteViewer viewer = new RouteViewer(routeName, inputImage))
                    {
                        start = viewer.GetClickedPoint(thinForSnap);
                        goal = viewer.GetClickedPoint(thinForSnap);
                    }

                    using (Bitmap routeMap = RouteHelper.CreateRouteMap(inputImage, difficulty, start, goal))
                    {
                        byte[] routeImage = RouteHelper.ConvertImageToBytes(routeMap);

                        conn.Open();
                        DatabaseHelper.AddRoute(conn, routeName, distance, difficulty, routeImage);
                        conn.Close();

                        Console.WriteLine("Route added successfully!");
                        System.Threading.Thread.Sleep(1000);
                        Console.Clear();
                    }
                }
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while adding route: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("The file you selected is not a valid image format.");
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("The file you selected could not be found.");
                System.Threading.Thread.Sleep(2000);
                Console.Clear();
            }
        }

        static void ViewRoutes()
        {
            try
            {
                conn.Open();
                List<Route> routes = DatabaseHelper.GetRoutes(conn);
                conn.Close();

                Console.WriteLine("Routes:\n");
                foreach (Route route in routes)
                {
                    Console.WriteLine($"RouteID: {route.GetID()} | Name: {route.GetName()} | Difficulty: {route.GetDifficulty()}\n");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while retrieving routes: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void DeleteRoute()
        {
            int routeID;
            while (true)
            {
                Console.Write("Enter the ID of the route to delete: ");
                if (int.TryParse(Console.ReadLine(), out routeID) && routeID > 0)
                {
                    break;
                }
                Console.WriteLine("Enter a positive integer for route ID");
            }

            try
            {
                conn.Open();
                int removed = DatabaseHelper.DeleteRoute(conn, routeID);
                conn.Close();

                if (removed == 0)
                {
                    Console.WriteLine("No route found with the given ID.");
                }
                else
                {
                    Console.WriteLine("Route deleted successfully!");
                }

                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while deleting route: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void DisplayRunnersMenu()
        {
            Console.Clear();
            Console.WriteLine("Runners Management Menu\n\nPlease choose one of the following options:\n\n\n  1. Add a new runner\n  2. View existing runners\n  3. Delete a runner\n  4. Back to main menu");
            Console.CursorLeft = 0;
            Console.CursorTop = 5;
            Console.Write(">");

            switch (GetOption(1, 4))
            {
                case 1:
                    Console.Clear();
                    AddRunner();
                    break;
                case 2:
                    Console.Clear();
                    ViewRunners();
                    break;
                case 3:
                    Console.Clear();
                    DeleteRunner();
                    break;
                case 4:
                    Console.Clear();
                    return;
            }
        }

        static void AddRunner()
        {
            Console.Clear();
            string forename = GetForename();
            string surname = GetSurname();
            int age = GetAge();
            string gender = GetGender();
            Console.WriteLine();
            Console.WriteLine();

            try
            {
                conn.Open();
                DatabaseHelper.AddRunner(conn, forename, surname, age, gender);
                conn.Close();

                Console.WriteLine("Runner added successfully!");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while adding runner: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ViewRunners()
        {
            try
            {
                conn.Open();
                List<int> runnerIds = DatabaseHelper.GetRunnerIds(conn);
                List<Runner> runners = DatabaseHelper.GetRunners(conn);
                conn.Close();

                Console.WriteLine("Runners:\n");
                for (int i = 0; i < runners.Count; i++)
                {
                    conn.Open();
                    string skill = SkillHelper.GetRunnerSkill(conn, runnerIds[i]);
                    conn.Close();

                    Runner runner = runners[i];
                    Console.WriteLine($"Runner ID: {runnerIds[i]} | Name: {runner.GetForename()} | Surname: {runner.GetSurname()} | Age: {runner.GetAge()} | Gender: {runner.GetGender()} | Skill: {skill}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while retrieving runners: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void DeleteRunner()
        {
            int runnerID;
            while (true)
            {
                Console.Write("Enter the ID of the runner to delete: ");
                if (int.TryParse(Console.ReadLine(), out runnerID) && runnerID > 0)
                {
                    break;
                }
                Console.WriteLine("Enter a positive integer for runner ID");
            }

            try
            {
                conn.Open();
                int removed = DatabaseHelper.DeleteRunner(conn, runnerID);
                conn.Close();

                if (removed == 0)
                {
                    Console.WriteLine("No runner found with the given ID.");
                }
                else
                {
                    Console.WriteLine("Runner deleted successfully!");
                }

                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while deleting runner: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void DisplayEventsMenu()
        {
            Console.Clear();
            Console.WriteLine("Events Management Menu\n\nPlease choose one of the following options:\n\n\n  1. Add a new event\n  2. View existing events\n  3. Add participant to event\n  4. Add results for a participant\n  5. Delete an event\n  6. Back to main menu");
            Console.CursorLeft = 0;
            Console.CursorTop = 5;
            Console.Write(">");

            switch (GetOption(1, 6))
            {
                case 1:
                    Console.Clear();
                    AddEvent();
                    break;
                case 2:
                    Console.Clear();
                    ViewEvents();
                    break;
                case 3:
                    Console.Clear();
                    AddParticipantToEvent();
                    break;
                case 4:
                    Console.Clear();
                    AddResultsForParticipant();
                    break;
                case 5:
                    Console.Clear();
                    DeleteEvent();
                    break;
                case 6:
                    Console.Clear();
                    return;
            }
        }

        static void AddEvent()
        {
            int routeID = GetRouteID();
            if (routeID == 0)
            {
                return;
            }

            string date = GetDate();
            string time = GetTime();
            string location = GetLocation();
            Console.WriteLine();

            try
            {
                conn.Open();
                DatabaseHelper.AddEvent(conn, routeID, date, time, location);
                conn.Close();

                Console.WriteLine("Event added successfully!");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while adding event: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void ViewEvents()
        {
            try
            {
                conn.Open();
                List<Event> events = DatabaseHelper.GetEvents(conn);
                conn.Close();

                Console.WriteLine("Events:\n");
                foreach (Event ev in events)
                {
                    string formattedDate = FormatAsDate(ev.GetDate());
                    string formattedTime = FormatAsTime(ev.GetTime());
                    Console.WriteLine($"Event ID: {ev.GetEventID()} | Route ID: {ev.GetRouteID()} | Date: {formattedDate} | Time: {formattedTime} | Location: {ev.GetLocation()}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while retrieving events: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void DeleteEvent()
        {
            int eventId;
            while (true)
            {
                Console.Write("Enter the ID of the event to delete: ");
                if (int.TryParse(Console.ReadLine(), out eventId) && eventId > 0)
                {
                    break;
                }
                Console.WriteLine("Enter a positive integer for event ID");
            }

            try
            {
                conn.Open();
                int removed = DatabaseHelper.DeleteEvent(conn, eventId);
                conn.Close();

                if (removed == 0)
                {
                    Console.WriteLine("No event found with the given ID.");
                }
                else
                {
                    Console.WriteLine("Event deleted successfully!");
                }

                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while deleting event: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void AddParticipantToEvent()
        {
            int runnerID;
            int eventID;

            Console.WriteLine("Enter ID of runner to add to event: ");
            if (!int.TryParse(Console.ReadLine(), out runnerID) || runnerID <= 0)
            {
                Console.WriteLine("Invalid runner ID");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                return;
            }

            Console.WriteLine("Enter ID of event to add runner to: ");
            if (!int.TryParse(Console.ReadLine(), out eventID) || eventID <= 0)
            {
                Console.WriteLine("Invalid event ID");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                return;
            }

            try
            {
                conn.Open();
                DatabaseHelper.EnsureParticipantsTableExists(conn);

                if (!DatabaseHelper.DoesRunnerExist(conn, runnerID))
                {
                    Console.WriteLine("No runner found with the given ID.");
                    conn.Close();
                    System.Threading.Thread.Sleep(1000);
                    Console.Clear();
                    return;
                }

                if (!DatabaseHelper.DoesEventExist(conn, eventID))
                {
                    Console.WriteLine("No event found with the given ID.");
                    conn.Close();
                    System.Threading.Thread.Sleep(1000);
                    Console.Clear();
                    return;
                }

                DatabaseHelper.AddParticipant(conn, eventID, runnerID);
                conn.Close();

                Console.WriteLine("Participant added successfully!");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while adding participant: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static void AddResultsForParticipant()
        {
            int runnerID;
            int eventID;

            Console.WriteLine("Enter ID of runner to add results for: ");
            if (!int.TryParse(Console.ReadLine(), out runnerID) || runnerID <= 0)
            {
                Console.WriteLine("Invalid runner ID");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                return;
            }

            Console.WriteLine("Enter ID of event to add results for: ");
            if (!int.TryParse(Console.ReadLine(), out eventID) || eventID <= 0)
            {
                Console.WriteLine("Invalid event ID");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
                return;
            }

            Console.WriteLine("Enter finish time (HH:MM:SS): ");
            string finishTime = Console.ReadLine();

            try
            {
                conn.Open();
                DatabaseHelper.EnsureParticipantsTableExists(conn);
                DatabaseHelper.AddParticipantResult(conn, eventID, runnerID, finishTime);
                conn.Close();

                Console.WriteLine("Results added successfully!");
                System.Threading.Thread.Sleep(1000);
                Console.Clear();
            }
            catch (SQLiteException excpetion)
            {
                Console.WriteLine("An error occurred while updating participant results: " + excpetion.Message);
                CloseConnectionIfOpen();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        static int GetRouteID()
        {
            while (true)
            {
                Console.Write("Enter route ID: ");
                int routeID;
                if (!int.TryParse(Console.ReadLine(), out routeID) || routeID <= 0)
                {
                    Console.WriteLine("Enter a positive integer for route ID");
                    continue;
                }

                try
                {
                    conn.Open();
                    bool exists = DatabaseHelper.DoesRouteIdExist(conn, routeID);
                    conn.Close();

                    if (exists)
                    {
                        return routeID;
                    }

                    Console.WriteLine("Enter a valid route ID");
                }
                catch (SQLiteException excpetion)
                {
                    Console.WriteLine("An error occurred while checking route ID: " + excpetion.Message);
                    CloseConnectionIfOpen();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    return 0;
                }
            }
        }

        static string GetTime()
        {
            while (true)
            {
                Console.Write("Enter event time (HH:MM:SS): ");
                string input = Console.ReadLine();

                TimeSpan time;
                if (TimeSpan.TryParseExact(input.Trim(), @"hh\:mm\:ss", CultureInfo.InvariantCulture, out time))
                {
                    return time.ToString(@"hh\:mm\:ss");
                }

                Console.WriteLine("Invalid time use HH:MM:SS format");
            }
        }

        static string GetDate()
        {
            while (true)
            {
                Console.Write("Enter event date (YYYY-MM-DD): ");
                string eventDate = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(eventDate))
                {
                    Console.WriteLine("Date is required");
                    continue;
                }

                DateTime date;
                if (DateTime.TryParseExact(eventDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    return date.ToString("yyyy-MM-dd");
                }

                Console.WriteLine("Invalid date. Use YYYY-MM-DD format");
            }
        }

        static string GetLocation()
        {
            string location;
            while (true)
            {
                Console.Write("Enter the location of the event to add: ");
                location = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(location))
                {
                    return location.Trim();
                }
                Console.WriteLine("Event location enterred is empty or only contains spaces");
            }
        }

        static string GetFilePath()
        {
            OpenFileDialog openFileDialogue = new OpenFileDialog();
            openFileDialogue.Title = "Select an image file";
            openFileDialogue.Filter = "Image Files|*.png";
            while (true)
            {
                if (openFileDialogue.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialogue.FileName;
                }
                else
                {
                    Console.WriteLine("No file selected. Please try again.");
                }
            }
        }

        static string GetRouteName()
        {
            string routeName;
            while (true)
            {
                Console.Write("Enter the name of the route to add: ");
                routeName = Console.ReadLine().Trim();
                if (!string.IsNullOrWhiteSpace(routeName))
                {
                    return routeName;
                }
                Console.WriteLine("Route name enterred is empty, only contains spaces, or contains a number");
                System.Threading.Thread.Sleep(1000);
            }
        }

        static double GetRouteDistance()
        {
            Console.Write("Enter route distance in km: ");
            while (true)
            {
                double distance;
                if (double.TryParse(Console.ReadLine(), out distance) && distance > 0)
                {
                    return distance;
                }
                Console.WriteLine("Enter a positive number for distance");
            }
        }

        static string GetRouteDifficulty()
        {
            Console.Write("Enter route difficulty (E for Easy, H for Hard): ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.E)
                {
                    return "Easy";
                }
                else if (key.Key == ConsoleKey.H)
                {
                    return "Hard";
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please press 'E' for Easy or 'H' for Hard.");
                }
            }
        }

        static string GetForename()
        {
            string forename;
            while (true)
            {
                Console.Write("Enter the forename of the runner to add: ");
                forename = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(forename))
                {
                    return forename.Trim();
                }
                Console.WriteLine("Runner forename enterred is empty or only contains spaces");
            }
        }

        static string GetSurname()
        {
            string surname;
            while (true)
            {
                Console.Write("Enter the surname of the runner to add: ");
                surname = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(surname))
                {
                    return surname.Trim();
                }
                Console.WriteLine("Runner surname enterred is empty or only contains spaces");
            }
        }

        static int GetAge()
        {
            while (true)
            {
                Console.Write("Enter age: ");
                int age;
                if (int.TryParse(Console.ReadLine(), out age) && age > 0)
                {
                    return age;
                }
                Console.WriteLine("Enter a positive integer for age");
            }
        }

        static string GetGender()
        {
            Console.Write("Enter gender (M for Male / F for Female): ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.M)
                {
                    return "Male";
                }
                else if (key.Key == ConsoleKey.F)
                {
                    return "Female";
                }
                else
                {
                    Console.WriteLine("Invalid input. Please press 'M' for Male or 'F' for female.");
                }
            }
        }

        static string FormatAsDate(object readDate)
        {
            string date = readDate.ToString();
            DateTime parsedDate = DateTime.Parse(date);
            return parsedDate.ToString("yyyy-MM-dd");
        }

        static string FormatAsTime(object readTime)
        {
            TimeSpan parsedTime = TimeSpan.Parse(readTime.ToString());
            return parsedTime.ToString(@"hh\:mm\:ss");
        }

        static int GetOption(int currentOption, int maxOptions)
        {
            bool exit = false;
            while (exit != true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.W && currentOption > 1)
                {
                    MoveUpMenu(currentOption);
                    currentOption--;
                }
                else if (key.Key == ConsoleKey.S && currentOption < maxOptions)
                {
                    MoveDownMenu(currentOption);
                    currentOption++;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    exit = true;
                }
            }
            return currentOption;
        }

        static void MoveUpMenu(int currentOption)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = currentOption + 4;
            Console.Write(" ");
            currentOption--;
            Console.CursorLeft = 0;
            Console.CursorTop = currentOption + 4;
            Console.Write(">");
        }

        static void MoveDownMenu(int currentOption)
        {
            Console.CursorLeft = 0;
            Console.CursorTop = currentOption + 4;
            Console.Write(" ");
            currentOption++;
            Console.CursorLeft = 0;
            Console.CursorTop = currentOption + 4;
            Console.Write(">");
        }

        static void CloseConnectionIfOpen()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}