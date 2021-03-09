using System;
using System.Collections.Generic;
using System.Linq;

namespace TAFL_Lab_2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Введите полный путь к файлу с описанием автомата: ");
                string filename = Console.ReadLine();
                StateReader sr = new StateReader();
                List<State> l = sr.getListMessedStates(filename);
                if (l != null)
                {
                    l = sr.getStates(l);
                    foreach (var z in l)
                        z.showState();
                    StateMachine sm = new StateMachine(l);
                    Console.WriteLine(sm.isHangs() ? "Имеет висячие вершины" : "Не имеет висячих вершин");

                    bool isDeter = sm.isDetermenistic();
                    Console.Write("Автомат");
                    Console.Write(isDeter ? " " : " не ");
                    Console.WriteLine("детерменирован");
                    if (!isDeter)
                    {
                        var aaaaa = sm.makeDeterminate();
                        foreach (var s in aaaaa)
                            s.showSetState();
                        sr.DeterminatedStateMachineWriteToFile(sm, filename+"Determinated.txt");
                    }
                    do
                    {
                        Console.WriteLine();
                        Console.Write("Выражение: ");
                        char[] expr = Console.ReadLine().ToArray();
                        Console.WriteLine(sm.isExpressionCorrect(expr) ? "Допустимо" : "Не допустимо");
                    } while (true);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
