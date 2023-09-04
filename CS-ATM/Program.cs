using System;
using System.Collections.Generic;
using System.Threading;

namespace ATM_C_
{
    internal class Program
    {
        public class BankCard
        {
            public string BankName { get; set; }
            public string FullName { get; set; }
            public string PAN { get; set; }
            public string PIN { get; set; }
            public string CVC { get; set; }
            public string ExpireDate { get; set; }
            public double Balance { get; set; }
            public Bank Bank { get; set; }

            public BankCard(string bankName, string fullName, string pan, string pin, string cvc,
                string expireDate, double balance)
            {
                BankName = bankName;
                FullName = fullName;
                PAN = pan;
                PIN = pin;
                CVC = cvc;
                ExpireDate = expireDate;
                Balance = balance;
                Bank = null;
            }
        }

        public class User
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int Age { get; set; }
            public double Salary { get; set; }

            public BankCard BankAccount { get; set; }

            public User(int id, string name, string surname, int age, double salary, BankCard bankAccount)
            {
                ID = id;
                Name = name;
                Surname = surname;
                Age = age;
                Salary = salary;
                BankAccount = bankAccount;
            }
        }

        public class Transaction
        {
            public DateTime TransactionDate { get; set; }
            public string TransactionType { get; set; }
            public double Amount { get; set; }
        }

        public class Bank
        {
            public List<User> Clients { get; set; }

            public Bank()
            {
                Clients = new List<User>();
            }

            public void ShowCardBalance(BankCard bankCard)
            {
                Console.WriteLine($"Balance: {bankCard.Balance}");
            }

            public void WithdrawCash(BankCard bankCard)
            {
                Console.WriteLine("How much cash do you want to withdraw?:");
                double amount = Convert.ToDouble(Console.ReadLine());
                if (amount > bankCard.Balance)
                {
                    throw new Exception("Insufficient Funds");
                }

                bankCard.Balance -= amount;
                Console.WriteLine($"{amount} Manat were successfully deducted from your balance\n" +
                                  $"New Balance: {bankCard.Balance} ");
            }

            private List<Transaction> transactionHistory = new List<Transaction>();

            public void AddTransaction(Transaction transaction)
            {
                transactionHistory.Add(transaction);
            }

            public void ShowTransactions(User user)
            {
                Console.WriteLine($"Transaction History for {user.Name} {user.Surname}:");

                foreach (Transaction transaction in user.BankAccount.Bank.transactionHistory)
                {
                    Console.WriteLine(
                        $"Date: {transaction.TransactionDate}, Type: {transaction.TransactionType}, Amount: {transaction.Amount}");
                }
            }

