using System.Collections.Generic;

namespace Skcrwblr
{
    public class Tracklist
    {
        private int maximumSize;
        private LinkedList<KcrwResponse> list;
        Stack<KcrwResponse> stack;
        public string Url { get; private set; }

        public Tracklist(string url, int maximumSize = 100)
        {
            list = new LinkedList<KcrwResponse>();
            stack = new Stack<KcrwResponse>(maximumSize);
            Url = url;
            this.maximumSize = maximumSize;
        }

        public void Add(KcrwResponse value)
        {
            list.AddLast(value);
            if (list.Count > maximumSize)
            {
                list.RemoveFirst();
            }
        }

        public int AddRange(KcrwResponse[] array)
        {
            bool listNotEmpty = list.Count > 0;
            foreach (KcrwResponse value in array)
            {
                if (listNotEmpty && value.PlayId == list.Last.Value.PlayId)
                {
                    break;
                }
                stack.Push(value);
            }
            int numAdded = stack.Count;
            while (stack.Count > 0)
            {
                Add(stack.Pop());
            }
            return numAdded;
        }

        public bool Contains(LinkedListNode<KcrwResponse> value)
        {
            return value.List.Equals(list);
        }

        public int Update()
        {
            KcrwResponse[] responses = KcrwApi.RequestAll(Url);
            int numAdded = AddRange(responses);
            return numAdded;
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public LinkedListNode<KcrwResponse> Last
        {
            get
            {
                return list.Last;
            }
        }
    }
}
