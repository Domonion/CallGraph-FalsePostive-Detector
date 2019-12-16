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

        public string Get()
        {
            return myCurrentStr;
        }
    }
}