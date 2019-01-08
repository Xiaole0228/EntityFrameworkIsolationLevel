using EntityFrameworkIsolationLevel.Model;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkIsolationLevel
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();

            Console.WriteLine("使用 Snapshot transaction: ");
            Task[] tasks =
            {
                Task.Factory.StartNew(() => FirstThead()),
                Task.Factory.StartNew(() => FirstThead())
            };

            Task.WaitAll(tasks);
            Console.WriteLine("-----------------------------------------------");

            Console.WriteLine("Press any key to exit.....");
            Console.ReadKey();

        }

        private static void FirstThead()
        {

            bool success = false;
            do
            {
                using (var db = new Model1())
                {

                    using (var dbTransaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                    {
                        Console.WriteLine($"FirstThead = BeginTransaction");
                        try
                        {
                            success = false;
                            //讀取資料1 
                            var d1 = db.Products.Find(1);
                            var d2 = db.Products.Find(2);

                            var d1v = d1.Quantity;
                            var d2v = d2.Quantity;

                            d1.Quantity += 2;
                            d2.Quantity++;

                            Console.WriteLine(
                                $"FirstThead = {d1.ProductName}:Quantity{d1v} > {d1.Quantity} Price:{d1.Price}");
                            Console.WriteLine(
                                $"FirstThead = {d2.ProductName}:Quantity{d2v} > {d2.Quantity} Price:{d2.Price}");

                            Thread.Sleep(1000);

                            db.SaveChanges();
                            dbTransaction.Commit();
                            Console.WriteLine($"FirstThead = Commit");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"FirstThead = Commit fail");
                            success = true;
                        }

                    }
                }
            } while (success);
        }

        private static void SecondThread()
        {
            for (int i = 0; i < 2; i++)
            {
                bool success = false;
                do
                {
                    using (var db = new Model1())
                    {
                        using (var dbTransaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
                        {
                            Console.WriteLine($"SecondThread = BeginTransaction");
                            try
                            {
                                success = false;                                
                                //讀取資料1 
                                var d1 = db.Products.Find(1);
                                var d2 = db.Products.Find(2);

                                var d1v = d1.Quantity;
                                var d2v = d2.Quantity;

                                d1.ProductId = 5;
                                d1.Quantity += 1;
                                d2.Quantity++;

                                Console.WriteLine(
                                    $"SecondThread = {d1.ProductName}:Quantity{d1v} > {d1.Quantity} Price:{d1.Price}");
                                Console.WriteLine(
                                    $"SecondThread = {d2.ProductName}:Quantity{d2v} > {d2.Quantity} Price:{d2.Price}");

                                Thread.Sleep(300);

                                db.SaveChanges();
                                dbTransaction.Commit();
                                Console.WriteLine($"SecondThread = Commit");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"SecondThread = Commit fail");
                                success = true;
                            }

                        }
                    }
                } while (success);
            }
        }
    }
}
