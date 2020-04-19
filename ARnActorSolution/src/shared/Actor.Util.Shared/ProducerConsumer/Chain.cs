using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Actor.Base;

namespace Actor.Util
{
    public class Chain : BaseActor
    {
        public Chain() : base()
        {
            Become(new Behavior<int, int, int>(Start));
        }

        private void Start(int mi, int mj, int mk)
        {
            List<Consumer<long>> list = new List<Consumer<long>>();

            for (int i = 0; i < mj; i++)
            {
                list.Add(new Consumer<long>());
            }

            Buffer<long> buffer = new Buffer<long>(list);

            List<Producer<long>> list2 = new List<Producer<long>>();

            for (int i = 0; i < mi; i++)
            {
                list2.Add(new Producer<long>(buffer));
            }

            for (long i = 0; i <= mk; i++)
            {
                foreach (var prod in list2)
                {
                    prod.SendMessage(i);
                }
            }

            while (true)
            {
                var fut = buffer.GetCurrentState().Result(10000);
                if (fut == null)
                {
                    Debug.WriteLine("Stop");
                }

                if (fut != null && fut == "BufferEmpty")
                {
                    break;
                }
            }

            Debug.WriteLine("End of chain");
        }
    }
}
