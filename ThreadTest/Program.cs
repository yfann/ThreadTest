using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadTest
{
    internal class Program
    {
        public delegate void DoSomethind();

        private static void Main(string[] args)
        {
            //Thread[] threads = new Thread[10];
            //Account acc = new Account(1000);
            //for (int i = 0; i < 10; i++)
            //{
            //    Thread t = new Thread(new ThreadStart(acc.DaTransactions));
            //    threads[i] = t;
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    threads[i].Start();
            //}

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

            //Interrupt Test
            //InterruptTest it = new InterruptTest();
            //Thread t = new Thread(new ThreadStart(it.Sleep));
            //t.Start();
            //while (!t.IsAlive) ;
            //Thread.Sleep(1);
            //t.Interrupt();
            //Console.ReadKey();

            //monitor test
            //MonitorTest mt = new MonitorTest();
            //Thread ping = new Thread(mt.Ping);
            //Thread pong = new Thread(mt.Pong);
            //ping.Start();
            //pong.Start();

            //autoresetevent test
            //Console.WriteLine(new AutoEventTest().Result(123));
            //AutoResetEventTest1 autoReset = new AutoResetEventTest1();
            //Thread t1 = new Thread(autoReset.loop1);
            //Thread t2 = new Thread(autoReset.Loop);
            //t1.Start();
            //t2.Start();

            //mutex test
            //MutexTest mu = new MutexTest();
            //Thread[] ts = new Thread[10];
            //for (int i = 0; i < 10; i++)
            //{
            //    ts[i] = new Thread(mu.Run);
            //}
            //for (int i = 0; i < 10; i++)
            //{
            //    ts[i].Start();
            //}

            //async
            //DoSomethind dosth = new DoSomethind(() => { Console.WriteLine("do some thing"); });
            //IAsyncResult result = dosth.BeginInvoke(new AsyncCallback(async =>
            //{
            //    Console.WriteLine("before end invoke");
            //    dosth.EndInvoke(async);
            //    Console.WriteLine(async.AsyncState.ToString());
            //}), "test");
            //Console.WriteLine("end");

            TaskTest();
            Console.ReadLine();
        }

        public static void ThreadTest1()
        {
            Console.WriteLine("{0} this is thread 1", Thread.CurrentThread.ManagedThreadId);
        }

        public static void ThreadTest2(object str)
        {
            Console.WriteLine("{0} {1}", Thread.CurrentThread.ManagedThreadId, str.ToString());
        }

        public static void TaskTest()
        {
            Action<object> action = (object obj) =>
            {
                Console.WriteLine("Task={0}, obj={1}, Thread={2}", Task.CurrentId, obj.ToString(), Thread.CurrentThread.ManagedThreadId);
            };

            // Construct an unstarted task
            Task t1 = new Task(action, "alpha");

            // Cosntruct a started task
            Task t2 = Task.Factory.StartNew(action, "beta");

            // Block the main thread to demonstate that t2 is executing
            t2.Wait();

            // Launch t1
            t1.Start();

            Console.WriteLine("t1 has been launched. (Main Thread={0})", Thread.CurrentThread.ManagedThreadId);

            // Wait for the task to finish.
            // You may optionally provide a timeout interval or a cancellation token
            // to mitigate situations when the task takes too long to finish.
            t1.Wait();

            // Construct an unstarted task
            Task t3 = new Task(action, "gamma");

            // Run it synchronously
            t3.RunSynchronously();

            // Although the task was run synchrounously, it is a good practice to wait for it which observes for
            // exceptions potentially thrown by that task.
            t3.Wait();
        }
    }

    internal class MutexTest
    {
        private Mutex mu = new Mutex();

        public void Run()
        {
            mu.WaitOne();
            Console.WriteLine("{0} before sleep", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(500);
            Console.WriteLine("{0} after sleep", Thread.CurrentThread.ManagedThreadId);
            mu.ReleaseMutex();
        }
    }

    internal class AutoResetEventTest1
    {
        private AutoResetEvent auto = new AutoResetEvent(false);
        private AutoResetEvent auto1 = new AutoResetEvent(false);
        public int times = 10;

        public void SetEvent()
        {
            for (int i = 0; i < times; i++)
            {
                Console.WriteLine("set {0}", i);
                auto.Set();
                auto1.WaitOne();
            }
        }

        public void Loop()
        {
            for (int i = 0; i < times; i++)
            {
                auto1.Set();
                Console.WriteLine("loop before wait {0}", i);
                auto.WaitOne();
                Console.WriteLine("loop after wait {0}", i);
            }
        }

        public void loop1()
        {
            for (int i = 0; i < times; i++)
            {
                Console.WriteLine("loop1 before wait {0}", i);
                auto1.WaitOne();
                Console.WriteLine("loop1 after wait {0}", i);
                auto.Set();
            }
        }
    }

    internal class MonitorTest
    {
        private object lockMe = new object();
        public int times = 5;

        public void Ping()
        {
            lock (lockMe)
            {
                for (int i = 0; i < times; i++)
                {
                    Console.WriteLine("Ping");
                    Monitor.Pulse(lockMe);
                    Monitor.Wait(lockMe);
                }
            }
        }

        public void Pong()
        {
            lock (lockMe)
            {
                for (int i = 0; i < times; i++)
                {
                    Console.WriteLine("Pong");
                    Monitor.Pulse(lockMe);
                    Monitor.Wait(lockMe);
                }
            }
        }
    }

    internal class InterruptTest
    {
        public void Sleep()
        {
            Console.WriteLine("Start to sleep");
            try
            {
                Thread.Sleep(10000);
            }
            catch (Exception e) { Console.WriteLine("Exception happens"); }
            Console.WriteLine("Awake from sleep");
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