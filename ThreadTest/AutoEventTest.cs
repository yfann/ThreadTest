using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadTest
{
    internal class AutoEventTest
    {
        private double baseNum, firstTerm, secondTerm, thirdTerm;
        private AutoResetEvent[] autoEvents;
        private ManualResetEvent manualEvent;
        private Random randomGenerator;

        public AutoEventTest()
        {
            autoEvents = new AutoResetEvent[]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };
            manualEvent = new ManualResetEvent(false);
        }

        public void CalculateBase(object stateInfo)
        {
            Console.WriteLine("caculate base");
            baseNum = randomGenerator.NextDouble();
            manualEvent.Set();
        }

        private void CalculateFirstTerm(object stateInfo)
        {
            double temp = randomGenerator.NextDouble();
            Console.WriteLine("before caculate firstterm");
            manualEvent.WaitOne();
            Console.WriteLine("caculate firstterm");
            firstTerm = temp * baseNum * randomGenerator.NextDouble();
            autoEvents[0].Set();
        }

        private void CalculateSecondTerm(object stateInfo)
        {
            double temp = randomGenerator.NextDouble();
            Console.WriteLine("before caculate secondterm");
            manualEvent.WaitOne();
            Console.WriteLine("caculate secondterm");
            secondTerm = temp * baseNum * randomGenerator.NextDouble();
            autoEvents[1].Set();
        }

        private void CalculateThirdTerm(object stateInfo)
        {
            double temp = randomGenerator.NextDouble();
            Console.WriteLine("before caculate thirdterm");
            manualEvent.WaitOne();
            Console.WriteLine("caculate thirdterm");
            thirdTerm = temp * baseNum * randomGenerator.NextDouble();
            autoEvents[2].Set();
        }

        public double Result(int seed)
        {
            randomGenerator = new Random(seed);
            ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateBase));
            ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateFirstTerm));
            ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateSecondTerm));
            ThreadPool.QueueUserWorkItem(new WaitCallback(CalculateThirdTerm));
            Console.WriteLine("before result");
            WaitHandle.WaitAll(autoEvents);
            Console.WriteLine("after result");
            manualEvent.Reset();//重置

            return firstTerm + secondTerm + thirdTerm;
        }
    }
}