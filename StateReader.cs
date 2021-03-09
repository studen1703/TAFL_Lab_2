using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace TAFL_Lab_2
{
    class StateReader
    {//класс для работы с файлами: запись/считывание состояний и переходов
        public List<State> getListMessedStates(string filename)
        {//функция считывания состояний и переходов из файла
            //возвращает список состояний, созданный по принципу "одно состояние - один переход" (состояния могут повторяться)
            try
            {
                List<State> states = new List<State>();
                using (StreamReader sr = new StreamReader(filename))
                {
                    do
                    {
                        string potentialState = sr.ReadLine();
                        Regex rg = new Regex(@"(q|f)(\d+)(,)(.)(=)(q|f)(\d+)", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                        if (rg.IsMatch(potentialState))
                        {
                            string[] splitString = rg.Split(potentialState);
                            List<string> matches = new List<string>();
                            foreach (var s in splitString)
                                if (s != "")
                                    matches.Add(s);
                            State state = new State();
                            state.isFinalState = matches[0] == "f";
                            state.stateNumber = int.Parse(matches[1]);
                            KeyValuePair<char, State> kp = new KeyValuePair<char, State>(matches[3].ToCharArray()[0],
                                new State(int.Parse(matches[6]), matches[5] == "f"));
                            state.transitions.Add(kp);
                            states.Add(state);
                        }
                        else if(!rg.IsMatch(potentialState) && potentialState !="") throw new Exception("Синтаксические ошибки в тексте: " + potentialState);
                    } while (!sr.EndOfStream);
                }
                return states;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public List<State> getStates(List<State> listMessedStates)
        {//функция для объединения всех переходов для одного состояния в единое представление
            //одно состояние - много переходов
            List<State> states = new List<State>();
            do
            {
                State currentState = listMessedStates[0];
                for (int i = 0; i < listMessedStates.Count(); i++)
                {
                    State inList = listMessedStates[i];
                    if (currentState.stateNumber == inList.stateNumber
                                                    && !currentState.isTheSameState(inList))
                    {   //если номер состояния один, но состояния не повторяют друг друга
                        //переписываем все состояния в спсиок для "главного"
                        for (int j = 0; j < inList.transitions.Count(); j++)
                        {
                            var kp = new KeyValuePair<char, State>(inList.transitions[j].Key,inList.transitions[j].Value);
                            currentState.transitions.Add(kp);
                            
                        }
                    listMessedStates.Remove(listMessedStates[i]);
                        i--;
                    }
                    if (currentState.isTheSameState(inList)) 
                    {
                        listMessedStates.Remove(listMessedStates[i]);
                        i--;
                    }
                    if (i == listMessedStates.Count() - 1)
                    {
                        states.Add(currentState);
                    }
                }
                listMessedStates.Remove(currentState);
            } while (listMessedStates.Count() > 0);
            return states;
        }
        public void DeterminatedStateMachineWriteToFile(StateMachine sm, string filename)
        {//функция записи детерминированного  автомата в файл
            using (FileStream fs = File.Create(filename))
            {
                if(sm.setStates != null)
                foreach(var setState in sm.setStates)
                {
                    string buf = "";
                    foreach (var tr in setState.setTransitions)
                    {
                            foreach (var state in tr.Value.stateNumbers)
                            {
                                buf = setState.getNameSetState();
                                buf += ",";
                                buf += tr.Key;
                                buf += "=";
                                buf += state.isFinalState ? "f" : "q";
                                buf += state.stateNumber;
                                buf += "\n";
                                byte[] arr = Encoding.Unicode.GetBytes(buf);
                                fs.Write(arr, 0, arr.Length);
                            }
                    }
                }
            }
        }
    }
}
