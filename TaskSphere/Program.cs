using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskSphere.Models;

namespace TaskSphere
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Read();
            Create();
            Update();
            DeleteCategory(6);
            DeleteTask(21);
            ADONetMethod();
        }

        private static void DeleteCategory(int id)
        {
            using (var context = new TaskSphereContext())
            {
                var categoryToDelete = context.Categories.Include(c => c.Tasks).SingleOrDefault(c => c.CategoryId == id);
                if (categoryToDelete != null)
                {
                    context.RemoveRange(categoryToDelete.Tasks);
                    context.Remove(categoryToDelete);
                }
            }
        }

        private static void DeleteTask(int id)
        {
            using (var context = new TaskSphereContext())
            {
                var taskToDelete = context.Tasks.SingleOrDefault(t => t.TaskId == id);
                if (taskToDelete != null)
                {
                    context.Remove(taskToDelete);
                }

                context.SaveChanges();
            }
        }

        private static void Update()
        {
            using (var context = new TaskSphereContext())
            {
                var newtask = new Models.Task()
                {
                    TaskId = 21,
                    Title = "Sample Task",
                    Status = "Completed",
                    Deadline = DateTime.Now.AddDays(3),
                    Description = "This is a sample task for updating.",
                    CategoryId = 2
                };

                context.Update(newtask);
                context.SaveChanges();
            }
        }

        private static void Create()
        {
            using (var context = new TaskSphereContext())
            {
                var task = new Models.Task()
                {
                    Title = "New Task",
                    Description = "This is a new task created using EF Core.",
                    Deadline = DateTime.Now.AddDays(7),
                    Status = "Pending",
                    CategoryId = context.Categories.SingleOrDefault(c => c.Name == "Home")?.CategoryId
                };

                context.Tasks.Add(task);
                var result = context.SaveChanges();
            }
        }

        private static void Read()
        {
            using (var context = new TaskSphereContext())
            {
                var categories = context.Categories.Include(c => c.Tasks);

                foreach (var category in categories)
                {
                    Console.WriteLine($"Category: {category.Name}");
                    foreach (var task in category.Tasks)
                    {
                        Console.WriteLine($"Task: {task.Title}");
                    }
                }
            }
        }

        private static void ADONetMethod()
        {
            string connectionString = "Data Source=DESKTOP-20KCN4C;Database=TaskSphere;Integrated Security=True;TrustServerCertificate=True;";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Console.WriteLine("Connection to the database established successfully.");
                ListAllTasks(connection);
                AddNewTaskToDatabase(connection);
                CompleteTaskById(connection);

                connection.Close();
            }
        }

        private static void ListAllTasks(SqlConnection connection)
        {
            var query = "SELECT * FROM Task AS t INNER JOIN Category AS c ON c.CategoryID = t.CategoryID";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Task ID: {reader["TaskID"]}, Task Name: {reader["Title"]}, Deadline: {reader["Deadline"]}, Category: {reader["Name"]}, Status: {reader["Status"]}");
                    }
                }
            }
        }

        private static void CompleteTaskById(SqlConnection connection)
        {
            string query;
            Console.WriteLine("\nEnter the Task ID to mark as completed:");
            var taskId = Console.ReadLine();

            query = "UPDATE Task SET Status = 'Completed' WHERE TaskID = @taskId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@taskId", taskId);

                var result = command.ExecuteNonQuery();
                Console.WriteLine($"Rows affected: {result}");
            }
        }

        private static void AddNewTaskToDatabase(SqlConnection connection)
        {
            string query;
            Console.WriteLine("\nEnter the title of a new task:");
            var taskTitle = Console.ReadLine();

            Console.WriteLine("Enter the description of the new task:");
            var taskDescription = Console.ReadLine();

            Console.WriteLine("Enter the deadline of the new task (YYYY-MM-DD):");
            var taskDeadline = Console.ReadLine();

            Console.WriteLine("Enter the category ID of the new task:");
            var categoryID = Console.ReadLine();

            var taskStatus = "Pending";

            query = $"INSERT INTO Task(Title, Description, Deadline, Status, CategoryID) VALUES (@title, @description, @deadline, @status, @categoryID)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@title", taskTitle);
                command.Parameters.AddWithValue("@description", taskDescription);
                command.Parameters.AddWithValue("@deadline", taskDeadline);
                command.Parameters.AddWithValue("@status", taskStatus);
                command.Parameters.AddWithValue("@categoryID", categoryID);

                var result = command.ExecuteNonQuery();
                Console.WriteLine($"Rows affected: {result}");
            }
        }
    }
}
