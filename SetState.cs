using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL_Lab_2
{
    class SetState
    {//класс для описания состояния-множества
        public List<State> stateNumbers; //список состояний, которые входят в состояние-множество
        public Dictionary<char, SetState> setTransitions;//список переходов
        public bool isAnyFinalState = false; //наличие хотя бы одного конечного состояния
        public SetState()
        {
            stateNumbers = new List<State>();
            setTransitions = new Dictionary<char, SetState>();
            checkForFinalState();
        }
        public bool isTheSameSetState(SetState anotherSetState)
        {//функция проверки на идентичность состояний-множеств
            bool isFind = false;
            if (anotherSetState.stateNumbers.Count == stateNumbers.Count)
            {
                foreach (var s in stateNumbers)
                {
                    foreach (var ss in anotherSetState.stateNumbers)
                    {
                        if (s.stateNumber == ss.stateNumber)
                        {
                                if (s.transitions.Count == ss.transitions.Count)
                                {
                                    isFind = true;
                                    break;
                                }
                        }
                    }
                    if (!isFind) return false;
                    isFind = false;
                }
                return true;
            }
            else return false;
        }
        public bool hasThisState(int num)
        {//функция проверки на наличие заданного состояния в списке состояний состояния-множества
            foreach (var s in stateNumbers)
                if (s.stateNumber == num)
                    return true;
            return false;
        }
        public string getNameSetState()
        {//функция, составляющая имя состояния-множества для последующего вывода/представления/использования в виде строки
            string name = "";
            for(int i = 0; i<stateNumbers.Count(); i++)
            {
                string c = stateNumbers[i].isFinalState ? "f" : "q";
                name += c;
                name += stateNumbers[i].stateNumber;
                if(i!= stateNumbers.Count()-1)
                    name += ", ";
            }
            return name;
        }
        public void showSetState()
        {//функция отображения состояния-множества на дисплей
            Console.WriteLine("Состояние: {" + getNameSetState() + "}");
            if (setTransitions.Count != 0)
            {
                Console.WriteLine("Переходы:");
                foreach (var s in setTransitions)
                {
                    Console.WriteLine(s.Key + "->" + "{" + s.Value.getNameSetState() + "}");
                }
                Console.WriteLine();
            }
        }
        public void checkForFinalState()
        {//функция проверки на наличие хотя бы одного финального состояния в списке состояния состояния-множества
            foreach (var state in stateNumbers)
                if (state.isFinalState)
                    isAnyFinalState = true;
        }
    }
}
