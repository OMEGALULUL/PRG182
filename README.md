# PRG182
Group project for PRG182. README contains info on dev work (Dev notes for us to use to present)

""// Please when changing lines of code on this program make sure to
// implement a dev note inside the README.md file to keep all of us in the loop for any bugs/breaks please""



14/05/2026, I uploaded the tester model and the C# file to the repo. when we commit changes or document everything as a note make sure to use "--Your name" to make sure we know who made the note in the README file please. --Chris


14/05/2026 - I uploaded the base tester model and the initial C# file to the repo. The main menu now asks for a user's name before showing options. The menu options were renamed to: Add income, Add expenses, Display Balance, and Expense review. The old Exit option was removed and the program now runs until sign-out. ViewSummary was renamed to DisplayBalance. Expense review is just a placeholder for now till implemented 
--Chris



15/05/2026 - Reworked Add income completely. Now it shows the current total income first (starts at 0 and builds). You enter an amount, confirm it, then see the updated total. After adding, you can press M for main menu or S to sign out. Signing out clears the current username but keeps all transaction data. No description or category prompts for income entries. A tracking set warns you if you try to add income again under the same name e.g "Idk" is typed in again as a sign in and then when going into the same prompt it warns them
--Chris



16/05/2026 (morning) - Built out Add expenses. It gives a numbered category list: Rent, Groceries, Entertainment, Transport, Utilities, Other. The program forces a valid number 1-6. After choosing a category, you see the current total expenses, then you enter and confirm the amount. A warning appears if you've added expenses before under that name, showing your last recorded category. The last category per user is stored for accurate warnings. You still get the M/S menu after adding an expense  
--Chris



16/05/2026 (afternoon) - Added option 0 to the main menu for Sign out. When you sign out from the menu, the terminal clears for privacy, shows a message, clears again, then asks for the next name. The same privacy clear happens when signing out from inside Add income or Add expenses 
--Chris




16/05/2026 (evening) - Started working on pie charts for Expense review. I tried ScottPlot but don't want external dependencies. System.Drawing needs an extra NuGet package on newer .NET, so I skipped that too. Ended up building an ASCII art pie chart generator that runs in the console, no external libs needed
--Chris




17/05/2026 (morning) - Fixed a bug in the ASCII pie where slices weren't showing because angles weren't normalized. Now all angles stay in the 0-2π range. The circle radius is 10 normally, but 15 when there's only one slice (100%) so it's huge. Tried letters A, B, C at first, but switched to solid block characters (█▓▒░▌▐▀▄) for way better visibility, found em on google looking for better ASCII display chart distinctation. Three charts are drawn: Income by Individual, Expenses by Category, Income vs Expenses, each with a percentage legend for the items
--Chris




17/05/2026 (afternoon) - Optimized startup by removing the unused AddTransaction method and some unnecessary Console.Clear calls. Used LINQ Sum in a few places. Added validation so you can't sign in with an empty name or a purely numeric name (like "123"). Everything else works the same


--Chris

The program is fully functional in CLI mode, no external libraries required.
