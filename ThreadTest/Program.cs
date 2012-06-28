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
            Thread t1 = new Thread(new ThreadStart(ThreadTest1));
            Thread t2 = new Thread(new ParameterizedThreadStart(ThreadTest2));

            t1.Start();
            t2.Start("this is thread 2");

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