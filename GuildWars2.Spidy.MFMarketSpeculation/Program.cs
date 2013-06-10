using System;
using System.Collections.Generic;
using System.Reflection;

namespace GuildWars2.Spidy.MFMarketSpeculation
{
    public class Program
    {
        private static IList<BaseSpeculation> m_SPECULATIONS = null;
        private static IList<BaseSpeculation> SPECULATIONS
        {
            get
            {
                if (m_SPECULATIONS == null)
                {
                    m_SPECULATIONS = new List<BaseSpeculation>();

                    Assembly asm = Assembly.GetExecutingAssembly();
                    foreach (Type type in asm.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(BaseSpeculation)))
                        {
                            m_SPECULATIONS.Add((BaseSpeculation)asm.CreateInstance(type.FullName));
                        }
                    }
                }

                return m_SPECULATIONS;
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Available speculations:");
            for (int i = 0; i < SPECULATIONS.Count; i++)
                Console.WriteLine("  [{0,2}] {1}", i + 1, SPECULATIONS[i].Name);
            Console.Write("Which speculation would you like to run? (1-{0}) [1]: ", SPECULATIONS.Count);
            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > SPECULATIONS.Count)
                choice = 1;
            SPECULATIONS[choice - 1].Run();
            Console.WriteLine("Speculation finished. Press any key to exit.");
            Console.ReadKey(true);
        }
    }
}
