using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to the Bad Student Management System!");
        
        List<string> students = new List<string>();
        List<int> ages = new List<int>();
        List<double> grades = new List<double>();
        
        while (true)
        {
            Console.WriteLine("1. Add Student");
            Console.WriteLine("2. Show All Students");
            Console.WriteLine("3. Find Student by Name");
            Console.WriteLine("4. Calculate Average Grade");
            Console.WriteLine("5. Exit");
            Console.Write("Enter choice: ");
            string choice = Console.ReadLine();
            
            if (choice == "1")
            {
                Console.Write("Enter student name: ");
                string name = Console.ReadLine();
                students.Add(name);
                
                Console.Write("Enter age: ");
                int age = Convert.ToInt32(Console.ReadLine());
                ages.Add(age);
                
                Console.Write("Enter grade: ");
                double grade = Convert.ToDouble(Console.ReadLine());
                grades.Add(grade);
                
                Console.WriteLine("Student added successfully!");
            }
            else if (choice == "2")
            {
                Console.WriteLine("List of Students:");
                for (int i = 0; i < students.Count; i++)
                {
                    Console.WriteLine("Name: " + students[i] + ", Age: " + ages[i] + ", Grade: " + grades[i]);
                }
            }
            else if (choice == "3")
            {
                Console.Write("Enter student name to search: ");
                string searchName = Console.ReadLine();
                bool found = false;
                for (int i = 0; i < students.Count; i++)
                {
                    if (students[i] == searchName)
                    {
                        Console.WriteLine("Found: " + students[i] + " Age: " + ages[i] + " Grade: " + grades[i]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Student not found!");
                }
            }
            else if (choice == "4")
            {
                double sum = 0;
                for (int i = 0; i < grades.Count; i++)
                {
                    sum += grades[i];
                }
                Console.WriteLine("Average Grade: " + (sum / grades.Count));
            }
            else if (choice == "5")
            {
                Console.WriteLine("Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Try again.");
            }
        }
    }
}
