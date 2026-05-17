// hey if you're editing this, drop a note in the README so we're all on the same page, thx

using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentBudgetTracker
{
    // Just a simple class to hold each transaction's info
    public class Transaction
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Type { get; set; } // "Income" or "Expense"
        public DateTime Date { get; set; }
        public string UserName { get; set; }

        public Transaction(string description, decimal amount, string category, string type, string userName)
        {
            Description = description;
            Amount = amount;
            Category = category;
            Type = type;
            Date = DateTime.Now;
            UserName = userName;
        }
    }

    class Program
    {
        // This list holds all transactions while the app is running
        static List<Transaction> transactions = new List<Transaction>();
        static string userName;
        // Track who's added income so we can warn on repeat
        static HashSet<string> incomeUsers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Track who's added expenses and their last category
        static HashSet<string> expenseUsers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        static Dictionary<string, string> lastExpenseCategory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static bool signOutRequested = false;

        static void Main(string[] args)
        {
            // Keep the app running forever, just sign out/in for different users
            while (true)
            {
                // Make sure we get a real name, not just numbers
                do
                {
                    Console.Write("Please enter your name: ");
                    userName = Console.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(userName) || userName.All(char.IsDigit))
                    {
                        Console.WriteLine("Name cannot be empty or numeric only. Please try again.");
                        userName = null;
                    }
                } while (userName == null);

                Console.Clear();
                bool inMenu = true;

                while (inMenu)
                {
                    Console.Clear();
                    Console.WriteLine($"=== Apartment Budget Tracker ===");
                    Console.WriteLine($"Welcome, {userName}!");
                    Console.WriteLine("1. Add income");
                    Console.WriteLine("2. Add expenses");
                    Console.WriteLine("3. Display Balance");
                    Console.WriteLine("4. Expense review");
                    Console.WriteLine("0. Sign out");
                    Console.Write("\nSelect an option (0-4): ");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "0":
                            SignOut();
                            inMenu = false;
                            break;
                        case "1":
                            AddIncome();
                            break;
                        case "2":
                            AddExpense();
                            break;
                        case "3":
                            DisplayBalance();
                            break;
                        case "4":
                            ExpenseReview();
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to try again.");
                            Console.ReadKey();
                            break;
                    }

                    // If the user signed out from inside a submenu, exit menu loop
                    if ((choice == "1" || choice == "2") && signOutRequested)
                    {
                        signOutRequested = false;
                        inMenu = false;
                    }
                }
            }
        }

        // Quick signout, clear the screen for privacy
        static void SignOut()
        {
            userName = string.Empty;
            Console.Clear();
            Console.WriteLine("You have been signed out. Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        // Add income flow, show current total, confirm, update, warn if repeated
        static void AddIncome()
        {
            Console.Clear();
            Console.WriteLine("--- Add Income ---");

            if (incomeUsers.Contains(userName))
            {
                Console.WriteLine($"You have already recorded income under the name '{userName}'.");
                Console.Write("Are you sure you want to add more income? (Y/N): ");
                if (Console.ReadLine()?.Trim().ToUpper() != "Y")
                {
                    Console.WriteLine("Income addition cancelled. Press any key to return to menu.");
                    Console.ReadKey();
                    return;
                }
            }

            decimal currentIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            Console.WriteLine($"Current total income: R {currentIncome}\n");

            decimal amount;
            while (true)
            {
                Console.Write("Enter income amount to add: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount))
                {
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                    continue;
                }

                Console.Write($"You entered R {amount}. Is this correct? (Y/N): ");
                string confirm = Console.ReadLine()?.Trim().ToUpper();
                if (confirm == "Y") break;
                if (confirm == "N") Console.WriteLine("Let's try again.");
                else Console.WriteLine("Please answer Y or N.");
            }

            transactions.Add(new Transaction("", amount, "", "Income", userName));
            incomeUsers.Add(userName);

            decimal newIncome = currentIncome + amount;
            Console.WriteLine($"\nIncome added! Your new total income is: R {newIncome}");

            ShowPostAddMenu();
        }

        // Add expense flow, pick category, confirm, show warning with last category
        static void AddExpense()
        {
            Console.Clear();
            Console.WriteLine("--- Add Expense ---");

            if (expenseUsers.Contains(userName))
            {
                string lastCat = lastExpenseCategory.TryGetValue(userName, out string cat) ? cat : "Unknown";
                Console.WriteLine($"You have already recorded expenses under the name '{userName}'.");
                Console.WriteLine($"Your last recorded expense category was '{lastCat}'.");
                Console.Write("Are you sure you want to add another expense? (Y/N): ");
                if (Console.ReadLine()?.Trim().ToUpper() != "Y")
                {
                    Console.WriteLine("Expense addition cancelled. Press any key to return to menu.");
                    Console.ReadKey();
                    return;
                }
            }

            // Keep asking until we get a valid category number
            string category = "";
            while (true)
            {
                Console.WriteLine("\nSelect expense category:");
                Console.WriteLine("1. Rent");
                Console.WriteLine("2. Groceries");
                Console.WriteLine("3. Entertainment");
                Console.WriteLine("4. Transport");
                Console.WriteLine("5. Utilities");
                Console.WriteLine("6. Other");
                Console.Write("Enter your choice (1-6): ");
                string catChoice = Console.ReadLine();

                category = catChoice switch
                {
                    "1" => "Rent",
                    "2" => "Groceries",
                    "3" => "Entertainment",
                    "4" => "Transport",
                    "5" => "Utilities",
                    "6" => "Other",
                    _ => null
                };

                if (category != null) break;
                Console.WriteLine("Invalid choice. Please enter a number from 1 to 6.");
            }

            decimal currentExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            Console.WriteLine($"\nCurrent total expenses: R {currentExpense}\n");

            decimal amount;
            while (true)
            {
                Console.Write("Enter expense amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount))
                {
                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                    continue;
                }

                Console.Write($"You entered R {amount} for {category}. Is this correct? (Y/N): ");
                string confirm = Console.ReadLine()?.Trim().ToUpper();
                if (confirm == "Y") break;
                if (confirm == "N") Console.WriteLine("Let's try again.");
                else Console.WriteLine("Please answer Y or N.");
            }

            transactions.Add(new Transaction("", amount, category, "Expense", userName));
            expenseUsers.Add(userName);
            lastExpenseCategory[userName] = category;

            decimal newExpense = currentExpense + amount;
            Console.WriteLine($"\nExpense added! Your new total expenses: R {newExpense}");

            ShowPostAddMenu();
        }

        // After adding income/expense, ask what to do next
        static void ShowPostAddMenu()
        {
            Console.WriteLine("\nWhat would you like to do?");
            Console.Write("Press M to return to the main menu, or S to sign out: ");
            string postChoice = Console.ReadLine()?.Trim().ToUpper();
            if (postChoice == "S")
            {
                signOutRequested = true;
                Console.Clear();
                Console.WriteLine("You have been signed out. All data saved. Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Returning to main menu... Press any key.");
                Console.ReadKey();
            }
        }

        // Display the big picture: income, expenses, net balance
        static void DisplayBalance()
        {
            Console.Clear();
            Console.WriteLine("--- Current Balance ---");
            decimal totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            decimal totalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => t.Amount);
            Console.WriteLine($"Total Income:   R {totalIncome}");
            Console.WriteLine($"Total Expenses: R {totalExpense}");
            Console.WriteLine("-------------------------");
            Console.WriteLine($"Net Balance:    R {totalIncome - totalExpense}");
            Console.WriteLine("\nPress any key to return to the menu.");
            Console.ReadKey();
        }

        // Expense review draws ASCII pie charts in the console (no external libs)
        static void ExpenseReview()
        {
            Console.Clear();
            Console.WriteLine("--- Expense Review ---");

            if (transactions.Count == 0)
            {
                Console.WriteLine("No transactions recorded yet. Nothing to chart.");
                Console.WriteLine("\nPress any key to return to menu.");
                Console.ReadKey();
                return;
            }

            var incomeTrans = transactions.Where(t => t.Type == "Income").ToList();
            var expenseTrans = transactions.Where(t => t.Type == "Expense").ToList();

            if (incomeTrans.Count > 0)
            {
                var incomeByUser = incomeTrans
                    .GroupBy(t => t.UserName)
                    .Select(g => new { Label = g.Key, Value = g.Sum(t => t.Amount) })
                    .ToList();
                DrawAsciiPieChart("Income by Individual",
                    incomeByUser.Select(x => x.Label).ToArray(),
                    incomeByUser.Select(x => (double)x.Value).ToArray());
            }

            if (expenseTrans.Count > 0)
            {
                var expensesByCat = expenseTrans
                    .GroupBy(t => t.Category)
                    .Select(g => new { Label = g.Key, Value = g.Sum(t => t.Amount) })
                    .ToList();
                DrawAsciiPieChart("Expenses by Category",
                    expensesByCat.Select(x => x.Label).ToArray(),
                    expensesByCat.Select(x => (double)x.Value).ToArray());
            }

            decimal totalIncome = incomeTrans.Sum(t => t.Amount);
            decimal totalExpense = expenseTrans.Sum(t => t.Amount);
            if (totalIncome > 0 || totalExpense > 0)
            {
                DrawAsciiPieChart("Income vs Expenses",
                    new[] { "Income", "Expenses" },
                    new[] { (double)totalIncome, (double)totalExpense });
            }

            Console.WriteLine("\nAll charts displayed. Press any key to return to menu.");
            Console.ReadKey();
        }

        // Draw an old-school ASCII pie chart with solid block chars for clear slices
        static void DrawAsciiPieChart(string title, string[] labels, double[] values)
        {
            double total = values.Sum();
            if (total == 0) return;

            // Use block characters so slices really stand out
            char[] sliceChars = new char[values.Length];
            char[] palette = { '█', '▓', '▒', '░', '▌', '▐', '▀', '▄' };
            for (int i = 0; i < values.Length; i++)
                sliceChars[i] = palette[i % palette.Length];

            // Bigger circle if it's a single 100% item
            int radius = (values.Length == 1) ? 15 : 10;
            int diameter = radius * 2;
            double PI = Math.PI;

            double[] sliceAngles = values.Select(v => (v / total) * 2 * PI).ToArray();

            // Normalize all angles to 0-2π so the slice math works
            double[] startAngles = new double[values.Length];
            double[] endAngles = new double[values.Length];
            double currentAngle = -PI / 2; // start from top
            for (int i = 0; i < values.Length; i++)
            {
                startAngles[i] = (currentAngle + 2 * PI) % (2 * PI);
                currentAngle += sliceAngles[i];
                endAngles[i] = (currentAngle + 2 * PI) % (2 * PI);
            }

            int canvasWidth = diameter * 2 + 1;
            int canvasHeight = diameter + 1;
            char[,] canvas = new char[canvasHeight, canvasWidth];
            for (int y = 0; y < canvasHeight; y++)
                for (int x = 0; x < canvasWidth; x++)
                    canvas[y, x] = ' ';

            // Fill the circle with block chars
            for (int y = 0; y < canvasHeight; y++)
            {
                for (int x = 0; x < canvasWidth; x++)
                {
                    double dx = (x / 2.0) - radius;
                    double dy = y - radius;
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    if (dist <= radius)
                    {
                        double angle = Math.Atan2(dy, dx);
                        if (angle < 0) angle += 2 * PI;

                        for (int i = 0; i < values.Length; i++)
                        {
                            double start = startAngles[i];
                            double end = endAngles[i];
                            bool inSlice = (start <= end) ? (angle >= start && angle < end) : (angle >= start || angle < end);
                            if (inSlice)
                            {
                                canvas[y, x] = sliceChars[i];
                                break;
                            }
                        }
                    }
                }
            }

            // Draw the chart and legend
            Console.WriteLine($"\n  {title}");
            Console.WriteLine(new string('-', 50));

            for (int y = 0; y < canvasHeight; y++)
            {
                Console.Write("  ");
                for (int x = 0; x < canvasWidth; x++)
                    Console.Write(canvas[y, x]);
                Console.WriteLine();
            }

            Console.WriteLine();
            for (int i = 0; i < labels.Length; i++)
            {
                double percent = (values[i] / total) * 100;
                Console.WriteLine($"  {sliceChars[i]} {labels[i]}: {values[i]:C} ({percent:0.0}%)");
            }
            Console.WriteLine(new string('-', 50));
            Console.WriteLine("  Press any key for next chart...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
