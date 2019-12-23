using System;
using System.Text;

namespace CoverageExtractor
{
    public class DotReductor
    {
        private string myCurrentStr;

        public void Register(string str)
        {
            myCurrentStr = str;
        }

        private static bool IsSeparator(char c)
        {
            return c == ',' || c == '<' || c == '>' || c == '(' || c == ')';
        }

        private bool IsValid(int x)
        {
            return x >= 0 && x < myCurrentStr.Length;
        }

        public void Reduct(int l, int r)
        {
            //separators - ,|<|>|(|)
            //a-zA-Z0-9_
            //check reduct in lr
            if (!IsValid(l) || !IsValid(r) || myCurrentStr[l] != '(' || myCurrentStr[r] != ')')
            {
                return;
            }

            var was = l;
            var now = l + 1;
            while (was != r)
            {
                while (!IsSeparator(myCurrentStr[now]))
                {
                    now++;
                }

                var newInd = now - 1;
                while (!IsSeparator(myCurrentStr[newInd]) && myCurrentStr[newInd] != '.') newInd--;
                if (myCurrentStr[newInd] == '.')
                {
                    var diff = newInd - was;
                    myCurrentStr = myCurrentStr.Remove(was + 1, newInd - was);
                    now -= diff;
                    r -= diff;
                }

                was = now;
                now++;
            }
        }

        private int FindNext(int ind)
        {
            var balance = 0;
            var now = ind + 1;
            while (now < myCurrentStr.Length && (balance != 0 || myCurrentStr[now] != '>'))
            {
                if (myCurrentStr[now] == '<') balance++;
                else if (myCurrentStr[now] == '>') balance--;
                now++;
            }

            if (myCurrentStr.Length == now) return -1;
            return now;
        }

        private void ReplaceValueTypes()
        {
            const string pattern = "ValueTuple<";
            int now;
            while ((now = myCurrentStr.LastIndexOf(pattern, StringComparison.Ordinal)) != -1)
            {
                var forNext = now + pattern.Length - 1;
                var next = FindNext(forNext);
                var sb = new StringBuilder(myCurrentStr) {[next] = ')'};
                sb.Replace(pattern, "(", now, pattern.Length);
                myCurrentStr = sb.ToString();
            }
        }

        public string Get()
        {
            ReplaceValueTypes();
            return myCurrentStr;
        }
    }
}