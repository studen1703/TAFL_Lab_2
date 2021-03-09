using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAFL_Lab_2
{
    class StateMachine
    {//класс автомата
        public State initialState; //начальное состояние - q0
        public List<State> states; //список состояний
        public List<SetState> setStates; //список состояний-множеств (для представления после детерменирования) 
        public StateMachine(List<State> States)
        {
            states = States;
            findInitialState();
        }
        public void findInitialState()
        {//функция нахождения начального состояния
            foreach (var s in states)
                if (s.stateNumber == 0 && !s.isFinalState)
                    initialState = s;
            if (initialState == null)
                throw new Exception("Нет начального состояния (q0)");
        }
        public bool isDetermenistic()
        {//функция проверки на то, является ли автомат детерминированным или нет
            foreach (var s in states)
                for (int j = 0; j < s.transitions.Count(); j++)
                    for (int i = 0; i < s.transitions.Count(); i++)
                            if (s.transitions[j].Key == s.transitions[i].Key && i!=j)
                                return false;
            return true;
        }
        public bool isHangs()
        {//функция проверки на наличие висячих вершин
            bool found = false;
            foreach (var s in states)
                foreach (var kp in s.transitions)
                    for (int i = 0; i < s.transitions.Count(); i++)
                    {
                        if (kp.Value.isFinalState) found = true;
                        else
                            if (kp.Value.stateNumber == s.stateNumber)
                            found = true;
                    }
            if (found) return false;
            else return true;
        }
        public bool isExpressionCorrect(char[] expr)
        {//функция проверки допустимости выражения
            if (setStates == null)
            {
                State current = new State(initialState);
                foreach (var c in expr)
                {
                    foreach (var kp in current.transitions)
                    {
                        if (kp.Key == c)
                        {
                            current = findState(kp.Value);
                            break;
                        }
                    }
                    if (current == null)
                        throw new Exception("Неверное выражение");
                }
                if (current.isFinalState)
                    return true;
                else return false;
            }
            else
            {
                SetState current = setStates[0];
                foreach (var c in expr)
                {
                    if (current.setTransitions.ContainsKey(c)) //если в словаре есть переход по этой букве
                    {
                        current = findSetState(current.setTransitions[c]);
                    }
                    else
                    {
                        return false; //если перехода не оказалось
                    }
                }
                if (current.isAnyFinalState)
                    return true;
                else return false;
            }
        }
        public SetState findSetState(SetState state)
        {//функция проверки на наличие состояния-множества в списке состояний-множеств автомата
            foreach (var s in setStates)
                if (state.isTheSameSetState(s))
                    return s;
            return state;
        }
        public State findState(State state)
        {//функция проверки на наличие состояния в списке состояний автомата
            foreach (var s in states)
                if (state.stateNumber == s.stateNumber && !state.isFinalState)
                    return s;
            return state;
        }
        public List<SetState> makeDeterminate()
        {//функция преобразования недетерминированного автомата в детерминированный
            List<SetState> allPossibleSetStates = new List<SetState>();
            SetState inProcessing = new SetState();
            inProcessing.stateNumbers.Add(states[0]); 
            allPossibleSetStates.Add(inProcessing);// добавили нулевой элемент
            for(int i = 0; i< allPossibleSetStates.Count; i++)
            {
                Dictionary<char, SetState> bufDictForNewSetState = findSetStatesForAllChars(allPossibleSetStates[i]);
                allPossibleSetStates[i].setTransitions = bufDictForNewSetState;//запомнили все переходы по всем буквам 

                foreach (SetState ss in bufDictForNewSetState.Values)
                    if (ss.stateNumbers.Count!=0)
                        if (isNewSetState(allPossibleSetStates, ss))
                            allPossibleSetStates.Add(ss);
                if (i == allPossibleSetStates.Count - 1)
                {
                    bufDictForNewSetState = findSetStatesForAllChars(allPossibleSetStates[i]);
                    allPossibleSetStates[i].setTransitions = bufDictForNewSetState;//запомнили все переходы по всем буквам для последнего
                }
            }
            setStates = allPossibleSetStates;
            return allPossibleSetStates;
        }
        public bool isNewSetState(List<SetState> list, SetState ss)
        {//булева функция проверки на наличие состояния-множества в списке состояний-множества автомата
            foreach (var a in list)
                if (a.isTheSameSetState(ss))
                    return false;
            return true;
        }
        public SetState U(List<SetState> lsst)
        {//функция объединения состояний-множеств в одно состояние-множество
            SetState ss = new SetState();
            foreach (var set in lsst)
                foreach (var state in set.stateNumbers)
                    if(!ss.hasThisState(state.stateNumber))
                            ss.stateNumbers.Add(findState(state));
            ss.stateNumbers.Sort();
            ss.checkForFinalState();
            return ss;
        }
        public List<char> getAlphabet()
        {//функция определения всех символов, по которым происходят переходы в автомате
            List<char> alphabet = new List<char>();
            foreach(var s in states)
            {
                foreach(var kp in s.transitions)
                {
                    alphabet.Add(kp.Key);
                }
            }
            int pos = 0;
            do
            {
                for (int i = 0; i < alphabet.Count(); i++)
                {
                    if (alphabet[pos] == alphabet[i] && pos != i)
                    {
                        alphabet.RemoveAt(i);
                        i--;
                    }
                }
                pos++;
            } while (pos < alphabet.Count());
            return alphabet;

        }
        public KeyValuePair<char, SetState> findSetStatesForChar(State state, char a)
        {//функция по определению перехода из одного состояния по одной букве в множество состояний
            KeyValuePair<char, SetState> SetStates = new KeyValuePair<char, SetState>();//a->{q1,q2,q3...}
            List<State> allStates = new List<State>();//{q1,q2,q3...}
            SetState stSt = new SetState();
            foreach (var kp in state.transitions)
            {
                if (kp.Key == a)
                   allStates.Add(findState(kp.Value));// добавляем в множество состояние, в которое переходит state
                
            }
            stSt.stateNumbers = allStates;
            SetStates = new KeyValuePair<char, SetState>(a, stSt);
            return SetStates;
        }
        public Dictionary<char, SetState> findSetStatesForAllChars(SetState Setstate)
        {//функция нахождения переходов из одного состояния в множества для всех букв  
            Dictionary<char, SetState> transition = new Dictionary<char, SetState>();//a->{q1,q3,..}; b->{q3,q4...}..
            List<char> alphabet = getAlphabet();
            foreach (char a in alphabet)
            {
                List<KeyValuePair<char, SetState>> buf = new List<KeyValuePair<char, SetState>>();
                foreach (State st in Setstate.stateNumbers)
                   buf.Add(findSetStatesForChar(findState(st), a)); //для одной буквы, но для всех состояний получили множества
                List<SetState> setlist = new List<SetState>();
                foreach (var set in buf)
                    setlist.Add(set.Value);
                SetState bufSetState = U(setlist);
                if(bufSetState.stateNumbers.Count!=0)
                transition.Add(a, bufSetState);
            }
            return transition;
        }
    }
}
