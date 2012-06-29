using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Thread[] threads = new Thread[10];
            Account acc = new Account(1000);
            for (int i = 0; i < 10; i++)
            {
                Thread t = new Thread(new ThreadStart(acc.DaTransactions));
                threads[i] = t;
            }
            for (int i = 0; i < 10; i++)
            {
                threads[i].Start();
            }

            //Worker wk = new Worker();
            //Thread t1 = new Thread(wk.Work);
            //t1.Start();
            //Console.WriteLine("This is main thread");
            //while (!t1.IsAlive) ;//等待t1的激活
            //Thread.Sleep(1);
            //wk.StopWork();

            //Thread t1 = new Thread(new ThreadStart(ThreadTest1));
            //Thread t2 = new Thread(new ParameterizedThreadStart(ThreadTest2));
            //t1.Start();
            //t2.Start("this is thread 2");

            //MethodTest tt = new MethodTest();
            //tt.InstanceMethod();

            Console.ReadKey();
        }

        public static void ThreadTest1()
        {
            Console.WriteLine("{0} this is thread 1", Thread.CurrentThread.ManagedThreadId);
        }

        public static void ThreadTest2(object str)
        {
            Console.WriteLine("{0} {1}", Thread.CurrentThread.ManagedThreadId, str.ToString());
        }
    }

    internal class Account //test lock
    {
        private object thislock = new object();
        private int balance;
        private Random r = new Random();

        public Account(int initial)
        {
            balance = initial;
        }

        private int Withdraw(int amount)
        {
            if (balance < 0)
            {
                throw new Exception("Negative");
            }
            lock (thislock)
            {
                if (balance >= amount)
                {
                    Console.WriteLine("Balance:" + balance);
                    Console.WriteLine("Amount:" + amount);
                    balance = balance - amount;
                    Console.WriteLine("Balance withdrawal:" + balance);
                    return amount;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void DaTransactions()
        {
            for (int i = 0; i < 100; i++)
            {
                Withdraw(r.Next(1, 100));
            }
        }
    }

    internal class Worker //test volatile
    {
        private volatile bool _stop = false;

        public void Work()
        {
            while (!_stop)
            {
                Console.WriteLine("working");
            }
            Console.WriteLine("stop working");
        }

        public void StopWork()
        {
            _stop = true;
        }
    }

    internal class MethodTest
    {
        public static void StaticMethod()
        {
            Console.WriteLine("this is a static method");
        }

        public void InstanceMethod()
        {
            InstanceMethod2();
            Console.WriteLine("this is a instance method");
        }

        public void InstanceMethod2()
        {
            InstanceMethod();
            Console.WriteLine("this is a instance method2");
        }
    }
}