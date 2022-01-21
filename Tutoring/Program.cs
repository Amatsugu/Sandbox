using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationScheduler
{
	class Program
	{
		/*
         •	There is a maximum of 5 appointments for Cleaning and 
                                  5 appointments for Procedures, and 
                                  4 appointments for Checkups, 
            but only 8 total appointments can be made for a single day 
         •	A Cleaning appointment cannot be scheduled for appointment #8 and
            a Procedure cannot be scheduled for appointment 1 and appointment 2.
         •	We will only schedule and test two days of worth appointments for this Assignment, 
            but you should size your application for 365 days.
        */

		static void Main(string[] args)
		{
			//Variable declaration
			string[,,] array = new string[365, 8, 1];//will hold the appointments
			int cleaningCount = 0, proceduresCount = 0, checkUpCount = 0;//will store the number of appointments per appointment type
			int singleDayCounter = 0, morningCounter = 0, afternoonCounter = 4;//will store the number of appoints in a day, in the morning and afternoon
			int appointmentType, appointmentTime, appointmentDay;//input variables

			while (true) //to ask again and again
			{
				if (singleDayCounter < 8) //8 appointments per day
				{
					//Input again?
					Console.Write("Do you want to reserve an appointment? [0] No [1] Yes. \n Enter choice [0 or 1]: ");
					int choice = Convert.ToInt32(Console.ReadLine());
					if (choice == 0)
					{
						break;
					}

					//view the appointments
					Console.Write("Do you want to view appointments? [0] No [1] Yes. \n Enter choice [0 or 1]: ");
					int choice1 = Convert.ToInt32(Console.ReadLine());
					if (choice1 == 1)
					{
						display(array);
					}

					//System.Console.Clear();//use to clear the screen

					//ask user what day to reserve
					Console.Write("\nAppointment Day \n [0] Monday \n [1] Tuesday \n Enter appointment day [0 or 1]: ");
					appointmentDay = Convert.ToInt32(Console.ReadLine());

					//ask the user if morning or afternoon
					Console.Write("\nAppointment Time \n [1] Morning \n [2] Afternoon \n Enter appointment time [1 or 2]: ");
					appointmentTime = Convert.ToInt32(Console.ReadLine());

					Console.Write("\nAppointment Types \n [1] Cleaning \n [2] Procedures \n [3] Checkup \n Enter appointment type [1, 2, or 3]: ");
					appointmentType = Convert.ToInt32(Console.ReadLine());

					//validation
					if ((appointmentType == 1 && afternoonCounter == 7) || (appointmentType == 2 && morningCounter == 0) || (appointmentType == 2 && morningCounter == 1))
					{
						Console.WriteLine("MESSAGE: Cleaning cannot be scheduled for last appointment and Procedure cannot be scheduled for first and second appointment.");
					}
					else
					{

						//Cleaning reservation 
						if (appointmentType == 1 && cleaningCount < 5)
						{
							cleaningCount++;
						}
						else if (cleaningCount >= 5)
						{
							Console.WriteLine("MESSAGE: Maximum cleaning appointments reached. Slot not available");
						}

						//Procedure reservation
						if (appointmentType == 2 && proceduresCount < 5)
						{
							proceduresCount++;
						}
						else if (proceduresCount >= 5)
						{
							Console.WriteLine("MESSAGE: Maximum procedure appointments reached. Slot not available");
						}

						//Checkup reservation
						if (appointmentType == 3 && checkUpCount < 4)
						{
							checkUpCount++;
						}
						else if (checkUpCount >= 4)
						{
							Console.WriteLine("MESSAGE: Maximum checkup appointments reached. Slot not available");
						}


						if (morningCounter < 4 && appointmentTime == 1)
						{
							morningCounter++;
							array[appointmentDay, morningCounter - 1, 0] = appointmentType + "";
							Console.WriteLine("MESSAGE: Appointment is accepted.");
						}
						else if (morningCounter >= 4)
						{
							Console.WriteLine("MESSAGE: Morning appointments full. Slot not available");
						}
						if (afternoonCounter < 8 && appointmentTime == 2)
						{
							afternoonCounter++;
							array[appointmentDay, afternoonCounter - 1, 0] = appointmentType + "";
							Console.WriteLine("MESSAGE: Appointment is accepted.");
						}
						else if (afternoonCounter >= 8)
						{
							Console.WriteLine("MESSAGE: Afternoon appointments full. Slot not available");
						}


						singleDayCounter++;


					}//else


				}//singleDay condition
				else //singleDay counter
				{
					Console.WriteLine("Maximum appointments reached.");
					break;
				}
			}//while

		}//main

		static void display(string[,,] array)
		{
			//clear the console
			System.Console.Clear();

			for (int i = 0; i < 2; i++)
			{
				if (i == 0)
				{
					Console.WriteLine("\nMonday Appointment #\n");
				}
				else
				{
					Console.WriteLine("\nTuesday Appointment #\n");
				}
				for (int j = 1; j < 9; j++)
				{
					Console.Write("\t {0}\t\t", j);
					if (array[i, j - 1, 0] == "1")
					{
						Console.Write("Cleaning");
					}
					else if (array[i, j - 1, 0] == "2")
					{
						Console.Write("Procedures");
					}
					else if (array[i, j - 1, 0] == "3")
					{
						Console.Write("Checkup");
					}
					Console.Write("\n");
				}
			}
		}

	}
}