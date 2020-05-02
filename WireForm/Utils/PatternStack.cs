using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wireform.MathUtils
{
    public class PatternStack<T>
    {
        PatternStackNode<T> root = null;
        PatternStackNode<T> head;

        public int HeadIndex { get; private set; } = 0;

        /// <summary>
        /// The current set of values which may or may not be the start of a loop
        /// </summary>
        public List<T> CurrentPattern { get; set; }

        /// <summary>
        /// The start index of the pattern
        /// </summary>
        int patternStartIndex = 0;
        PatternStackNode<T> patternStartNode = null;

        /// <summary>
        /// The first node in the tentative set of nodes which match the pattern
        /// </summary>
        public PatternStackNode<T> matchedStartNode { get; private set; }
        int matchedStartIndex = 0;


        public PatternStack()
        {
            CurrentPattern = new List<T>();
        }

        public bool Push(T value)
        {
            PatternStackNode<T> node = new PatternStackNode<T>(null, head, value);
            if (head != null)
            {
                head.Next = node;
            }
            else
            {
                root = node;
                HeadIndex = -1;
            }
            head = node;

            HeadIndex++;
            return AddToPattern(head);
        }

        public T Peek()
        {
            if(head == null)
            {
                return default(T);
            }
            return head.Value;
        }

        public T Pop()
        {
            T value = head.Value;
            head = head.Previous;
            head.Next = null;

            HeadIndex--;
            if(HeadIndex == 0)
            {
                root = null;
            }


            ResetPattern();
            return value;
        }

        public void Pop(int count)
        {
            if(count == 0)
            {
                return;
            }

            for(int i = 0; i < count; i++)
            {
                head = head.Previous;
            }
            head.Next = null;

            HeadIndex -= count;
            if (HeadIndex == -1)
            {
                root = null;
                head = null;
                HeadIndex = 0;
            }
            else
            {

                ResetPattern();
            }
        }

        public bool Contains(T value)
        {
            for(var current = root; current != null; current = current.Next)
            {
                if(current.Value.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEmpty()
        {
            return root == null;
        }

        /// <summary>
        /// Adds a new node to the current pattern and revalidates
        /// </summary>
        /// <returns>
        /// Returns true if pattern is matched fully and repetition is hit
        /// </returns>
        private bool AddToPattern(PatternStackNode<T> node)
        {
            CurrentPattern.Add(node.Value);
            
            if(patternStartNode == null)
            {
                patternStartNode = node;
                patternStartIndex = HeadIndex;
            }


            if(matchedStartNode != null)
            {
                if (CheckIfMatches())
                {
                    if (PatternFull())
                    {
                        return true;
                    }
                    return false;
                }
            }

            if (FindMatch())
            {
                if (PatternFull())
                {
                    return true;
                }
                return false;
            }

            if (MinimizePattern())
            {
                if (PatternFull())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Shrinks pattern until it finds a match or becomes empty
        /// </summary>
        /// <returns>
        /// Returns false if pattern becomes empty
        /// </returns>
        private bool MinimizePattern()
        {
            while(patternStartIndex != HeadIndex + 1)
            {
                if (FindMatch())
                {
                    return true;
                }
                CurrentPattern.RemoveAt(0);
                patternStartIndex++;
                patternStartNode = patternStartNode.Next;
            }

            matchedStartIndex = 0;
            patternStartIndex = 0;
            matchedStartNode = null;
            patternStartNode = null;
            return false;
        }

        /// <summary>
        /// Returns true if the end of the match meets up to the beginning of the pattern
        /// </summary>
        private bool PatternFull()
        {
            return matchedStartIndex + CurrentPattern.Count - 1 == patternStartIndex - 1;
        }

        /// <summary>
        /// Tries to find a match for the current Pattern
        /// </summary>
        /// <returns>
        /// Returns true if pattern is matched by match nodes. 
        /// Note: Does not require the match to be a full match (repetition might not be hit)
        /// </returns>
        private bool FindMatch()
        {
            if(patternStartIndex == 0)
            {
                return false;
            }

            var currentNode = patternStartNode.Previous;
            for(int i = patternStartIndex - 1; i >= 0; i--)
            {
                matchedStartIndex = i;
                matchedStartNode = currentNode;
                if (CheckIfMatches())
                {
                    return true;
                }
                currentNode = currentNode.Previous;
            }

            matchedStartNode = null;
            matchedStartIndex = 0;
            return false;
        }

        /// <summary>
        /// Does the set of nodes starting at matchedStartNode match the currentPattern
        /// </summary>
        /// <returns>
        /// Returns true if pattern is matched by match nodes. 
        /// Note: Does not require the match to be a full match (repetition might not be hit)
        /// </returns>
        private bool CheckIfMatches()
        {
            var currentNode = matchedStartNode;
            for(int i = 0; i < CurrentPattern.Count; i++)
            {
                if (!currentNode.Value.Equals(CurrentPattern[i]))
                {
                    return false;
                }
                currentNode = currentNode.Next;
            }
            return true;
        }

        private void ResetPattern()
        {
            matchedStartIndex = 0;
            patternStartIndex = 0;
            patternStartNode = root;
            matchedStartNode = null;

            CurrentPattern.Clear();
            for(var currentNode = root; currentNode != null; currentNode = currentNode.Next)
            {
                CurrentPattern.Add(currentNode.Value);
            }

            MinimizePattern();
        }
    }

    public sealed class PatternStackNode<T>
    {
        public PatternStackNode<T> Next;
        public PatternStackNode<T> Previous;
        public T Value;

        public PatternStackNode(PatternStackNode<T> Next, PatternStackNode<T> Previous, T Value)
        {
            this.Next = Next;
            this.Previous = Previous;
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
