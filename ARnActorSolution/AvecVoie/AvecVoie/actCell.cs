using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Actor.Base ;
using Actor.Util ;

namespace AvecVoie
{
    
    public class LifeBoard : actActor
    {
        private long fSize;
        actCell[,] fBoard;
        private long fCycle = 0;
        public LifeBoard(long size) : base()
        {
            fSize = size;
            fBoard = new actCell[size, size];
            Become(new bhvBehavior<CellRequest>(DoRequest));
            AddBehavior(new bhvBehavior<string>(DoCycle));
        }

        private void DoRequest(CellRequest aRequest)
        {
            if ((aRequest.x >= 0) && (aRequest.y >=0) && (aRequest.x < fSize) && (aRequest.y < fSize))
            {
                // relay
                fBoard[aRequest.x, aRequest.y].SendMessage(aRequest);
            }
        }

        private void DoCycle(string msg)
        {
            // estimate
            fCycle++;
            for (int i = 0; i < fSize; i++)
                for (int j = 0; j < fSize; j++)
                {
                    CellRequest request = new CellRequest();
                    request.x = i;
                    request.y = j;
                    request.Sender = this;
                    request.Cycle = fCycle;
                    request.Order = 1;
                    if (fBoard[i,j] == null)
                    {
                        fBoard[i, j] = new actCell(i, j, 1);
                    }
                    fBoard[i, j].SendMessage(request);
                }
        }
    }

    public struct CellRequest
    {
        public int x;
        public int y;
        public int count;
        public int Order; // 0 need estimate;1 cycle;2 answer from estimate
        public IActor Sender;
        public int Status; // 0 dead 1 alive
        public long Cycle;
    }

    public class bhvCell : bhvBehavior<CellRequest>
    {
        private int CellStatus = 0; // 0 dead 1 alive
        private long x;
        private long y;
        private int Cycle = 0;
        private int NextStatus = 0; // 0 dead 1 alive

        public bhvCell(long px, long py, int aStatus)
            : base()
        {
            x = px;
            y = py;
            CellStatus = aStatus;
            Pattern = t => {return true ;} ;
            Apply = DoRequest ;
        }

        private void DoRequest(CellRequest aRequest)
        {
            if (aRequest.Order == 0)
            {
                // answer to neighbour status
                aRequest.Status = CellStatus ;
                aRequest.Order = 2;
                aRequest.Sender.SendMessage(aRequest);
            } else
                if (aRequest.Order == 2)
                {
                    NextStatus += aRequest.Status;
                }
                if (aRequest.Order == 1)
                {
                    // calculate neighbour
                    aRequest.Order = 0;
                    aRequest.x--;
                    aRequest.y--;
                    aRequest.Sender.SendMessage(aRequest) ;
                    aRequest.y++;
                    aRequest.Sender.SendMessage(aRequest);
                    aRequest.y++;
                    aRequest.Sender.SendMessage(aRequest);

                    aRequest.x++;
                    aRequest.y--;
                    aRequest.Sender.SendMessage(aRequest);
                    aRequest.y++;
                    // aRequest.Sender.SendMessage(aRequest);
                    aRequest.y++;
                    aRequest.Sender.SendMessage(aRequest);

                    aRequest.x++;
                    aRequest.y--;
                    aRequest.Sender.SendMessage(aRequest);
                    aRequest.y++;
                    aRequest.Sender.SendMessage(aRequest);
                    aRequest.y++;
                    aRequest.Sender.SendMessage(aRequest);
                }
        }
    }

    public class actCell : actActor
    {
        public actCell(long px, long py, int pstatus) : base()
        {
            Become(new bhvCell(px,py,pstatus));
        }
    }
}
