using IronWill.Models;
using Microsoft.EntityFrameworkCore;

namespace IronWill.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // context.Database.EnsureCreated(); // Ensure created is fine, but we will skip seeding.
            // context.Database.EnsureCreated();
            context.Database.Migrate();

            // Look for any habits.
            if (context.Habits.Any())
            {
                return;   // DB has been seeded
            }

            // CLEAN SLATE Requested via "Reset Info" command.
            // Commenting out dummy data generation to ensure empty start.
            
            /*
            var habits = new Habit[]
            {
                new Habit{Name="No Smoking", LastRelapseDate=DateTime.Now.AddDays(-5), BestStreakDays=12, Icon="fa-smoking"},
                new Habit{Name="No Gaming", LastRelapseDate=DateTime.Now.AddDays(-2), BestStreakDays=5, Icon="fa-gamepad"}
            };
            foreach (Habit h in habits)
            {
                context.Habits.Add(h);
            }

            var projects = new Project[]
            {
                new Project{ClientName="Contoso Ltd", ProjectName="E-Commerce API", ContractAmount=5000, PaidAmount=2000, Status="Active", ProgressPercentage=40, Deadline=DateTime.Now.AddDays(30)},
                new Project{ClientName="Fabrikam Inc", ProjectName="Inventory System", ContractAmount=3000, PaidAmount=0, Status="Pending", ProgressPercentage=0, Deadline=DateTime.Now.AddDays(14)}
            };
            foreach (Project p in projects)
            {
                context.Projects.Add(p);
            }

            var subjects = new Subject[]
            {
                new Subject{Name="Engineering Math", TargetHoursPerWeek=10, Topics=new List<Topic>
                {
                    new Topic{Title="Linear Algebra", IsCompleted=true},
                    new Topic{Title="Calculus III", IsCompleted=false},
                    new Topic{Title="Differential Equations", IsCompleted=false}
                }},
                new Subject{Name="Algorithms", TargetHoursPerWeek=8, Topics=new List<Topic>
                {
                    new Topic{Title="Sorting", IsCompleted=true},
                    new Topic{Title="Graph Theory", IsCompleted=false},
                    new Topic{Title="Dynamic Programming", IsCompleted=false}
                }}
            };
            foreach (Subject s in subjects)
            {
                context.Subjects.Add(s);
            }
            
            context.SaveChanges();
            */
        }
    }
}