            public void TransferFunds(User currentUser)
            {
                Console.WriteLine("Enter the destination card PAN:");
                string destinationPan = Console.ReadLine();

                Console.WriteLine("Enter the amount to transfer:");
                double amount = Convert.ToDouble(Console.ReadLine());

                BankCard sourceCard = currentUser.BankAccount;
                BankCard destinationCard = null;

                foreach (User user in Clients)
                {
                    if (user.BankAccount != null && destinationPan == user.BankAccount.PAN)
                    {
                        destinationCard = user.BankAccount;
                        break;
                    }
                }

                if (destinationCard == null)
                {
                    Console.WriteLine("Destination card not found.");
                    return;
                }

                try
                {
                    TransferFunds(sourceCard, destinationCard, amount);
                    sourceCard.Bank.AddTransaction(new Transaction
                    {
                        TransactionDate = DateTime.Now,
                        TransactionType = "Card to Card Transfer",
                        Amount = amount
                    });
                    destinationCard.Bank.AddTransaction(new Transaction
                    {
                        TransactionDate = DateTime.Now,
                        TransactionType = "Card to Card Transfer",
                        Amount = amount
                    });
                    Console.WriteLine("Transfer successful.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            public void TransferFunds(BankCard sourceCard, BankCard destinationCard, double amount)
            {
                if (amount <= 0)
                {
                    throw new ArgumentException("Amount must be greater than zero.");
                }

                if (amount > sourceCard.Balance)
                {
                    throw new Exception("Insufficient funds for the transfer.");
                }

                // Deduct the amount from the source card
                sourceCard.Balance -= amount;

                // Add the amount to the destination card
                destinationCard.Balance += amount;

                // Record the transaction for both cards
                Transaction transaction = new Transaction
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Card to Card Transfer",
                    Amount = amount
                };

                // Add the transaction to the source card's bank
                sourceCard.Bank.AddTransaction(transaction);

                // Add the transaction to the destination card's bank
                destinationCard.Bank.AddTransaction(transaction);

                Console.WriteLine($"Transfer of {amount} successful.");
            }
        }

        static void Main(string[] args)
        {
            BankCard bankCard1 = new BankCard("Capital Bank", "Mark Frank", "5103071506577648", "1234", "123",
                "05/2025", 2000);
            BankCard bankCard2 = new BankCard("Capital Bank", "Arthur Morgan", "5663767647647778", "4321", "321",
                "09/2027", 1000);
            BankCard bankCard3 = new BankCard("Capital Bank", "John Doe", "1234567890123456", "6789", "456",
                "12/2024", 1500);
            BankCard bankCard4 = new BankCard("Capital Bank", "Alice Smith", "9876543210987654", "2468", "789",
                "05/2025", 3000);
            BankCard bankCard5 = new BankCard("Capital Bank", "Bob Johnson", "1111222233334444", "1357", "246",
                "09/2026", 2500);

            User user1 = new User(1, "Mark", "Frank", 19, 2000, bankCard1);
            User user2 = new User(2, "Arthur", "Morgan", 36, 1000, bankCard2);
            User user3 = new User(3, "John", "Doe", 30, 1500, bankCard3);
            User user4 = new User(4, "Alice", "Smith", 25, 3000, bankCard4);
            User user5 = new User(5, "Bob", "Johnson", 22, 2500, bankCard5);

            Bank bank = new Bank();
            bank.Clients.Add(user1);
            bank.Clients.Add(user2);
            bank.Clients.Add(user3);
            bank.Clients.Add(user4);
            bank.Clients.Add(user5);

            while (true)
            {
                Thread.Sleep(1500);
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Please Enter Your PIN : ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                string enteredPin = Console.ReadLine();
                User currentUser = null;

                foreach (User user in bank.Clients)
                {
                    if (enteredPin == user.BankAccount.PIN)
                    {
                        currentUser = user;
                        break;
                    }
                }

                if (currentUser != null)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\nWelcome: {currentUser.Name} {currentUser.Surname}\n");
                    Console.WriteLine("Please choose one of the options below.");
                    Console.WriteLine("1. Balance");
                    Console.WriteLine("2. Cash Withdrawal");
                    Console.WriteLine("3. Transaction History");
                    Console.WriteLine("4. Card to Card Transfer");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("\nOption -> ");
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    int option = Convert.ToInt32(Console.ReadLine());
                    switch (option)
                    {
                        case 1:
                            bank.ShowCardBalance(currentUser.BankAccount);
                            break;
                        case 2:
                            Console.WriteLine("\nPlease choose one of the options below:");
                            Console.WriteLine("1. 10 AZN");
                            Console.WriteLine("2. 20 AZN");
                            Console.WriteLine("3. 50 AZN");
                            Console.WriteLine("4. 100 AZN");
                            Console.WriteLine("5. Other");

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("\nOption -> ");
                            Console.ForegroundColor = ConsoleColor.Magenta;

                            int withdrawOption;
                            while (!int.TryParse(Console.ReadLine(), out withdrawOption) || withdrawOption < 1 ||
                                   withdrawOption > 5)
                            {
                                Console.WriteLine("Wrong Choice! Make your choice between 1-5:");
                            }

                            double withdrawAmount = 0;

                            switch (withdrawOption)
                            {
                                case 1:
                                    withdrawAmount = 10;
                                    break;
                                case 2:
                                    withdrawAmount = 20;
                                    break;
                                case 3:
                                    withdrawAmount = 50;
                                    break;
                                case 4:
                                    withdrawAmount = 100;
                                    break;
                                case 5:
                                    Console.WriteLine("Enter the Amount:");
                                    withdrawAmount = Convert.ToDouble(Console.ReadLine());
                                    break;
                                default:
                                    Console.WriteLine("Wrong Choice!");
                                    break;
                            }

                            if (withdrawAmount > currentUser.BankAccount.Balance)
                            {
                                try
                                {
                                    throw new Exception("You do not have enough funds in your balance!");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                            else
                            {
                                currentUser.BankAccount.Balance -= withdrawAmount;
                                Console.WriteLine(
                                    $"{withdrawAmount} Manat successfully withdrawn from your balance.\n" +
                                    $"Your Balance: {currentUser.BankAccount.Balance} Manat");
                                Console.WriteLine("Please take your cash.");
                                Console.ReadLine();
                                Console.Clear();
                            }

                            break;

                        case 3:
                            bank.ShowTransactions(currentUser);
                            Console.ReadLine();
                            break;

                        case 4:
                            bank.TransferFunds(currentUser);
                            Console.ReadLine();
                            break;
                        default:
                            Console.WriteLine("Wrong Choose!");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.Write("Wrong Password, Please Try Again: ");
                    Console.ReadLine();
                }
            }
        }
    }
}