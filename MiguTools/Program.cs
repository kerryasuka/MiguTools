using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MiguTools
{
    class Program
    {
        static int totalCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            FileOperation fo = new FileOperation();

            fo.CountEvent += Fo_CountCallBack;
            fo.ErrorEvent += Fo_ErrorCallBack;
            fo.LogEvent += Fo_LogCallBack;
            fo.EndEvent += Fo_EndEvent;

            Task.Factory.StartNew(() =>
            {
                fo.Start();
            });
            Console.ReadKey();
        }

        private static void Fo_EndEvent()
        {
            Console.WriteLine("Please quit.");
            Console.ReadKey();
        }

        private static void Fo_LogCallBack(string fileName, int normalCount)
        {
            Console.WriteLine(string.Format("Complete file 【{0}】 / 【{1}】, name {2}.", normalCount, totalCount, fileName));
        }

        private static void Fo_ErrorCallBack(List<string> list)
        {
            Console.WriteLine("=====================================================");
            list.ForEach(f =>
            {
                Console.WriteLine(string.Format("Abnormal file 【{0}】/【{1}】, name {2}.", list.IndexOf(f) + 1, list.Count, f));
            });
            Console.WriteLine("=====================================================");            
        }

        private static void Fo_CountCallBack(int count)
        {
            totalCount = count;
            Console.WriteLine(string.Format("Detect 【{0}】 files.", count));
        }
    }
}
