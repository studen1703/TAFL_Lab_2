using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL_Lab_2
{
    //класс для описания состояния
    class State: IComparable
    {
        public bool isFinalState; //показывает является ли состояние финальным
        public List<KeyValuePair<char, State>> transitions; //список переходов вида: символ - состояние (а, q1)
        public int stateNumber; //номер состояния (для q1 - 1)
        public State()
        {
            transitions = new List<KeyValuePair<char, State>>();
        }
        public State(State duplicate)
        {
            isFinalState = duplicate.isFinalState;
            stateNumber = duplicate.stateNumber;
            transitions = new List<KeyValuePair<char, State>>();
            foreach (var t in duplicate.transitions)
                    transitions.Add(t);
        }
        public State(int num, bool isF)
        {
            stateNumber = num;
            isFinalState = isF;
            transitions = new List<KeyValuePair<char, State>>();
        }
        public bool isTheSameState(State anotherState)
        {//функция проверки на идентичность состояний
            if (isFinalState == anotherState.isFinalState
                && stateNumber == anotherState.stateNumber
                    && transitions.Count() == anotherState.transitions.Count())
                for (int i = 0; i < this.transitions.Count(); i++)
                    if (transitions[i].Key == anotherState.transitions[i].Key
                        && transitions[i].Value.stateNumber == anotherState.transitions[i].Value.stateNumber)
                        return true;
            return false;
        }
        public void showState()
        {//функция отображения состояния на дисплей
            string c = isFinalState ? "f" : "q";
            Console.WriteLine("Состояние: " + c + stateNumber.ToString());
            if (transitions.Count != 0)
            {
                Console.WriteLine("Переходы: ");
                foreach (var v in transitions)
                {
                    c = v.Value.isFinalState ? "f" : "q";
                    Console.WriteLine(v.Key.ToString() + " -> " + c + v.Value.stateNumber);
                }
            }
            else Console.WriteLine("Переходов нет: ");
        }
        public int CompareTo(object o)
        {//функция сравнения
            State s = o as State;
            if (s != null)
                return this.stateNumber.CompareTo(s.stateNumber);
            else
                throw new Exception("Невозможно сравнить два объекта");
        }
    }
}
